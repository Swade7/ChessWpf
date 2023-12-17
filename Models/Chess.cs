using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ChessWpf.Models.Pieces;


namespace ChessWpf.Models
{
    public enum PieceType 
    {
        Empty,
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }
    public enum Player
    {
        White,
        Black,
        None
    }

    public struct Move
    {
        public int FromCol;
        public int FromRow;
        public int ToCol;
        public int ToRow;
    }

    public struct Loc
    {
        public int Col;
        public int Row;
    }

    public enum Status
    {
        Active,
        WhiteWin,
        BlackWin,
        Stalemate,
        GameOver
    }

    
    public class Chess
    {
        public const int BOARD_SIZE = 8;
        private const int WHITE_ROW = 0;
        private const int BLACK_ROW = BOARD_SIZE - 1;

        Player currentPlayer;
        List<Move> moves;
        int movesSincePawnMovedOrPieceCaptured;
        bool hasWhiteCastled;
        bool hasBlackCastled;
        Piece[,] board;
        List<Piece> whitePieces;
        List<Piece> blackPieces;

        // Properties
        public Player CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
            private set
            {
                currentPlayer = value;
            }
        }
        public Player Opponent
        {
            get
            {
                if (CurrentPlayer == Player.White)
                {
                    return Player.Black;
                }
                else
                {
                    return Player.White;
                }
            }

        }
        public Piece[,] Board
        {
            get
            {
                return board;
            }
        }

        public Move? LastMove
        {
            get
            {
                if (moves.Count > 0)
                {
                    return moves[moves.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Move> PossibleMoves {
            get
            {
                // Create a list to store the possible moves
                List<Move> moves = new List<Move>();

                // Iterate over the board to find the Piece that belong to the currentPlayer
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    for (int row = 0; row < BOARD_SIZE; row++)
                    {
                        if (board[col, row].Player == CurrentPlayer)
                        {
                            // Check each possible "to" location
                            for (int toCol = 0; toCol < BOARD_SIZE; toCol++)
                            {
                                for (int toRow = 0; toRow < BOARD_SIZE; toRow++)
                                {
                                    Move move = new Move
                                    {
                                        FromCol = col,
                                        FromRow = row,
                                        ToCol = toCol,
                                        ToRow = toRow
                                    };
                                    if (board[col, row].CheckValidMove(move, Board, CurrentPlayer, LastMove) && !WouldBeCheck(move))
                                    {
                                        moves.Add(move);
                                    }
                                }
                            }
                        }
                    }
                }

                return moves;
            }
        }

        public int NumMoves
        {
            get
            {
                return moves.Count;
            }
        }

        public List<Piece> BlackPieces {
            get
            {
                return blackPieces;
            }
        }
        public List<Piece> WhitePieces 
        {
            get
            {
                return whitePieces;
            }
        }


        // Constructors
        public Chess()
        {
            InitializeBoard();
        }
        public Chess(Chess rhs)
        {
            // Set the variables of the copy to the correct values
            currentPlayer = rhs.currentPlayer;
            hasWhiteCastled = rhs.hasWhiteCastled;
            hasBlackCastled = rhs.hasBlackCastled;
            moves = rhs.moves;
            movesSincePawnMovedOrPieceCaptured = rhs.movesSincePawnMovedOrPieceCaptured;
            board = rhs.board;
            whitePieces = rhs.whitePieces;
            blackPieces = rhs.blackPieces;
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                board[i, WHITE_ROW + 1] = new Pawn(Player.White);
                board[i, BLACK_ROW - 1] = new Pawn(Player.Black);
            }

            // Rooks
            board[0, WHITE_ROW] = new Rook(Player.White);
            board[0, BLACK_ROW] = new Rook(Player.Black);
            board[7, WHITE_ROW] = new Rook(Player.White);
            board[7, BLACK_ROW] = new Rook(Player.Black);

            // Knights
            board[1, WHITE_ROW] = new Knight(Player.White);
            board[1, BLACK_ROW] = new Knight(Player.Black);
            board[6, WHITE_ROW] = new Knight(Player.White);
            board[6, BLACK_ROW] = new Knight(Player.Black);

            // Bishops
            board[2, WHITE_ROW] = new Bishop(Player.White);
            board[2, BLACK_ROW] = new Bishop(Player.Black);
            board[5, WHITE_ROW] = new Bishop(Player.White);
            board[5, BLACK_ROW] = new Bishop(Player.Black);

            // Queen
            board[3, WHITE_ROW] = new Queen(Player.White);
            board[3, BLACK_ROW] = new Queen(Player.Black);

            // King
            board[4, WHITE_ROW] = new King(Player.White);
            board[4, BLACK_ROW] = new King(Player.Black);

            // Add the pieces to the lists
            whitePieces = new List<Piece>();
            blackPieces = new List<Piece>();
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                whitePieces.Add(board[col, WHITE_ROW]);
                whitePieces.Add(board[col, WHITE_ROW + 1]);
                blackPieces.Add(board[col, BLACK_ROW]);
                blackPieces.Add(board[col, BLACK_ROW - 1]);
            }

            // Initialize the empty places
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                for (int row = WHITE_ROW + 2; row <= BLACK_ROW - 2; row++)
                {
                    board[col, row] = new Empty();
                }
            }

            // Set the current player to white
            currentPlayer = Player.White;

            movesSincePawnMovedOrPieceCaptured = 0;
        }

