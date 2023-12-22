using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class Queen : Piece
    {
        private const int BOARD_SIZE = 8;

        // Constructor
        public Queen(Player player) : base(PieceType.Queen, player) { }

        // Copy
        public override Piece Clone()
        {
            return new Queen(Player);
        }

        private bool CheckValidStraightMove(Move move, Piece[,] board)
        {
            // Check if trying to move diagonally
            if (move.FromCol != move.ToCol && move.FromRow != move.ToRow)
            {
                return false;
            }

            // Get the direction of the move
            char direction;
            if (move.FromCol == move.ToCol)
            {
                direction = 'v';
            }
            else
            {
                direction = 'h';
            }

            // Check if there are pieces in the way
            if (direction == 'v')
            {
                // Check if moving up
                if (move.ToRow < move.FromRow)
                {
                    for (int i = move.FromRow - 1; i > move.ToRow; i--)
                    {
                        if (board[i, move.FromCol].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
                // Check if moving down
                else
                {
                    for (int i = move.FromRow + 1; i < move.ToRow; i++)
                    {
                        if (board[i, move.FromCol].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }
            // Horizontal
            else
            {
                // Check if moving left
                if (move.ToCol < move.FromCol)
                {
                    for (int i = move.FromCol - 1; i > move.ToCol; i--)
                    {
                        if (board[move.FromRow, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
                // Check if moving right
                else
                {
                    for (int i = move.FromCol + 1; i < move.ToCol; i++)
                    {
                        if (board[move.FromRow, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool CheckValidDiagonalMove(Move move, Piece[,] board)
        {
            // Check if the move is diagonal
            int rowDifference = Math.Abs(move.ToRow - move.FromRow);
            int colDifference = Math.Abs(move.ToCol - move.FromCol);

            if (rowDifference != colDifference)
            {
                return false;
            }

            // Check for pieces blocking the path
            if (move.ToCol > move.FromCol)
            {
                if (move.ToRow > move.FromRow)
                {
                    for (int i = move.FromCol + 1; i < move.ToCol; i++)
                    {
                        if (board[move.FromRow + (i - move.FromCol), i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
                // move.ToRow < move.FromRow
                else
                {
                    for (int i = move.FromCol + 1; i < move.ToCol; i++)
                    {
                        if (board[move.FromRow - (i - move.FromCol), i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }
            // move.ToCol < move.FromCol
            else
            {
                if (move.ToRow > move.FromRow)
                {
                    for (int i = move.ToCol + 1; i < move.FromCol; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"Checking: {move.ToRow - (i - move.FromRow)}, {i}");
                        if (board[move.ToRow - (i - move.ToCol), i].Player != Player.None)
                        {
                            System.Diagnostics.Debug.WriteLine($"Piece: {board[move.ToRow - (i - move.FromRow), i].PieceType.ToString()}" +
                                                               $" blocking path at: {move.ToRow - (i - move.FromRow)} , {i}");
                            return false;
                        }
                    }
                }
                // move.ToRow < move.FromRow
                else
                {
                    for (int i = move.ToCol + 1; i < move.FromCol; i++)
                    {
                        if (board[move.ToRow + (i - move.ToCol), i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Check if the move is out of bounds
            if (move.ToRow > BOARD_SIZE - 1 || move.ToRow < 0 || move.ToCol > BOARD_SIZE - 1 || move.ToCol < 0)
            {
                return false;
            }

            // Check if the piece belongs to the current player
            if (board[move.FromRow, move.FromCol].Player != currentPlayer)
            {
                return false;
            }

            // Check if the space to be moved to is already occupied by the current player's piece
            if (board[move.ToRow, move.ToCol].Player == currentPlayer)
            {
                return false;
            }

            // Make sure they are actually moving
            else if (move.FromCol == move.ToCol && move.ToRow == move.FromRow)
            {
                return false;
            }


            return CheckValidStraightMove(move, board) || CheckValidDiagonalMove(move, board);
        }

        public override void UpdatePiece() {}
    }
}
