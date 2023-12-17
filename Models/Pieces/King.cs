using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class King : Pieces
    {
        private const int BOARD_SIZE = 8;
        private bool hasMoved;

        // Constructor
        public King(Player player) : base(PieceType.King, player) {
            hasMoved = false;
        }

        // Copy
        public override Pieces Clone()
        {
            King king = new King(Player)
            {
                hasMoved = hasMoved
            };
            return king;
        }

        public override bool CheckValidMove(Move move, Pieces[,] board, Player currentPlayer, Move? lastMove)
        {
            // Check if the move is out of bounds
            if (move.ToRow > BOARD_SIZE - 1 || move.ToRow < 0 || move.ToCol > BOARD_SIZE - 1 || move.ToCol < 0)
            {
                return false;
            }

            // Check if the piece belongs to the current player
            if (board[move.FromCol, move.FromRow].Player != currentPlayer)
            {
                return false;
            }

            // Check if the space to be moved to is already occupied by the current player's piece
            if (board[move.ToCol, move.ToRow].Player == currentPlayer)
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
                int rookRow = board[move.FromCol, move.FromRow].Player == Player.White ? 0 : 7;

                // Check if the Rook is there
                if (board[rookCol, rookRow].PieceType == PieceType.Rook &&
                    board[rookCol, rookRow].Player == currentPlayer)
                {
                    // Check if the Rook has moved
                    if (((Rook)board[rookCol, rookRow]).HasMoved)
                    {
                        return false;
                    }

                    // Check if the path is clear
                    for (int i = Math.Min(move.FromCol, move.ToCol) + 1; i < Math.Max(move.FromCol, move.ToCol); i++)
                    {
                        if (board[i, move.FromRow].Player != Player.None)
                        {
                            return false;
                        }
                    }  
                }
            }

            // Allow the King to only move one space
            int rowDifference = Math.Abs(move.ToRow - move.FromRow);
            int colDifference = Math.Abs(move.ToCol - move.FromCol);

            if (rowDifference > 1 || colDifference > 1)
            {
                return false;
            }

            return true;
        }

        public override void UpdatePiece() {
            hasMoved = true;
        }
    }
}
