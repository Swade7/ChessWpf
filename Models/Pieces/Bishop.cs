using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class Bishop : Piece
    {
        private const int BOARD_SIZE = 8;

        // Constructor
        public Bishop(Player player) : base(PieceType.Bishop, player) { }

        // Copy
        public override Piece Clone()
        {
            return new Bishop(Player);
        }
       
        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
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
                        if (board[i, move.FromRow + (i - move.FromCol)].Player != Player.None)
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
                        if (board[i, move.FromRow - (i - move.FromCol)].Player != Player.None)
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
                        if (board[i, move.ToRow - (i - move.FromRow)].Player != Player.None)
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
                        if (board[i, move.ToRow + (i - move.ToCol)].Player != Player.None)
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
