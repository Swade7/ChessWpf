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

            // Knight specific checks
            int rowDifference = Math.Abs(move.ToRow - move.FromRow);
            int colDifference = Math.Abs(move.ToCol - move.FromCol);

            return rowDifference == 2 && colDifference == 1 ||
                rowDifference == 1 && colDifference == 2;
        }

        public override void UpdatePiece() { }
    }
}
