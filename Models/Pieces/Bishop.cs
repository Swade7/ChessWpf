using System;

namespace ChessWpf.Models.Pieces
{
    public class Bishop : Piece
    {
        // Constructor
        public Bishop(Player player) : base(PieceType.Bishop, player) { }

        // Copy Constructor
        public override Piece Clone(Piece piece)
        {
            return new Bishop(Player);
        }
       
        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
            {
                return false;
            }

            // Bishop specific checks
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
        public override void UpdatePiece() { }
    }
}
