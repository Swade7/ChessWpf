using System;

namespace ChessWpf.Models.Pieces
{
    public class King : Piece
    {
        private bool hasMoved;

        public bool HasMoved
        {
            get { return hasMoved; }
            private set
            {
                    hasMoved = value;
            }
        }

        // Constructor
        public King(Player player, bool hasMoved = false) : base(PieceType.King, player) {
            HasMoved = hasMoved;
        }

        // Copy Constructor
        public override Piece Clone(Piece piece)
        {
            King oldKing = (King)piece;
            return new King(Player, oldKing.HasMoved){};
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
            {
                return false;
            }

            // Check if trying to castle
            if (!hasMoved && Math.Abs(move.FromCol - move.ToCol) == 2 &&
                move.FromRow == move.ToRow)
            {
                // Get the Rook's Column
                int rookCol = move.FromCol > move.ToCol ? 0 : 7;

                // Get the Rook's Row
                int rookRow = board[move.FromRow, move.FromCol].Player == Player.White ? 0 : 7;

                // Check if the Rook is there
                if (board[rookRow, rookCol].PieceType == PieceType.Rook &&
                    board[rookRow, rookCol].Player == currentPlayer)
                {
                    // Check if the King or Rook has moved
                    if (hasMoved || ((Rook)board[rookRow, rookCol]).HasMoved)
                    {
                        return false;
                    }                  

                    // Check if the path is clear
                    for (int i = Math.Min(move.FromCol, move.ToCol) + 1; i < Math.Max(move.FromCol, move.ToCol); i++)
                    {
                        if (board[move.FromRow, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                    return true;
                }               
            }
            // If not trying to castle
            else
            {
                // Allow the King to only move one space
                int rowDifference = Math.Abs(move.ToRow - move.FromRow);
                int colDifference = Math.Abs(move.ToCol - move.FromCol);

                if (rowDifference > 1 || colDifference > 1)
                {
                    return false;
                }
            }

            return true;
        }

        public override void UpdatePiece() {
            HasMoved = true;
        }
    }
}
