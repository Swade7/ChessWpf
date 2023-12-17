using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ChessWpf.Models.Pieces
{
    public class Rook : Pieces
    {
        public const int BOARD_SIZE = 8;

        private bool hasMoved;

        public bool HasMoved
        {
            get { return HasMoved; }
            private set
            {
                if (hasMoved == true)
                    hasMoved = value;
            }
        }

        // Constructor
        public Rook(Player player) : base(PieceType.Rook, player) { }

        // Copy
        public override Pieces Clone()
        {
            return new Rook(Player);
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

            // Rook specific checks
            if (move.FromCol == move.ToCol)
            {
                // Moving up
                if (move.ToRow > move.FromRow)
                {
                    for (int i = move.FromRow + 1; i < move.ToRow; i++)
                    {
                        if (board[move.FromCol, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
                // Moving down
                else
                {
                    for (int i = move.FromRow - 1; i > move.ToRow; i--)
                    {
                        if (board[move.FromCol, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }
            else if (move.FromRow == move.ToRow)
            {
                // Moving right
                if (move.ToCol > move.FromCol)
                {
                    for (int i = move.FromCol + 1; i < move.ToCol; i++)
                    {
                        if (board[i, move.FromRow].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
                // Moving left
                else
                {
                    for (int i = move.FromCol - 1; i > move.ToCol; i--)
                    {
                        if (board[i, move.FromRow].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override void UpdatePiece() {
            HasMoved = true;
        }

    }
}
