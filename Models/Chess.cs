﻿using System;
using System.Collections.Generic;
using System.Windows;
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
        public int FromRow;
        public int FromCol;
        public int ToRow;
        public int ToCol;
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
        private const int MAX_MOVES_SINCE_PAWN_MOVED_OR_PIECE_CAPTURED = 50;

        Player currentPlayer;
        List<Move> moves;
        int movesSincePawnMovedOrPieceCaptured;
        bool hasWhiteCastled;
        bool hasBlackCastled;
        Piece[,] board;
        Point selectedLocation;
        List<Point> whitePieces;
        List<Point> blackPieces;
        Point blackKingLocation;
        Point whiteKingLocation;

        // Properties
        public int BoardSize
        {
            get
            {
                return BOARD_SIZE;
            }
        }
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
                return CurrentPlayer == Player.White ? Player.Black : Player.White;
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

                foreach (Point point in (currentPlayer == Player.White) ? whitePieces : blackPieces)
                {
                    int row = (int)point.X;
                    int col = (int)point.Y;

                    // Check each possible "to" location
                    for (int toRow = 0; toRow < BOARD_SIZE; toRow++)
                    {
                        for (int toCol = 0; toCol < BOARD_SIZE; toCol++)
                        {
                            Move move = new Move
                            {
                                FromCol = col,
                                FromRow = row,
                                ToCol = toCol,
                                ToRow = toRow
                            };
                            if (board[row, col].CheckValidMove(move, Board, CurrentPlayer, LastMove) && !WouldBeCheck(move))
                            {
                                moves.Add(move);
                            }
                        }
                    }
                }

                return moves;
            }
        }      

    public List<Move> PossibleMovesForSelectedPiece
        {
            get
            {
                List<Move> moves = new List<Move>();

                for (int toRow = 0; toRow < BOARD_SIZE; toRow++)
                {
                    for (int toCol = 0; toCol < BOARD_SIZE; toCol++)
                    {
                        Move move = new Move
                        {
                            FromCol = (int)selectedLocation.Y,
                            FromRow = (int)selectedLocation.X,
                            ToCol = toCol,
                            ToRow = toRow
                        };
                        try
                        {
                            if (board[(int)selectedLocation.X, (int)selectedLocation.Y].CheckValidMove(move, Board, CurrentPlayer, LastMove) && !WouldBeCheck(move))
                            {
                                // Prevent invalid castling moves from being added to the list
                                if (board[(int)selectedLocation.X, (int)selectedLocation.Y].PieceType == PieceType.King && Math.Abs(move.FromCol - move.ToCol) == 2)
                                {
                                    King king = (King)board[(int)selectedLocation.X, (int)selectedLocation.Y];
                                    if (king.HasMoved)
                                    {
                                        continue;
                                    }
                                    if (UnderAttack(move.FromRow, move.FromCol))
                                    {
                                        continue;
                                    }
                                }
                                moves.Add(move);
                            }
                        }
                        catch (Exception)
                        {
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

        // Returns the location selected by the user or (-1, -1) if no location is selected
        public Point SelectedLocation
        {
            get {  return selectedLocation; }
            set
            {
                if (value.X >= 0 && value.X < BOARD_SIZE && value.Y >= 0 && value.Y < BOARD_SIZE)
                {
                    selectedLocation = value;
                }
            }
        }
        
        public List<Point> BlackPieces {
            get
            {
                return blackPieces;
            }
            set 
            {  
                blackPieces = value;
            }
        }
        public List<Point> WhitePieces 
        {
            get
            {
                return whitePieces;
            }
            set
            {
                whitePieces = value;
            }
        }

        public Point BlackKingLocation
        {
            get
            {
                return blackKingLocation;
            }
            set
            {
                blackKingLocation = value;
            }
        }

        public Point WhiteKingLocation
        {
            get
            {
                return whiteKingLocation;
            }
            set
            {
                whiteKingLocation = value;
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
            selectedLocation = rhs.selectedLocation;
            blackKingLocation = rhs.blackKingLocation;
            whiteKingLocation = rhs.whiteKingLocation;
        }

        private void InitializeBoard()
        {
            board = new Piece[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                board[WHITE_ROW + 1, i] = new Pawn(Player.White);
                board[BLACK_ROW - 1, i] = new Pawn(Player.Black);
            }

            // Rooks
            board[WHITE_ROW, 0] = new Rook(Player.White);
            board[BLACK_ROW, 0] = new Rook(Player.Black);
            board[WHITE_ROW, 7] = new Rook(Player.White);
            board[BLACK_ROW, 7] = new Rook(Player.Black);

            // Knights
            board[WHITE_ROW, 1] = new Knight(Player.White);
            board[BLACK_ROW, 1] = new Knight(Player.Black);
            board[WHITE_ROW, 6] = new Knight(Player.White);
            board[BLACK_ROW, 6] = new Knight(Player.Black);

            // Bishops
            board[WHITE_ROW, 2] = new Bishop(Player.White);
            board[BLACK_ROW, 2] = new Bishop(Player.Black);
            board[WHITE_ROW, 5] = new Bishop(Player.White);
            board[BLACK_ROW, 5] = new Bishop(Player.Black);

            // Queen
            board[WHITE_ROW, 3] = new Queen(Player.White);
            board[BLACK_ROW, 3] = new Queen(Player.Black);

            // King
            board[WHITE_ROW, 4] = new King(Player.White);
            board[BLACK_ROW, 4] = new King(Player.Black);
            WhiteKingLocation = new Point(WHITE_ROW, 4);
            BlackKingLocation = new Point(BLACK_ROW, 4);

            // Add the pieces to the lists
            whitePieces = new List<Point>();
            blackPieces = new List<Point>();
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                whitePieces.Add(new Point(WHITE_ROW, col));
                whitePieces.Add(new Point(WHITE_ROW + 1, col));
                blackPieces.Add(new Point(BLACK_ROW, col));
                blackPieces.Add(new Point(BLACK_ROW - 1, col));
            }

            // Initialize the empty tiles
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                for (int row = WHITE_ROW + 2; row <= BLACK_ROW - 2; row++)
                {
                    board[row, col] = Empty.Instance;
                }
            }

            moves = new List<Move>();

            // Set the current player to white
            currentPlayer = Player.White;

            movesSincePawnMovedOrPieceCaptured = 0;

            selectedLocation = new Point(-1, -1);
        }

        // Attempts to make a move and returns true if the move is valid and false otherwise
        public bool MakeMove(Move move)
        {
            Piece piece = GetPiece(move.FromRow, move.FromCol);
            
            if (PossibleMovesForSelectedPiece.Contains(move) && piece.PieceType != PieceType.Empty)
            {
                if (piece.PieceType == PieceType.King && Math.Abs(move.FromCol - move.ToCol) == 2)
                {
                    King king = (King)piece;

                    if (king.HasMoved)
                    {
                        return false;
                    }

                    // Check if the King would move through check
                    for (int i = Math.Min(move.FromCol, move.ToCol); i < Math.Max(move.FromCol, move.ToCol); i++)
                    {
                        if (UnderAttack(move.FromRow, i))
                        {
                            return false;
                        }
                    }

                    try
                    {
                        Castle(move);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                else if (CanEnPassant(move))
                {
                    EnPassant(move);
                }

                UpdatePlayerPieces(move);
                UpdateBoard(move);
                piece.UpdatePiece();
                moves.Add(move);

                if (piece.PieceType == PieceType.Pawn && (move.ToRow == 0 || move.ToRow == BOARD_SIZE - 1))
                {
                    PawnToQueen(move);
                }

                ChangeTurn();

                return true;
            }

            return false;
        }

        // Update the lists containing the pieces of each player
        private void UpdatePlayerPieces(Move move)
        {
            List<Point> playerPieces = (currentPlayer == Player.White) ? whitePieces : blackPieces;
            List<Point> opponentPieces = (currentPlayer == Player.White) ? blackPieces : whitePieces;

            // Remove a captured piece from the appropriate list
            if (board[move.ToRow, move.ToCol].Player == Opponent)
            {
                opponentPieces.Remove(new Point(move.ToRow, move.ToCol));
            }
            playerPieces.Remove(new Point(move.FromRow, move.FromCol));
            playerPieces.Add(new Point(move.ToRow, move.ToCol));
        }

        private Piece GetPiece(int row, int col)
        {
            return board[row, col];
        }

        // Returns the current state of the game
        public Status UpdateStatus()
        {          
            if (IsStalemate())
            {
                return Status.Stalemate;
            }
            
            else if (Checkmate())
            {
                return currentPlayer == Player.White ? Status.BlackWin : Status.WhiteWin;
            }

            else
            {
                return Status.Active;
            }
        }

        public void ResetSelectedLocation()
        {
            selectedLocation = new Point(-1, -1);
        }

        // Checks for check/checkmate/stalemate
        public bool Checkmate()
        {
            return PossibleMoves.Count == 0 && Check();
        }

        // Check if the current player is in check
        public bool Check()
        {
            Point kingLocation = (currentPlayer == Player.White) ? WhiteKingLocation : BlackKingLocation;

            // Locate the king (Should this be needed?)
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if (board[row, col].PieceType == PieceType.King && board[row, col].Player == currentPlayer)
                    {
                        kingLocation = new Point(row, col);
                        break;
                    }
                }
            }

            return UnderAttack((int)kingLocation.X, (int)kingLocation.Y);
        }

        public bool IsStalemate()
        {
            return (PossibleMoves.Count == 0 && !Check()) || movesSincePawnMovedOrPieceCaptured >= MAX_MOVES_SINCE_PAWN_MOVED_OR_PIECE_CAPTURED;
        }

        public bool WouldBeCheck(Move move)
        {
            // Create a copy of the game to test a move
            Chess chessCopy = new Chess(this);
            chessCopy.UpdateBoard(move);

            // Return if the user would be in check as a result of the move
            return chessCopy.Check();
        }

        // Check if a piece could be captured by the opponent's piece
        public bool UnderAttack(int pieceRow, int pieceCol)
        {
            foreach (Point point in (currentPlayer == Player.White) ? blackPieces : whitePieces)
            {
                // Create a Move variable for formatting
                Move move = new Move
                {
                    FromRow = (int)point.X,
                    FromCol = (int)point.Y,
                    ToRow = pieceRow,
                    ToCol = pieceCol
                };

                // Get the piece at the location
                Piece piece = GetPiece(move.FromRow, move.FromCol);

                // Check if the piece can attack the location
                if (piece.CheckValidMove(move, Board, Opponent, LastMove))
                {
                    return true;
                }
            }

            // Piece is not under attack
            return false;
        }

        private void Castle(Move move)
        {
            // Get the location of the Rook and move it
            Move rookMove = new Move();

            // Set the to and from row based on the player's color
            rookMove.FromRow = move.FromRow;
            rookMove.ToRow = move.ToRow; 

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

            // Get the Rook at the location
            Rook rook = (Rook)GetPiece(rookMove.FromRow, rookMove.FromCol);

            // Update the piece and the board
            UpdateBoard(rookMove);
            rook.UpdatePiece();

            // Get the column of the king (3 for White, 5 for Black)
            int kingCol = (rookMove.FromCol == 0) ? 3 : 5;
            
            // Update the king's location
            UpdateKingLocation(new Point(rookMove.FromRow, kingCol));
        }
        private void EnPassant(Move move)
        {
            // Capture the opposing pawn (set the location to Empty)
            if (currentPlayer == Player.White)
            {
                board[move.ToRow - 1, move.ToCol] = Empty.Instance;
            }
            else if (currentPlayer == Player.Black)
            {
                board[move.ToRow + 1, move.ToCol] = Empty.Instance;
            }
        }

        private bool CanEnPassant(Move move)
        {
            // Check if the move is a valid en passant move
            return (board[move.ToRow, move.ToCol].PieceType == PieceType.Empty
                && board[move.FromRow, move.FromCol].PieceType == PieceType.Pawn
                && move.FromCol != move.ToCol);
        }
        private void PawnToQueen(Move move)
        {
            board[move.ToRow, move.ToCol] = new Queen(currentPlayer);
        }
        private void ChangeTurn()
        {
            currentPlayer = (currentPlayer == Player.White) ? Player.Black : Player.White;
        }

        private void UpdateKingLocation(Point kingLocation)
        {
            if (currentPlayer == Player.White)
            {
                WhiteKingLocation = kingLocation;
            }
            else if (currentPlayer == Player.Black)
            {
                BlackKingLocation = kingLocation;
            }
        }

        private void UpdateBoard(Move move)
        {
            if (board[move.ToRow, move.ToCol].PieceType != PieceType.Empty)
            {
                // Add captured piece handling here if needed
            }

            if (board[move.FromRow, move.FromCol].PieceType == PieceType.Pawn ||
                board[move.ToRow, move.ToCol].PieceType != PieceType.Empty)
            {
                movesSincePawnMovedOrPieceCaptured = 0;
            }
            else
            {
                movesSincePawnMovedOrPieceCaptured++;
            };

            // Create a new board to store the updated board
            Piece[,] newBoard = new Piece[BOARD_SIZE, BOARD_SIZE];
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if (row == move.ToRow && col == move.ToCol)
                    {
                        // Move the piece and update it
                        newBoard[row, col] = board[move.FromRow, move.FromCol].Clone(board[move.FromRow, move.FromCol]);
                        newBoard[row, col].UpdatePiece();
                    }
                    else if (row == move.FromRow && col == move.FromCol)
                    {
                        newBoard[row, col] = Empty.Instance;
                    }
                    else
                    {
                        newBoard[row, col] = board[row, col].Clone(board[row, col]);
                    }                    
                }
            }

            // Set the board to the new board
            board = newBoard;

            // Update the king's location
            if (board[move.ToRow, move.ToCol].PieceType == PieceType.King && Math.Abs(move.ToCol - move.FromCol) == 1)
            {
                UpdateKingLocation(new Point(move.ToRow, move.ToCol));
            }
        }
    }
}