        public void MakeMove(Move move)
        {
            Piece piece = GetPiece(move.FromCol, move.FromRow);
            PieceType pieceType = piece.PieceType;

            if (piece.CheckValidMove(move, board, currentPlayer, LastMove))
            {
                if (!WouldBeCheck(move))
                {
                    if (pieceType == PieceType.King && Math.Abs(move.FromCol - move.ToCol) == 2)
                    {
                        // Check if the King would move through check
                        Piece[,] tempBoard = (Piece[,])board.Clone();
                        for (int i = Math.Min(move.FromCol, move.ToCol) + 1; i < Math.Max(move.FromCol, move.ToCol); i++)
                        {
                            tempBoard[i, move.FromRow] = new King(currentPlayer);
                            tempBoard[i - 1, move.FromRow] = new Empty();
                            if (UnderAttack(i, move.FromRow))
                            {
                                return;
                            }
                        }
                        Castle(move);
                    }
                    else if (pieceType == PieceType.Pawn && Math.Abs(move.FromCol - move.ToCol) == 2)
                    {
                        EnPassant(move);
                    }

                    UpdateBoard(move);
                    piece.UpdatePiece();
                    moves.Add(move);

                    if (pieceType == PieceType.Pawn && (move.ToRow == 0 || move.ToRow == BOARD_SIZE - 1))
                    {
                        PawnToQueen(move);
                    }

                    ChangeTurn();
                }
                else
                {
                    Console.WriteLine("The move would put you in check.");
                }
            }
            else
            {
                Console.WriteLine("Invalid move.");
            }
        }

        private Piece GetPiece(int col, int row)
        {
            return board[col, row];
        }

        public Status UpdateStatus()
        {
            if (IsStalemate())
            {
                return Status.Stalemate;
            }
            else if (Checkmate())
            {
                if (currentPlayer == Player.White)
                {
                    return Status.BlackWin;
                }
                else if (currentPlayer == Player.Black)
                {
                    return Status.WhiteWin;
                }
            }

            return Status.Active;
        }

        // Checks for check/checkmate/stalemate
        public bool Checkmate()
        {
            if (Check())
            {
                // Get the king's location
                int kingCol = -1;
                int kingRow = -1;

                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    for (int row = 0; row < BOARD_SIZE; row++)
                    {
                        // Get the piece at the location
                        Piece piece = GetPiece(col, row);
                        if (piece.PieceType == PieceType.King && piece.Player == CurrentPlayer)
                        {
                            kingCol = col;
                            kingRow = row;

                            break;
                        }
                    }
                    if (kingCol != -1 && kingRow != -1)
                    {
                        break;
                    }
                }

                // Check if there are any possible moves that would put the player no longer in check
                if (PossibleMoves.Count == 0)
                {
                    Console.WriteLine("Checkmate");
                    return true;
                }
            }

