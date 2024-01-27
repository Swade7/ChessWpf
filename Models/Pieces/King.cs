using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class King : Piece
    {
        private const int BOARD_SIZE = 8;
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

        // Copy
        public override Piece Clone(Piece piece)
        {
            King oldKing = (King)piece;
            return new King(Player, oldKing.HasMoved)
            {
                //hasMoved = oldKing.HasMoved
            };
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
