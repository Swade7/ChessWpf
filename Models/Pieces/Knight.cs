using System;

namespace ChessWpf.Models.Pieces
{
    public class Knight : Piece
    {
        // Constructor
        public Knight(Player player) : base(PieceType.Knight, player) { }

        // Copy Constructor
        public override Piece Clone(Piece piece)
        {
            return new Knight(Player);
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
            {
                return false;
            }

            // Knight specific checks
            int rowDifference = Math.Abs(move.ToRow - move.FromRow);
            int colDifference = Math.Abs(move.ToCol - move.FromCol);

            return rowDifference == 2 && colDifference == 1 ||
                rowDifference == 1 && colDifference == 2;
        }

        public override void UpdatePiece() { }
    }
}