            return false;
        }

        public bool Check()
        {
            // Declare variables for the king location
            int kingCol = -1;
            int kingRow = -1;


            // Locate the king
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                for (int row = 0; row < BOARD_SIZE; row++)
                {
                    // Get the piece at the location
                    Piece piece = GetPiece(col, row);
                    if (piece.PieceType == PieceType.King && piece.Player == currentPlayer)
                    {
                        kingCol = col;
                        kingRow = row;

                        break;
                    }
                }
                if (kingCol != -1 && kingRow != -1)
                {
                    break;
                }
            }

            return UnderAttack(kingCol, kingRow);
        }
        public bool IsStalemate()
        {
            if (!Check())
            {
                if (PossibleMoves.Count == 0)
                {
                    return true;
                }
            }

            // Check if the number of moves since a pawn moved or a piece was captured is 50
            if (movesSincePawnMovedOrPieceCaptured >= 50)
            {
                return true;
            }
            return false;
        }
        public bool WouldBeCheck(Move move)
        {
            // Create a copy of the game to test a move
            Chess chessCopy = new Chess(this);

            Piece piece = chessCopy.GetPiece(move.FromCol, move.FromRow);

            // Update the board	
            chessCopy.UpdateBoard(move);
            piece.UpdatePiece();

            // Return if the user would be in check as a result of the move
            return chessCopy.Check();
        }

        public bool UnderAttack(int pieceCol, int pieceRow)
        {
            // Get the opponent
            Player opponent = Opponent;

            // Iterate through the rest of the board and check if the piece is under attack
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                for (int row = 0; row < BOARD_SIZE; row++)
                {
                    // Get the piece at the location
                    Piece piece = GetPiece(col, row);

                    if (piece.Player == Opponent)
                    {
                        // Create a Move variable for formatting
                        Move move = new Move
                        {
                            FromCol = col,
                            FromRow = row,
                            ToCol = pieceCol,
                            ToRow = pieceRow
                        };

                        if (piece.CheckValidMove(move, Board, opponent, LastMove))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Castle(Move move)
        {
            // Get the location of the Rook and move it
            Move rookMove = new Move();

            // Set the to and from row based on the player's color
            if (CurrentPlayer == Player.White)
            {
                rookMove.FromRow = WHITE_ROW;
                rookMove.ToRow = WHITE_ROW;
            }
            else if (CurrentPlayer == Player.Black)
            {
                rookMove.FromRow = BLACK_ROW;
                rookMove.ToRow = BLACK_ROW;
            }
            else
            {
                return;
            }

            // Determine if the rook to the left or right should be moved
            // Left
            if (move.FromCol > move.ToCol)
            {
                rookMove.FromCol = 0;
                rookMove.ToCol = move.ToCol + 1;
            }
            // Right
            else if (move.FromCol < move.ToCol)
            {
                rookMove.FromCol = BOARD_SIZE - 1;
                rookMove.ToCol = move.ToCol - 1;
            }
            else
            {
                return;
            }

            // Get the Rook at the location
            Rook rook = (Rook)GetPiece(rookMove.FromCol, rookMove.FromRow);

            // Update the piece and the board
            UpdateBoard(rookMove);
            rook.UpdatePiece();
        }
        private void EnPassant(Move move)
        {
            // Get the location of the pawn being captured
            if (currentPlayer == Player.White)
            {
                board[move.ToRow + 1, move.ToCol] = new Empty();
            }
            else if (currentPlayer == Player.Black)
            {
                board[move.ToRow - 1, move.ToCol] = new Empty();
            }
            else
            {
                return;
            }
        }
        private void PawnToQueen(Move move)
        {
            board[move.ToRow, move.ToCol] = new Queen(currentPlayer);
        }
        private void ChangeTurn()
        {
            currentPlayer = (currentPlayer == Player.White) ? Player.Black : Player.White;
        }
        private void UpdateBoard(Move move)
        {
            if (board[move.ToCol, move.ToRow].PieceType != PieceType.Empty)
            {
                // Add captured piece handling here if needed
            }

            if (board[move.FromCol, move.FromRow].PieceType == PieceType.Pawn ||
                board[move.ToCol, move.ToRow].PieceType != PieceType.Empty)
            {
                movesSincePawnMovedOrPieceCaptured = 0;
            }
            else
            {
                movesSincePawnMovedOrPieceCaptured++;
            }

            // Move the piece and setthe previous space to Empty
            board[move.ToCol, move.ToRow] = board[move.FromCol, move.FromRow];
            board[move.FromCol, move.FromRow] = new Empty();
        }
    }
}
