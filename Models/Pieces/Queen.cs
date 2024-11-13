using System;

namespace ChessWpf.Models.Pieces
{
    public class Queen : Piece
    {
        // Constructor
        public Queen(Player player) : base(PieceType.Queen, player) { }

        // Copy Constructor
        public override Piece Clone(Piece piece)
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
            char direction = move.FromCol == move.ToCol ? 'v' : 'h';

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
                        if (board[move.ToRow - (i - move.ToCol), i].Player != Player.None)
                        {
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
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
            {
                return false;
            }

            return CheckValidStraightMove(move, board) || CheckValidDiagonalMove(move, board);
        }

        public override void UpdatePiece() {}
    }
}