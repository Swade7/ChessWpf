namespace ChessWpf.Models.Pieces
{
    public class Rook : Piece
    {
        private bool hasMoved = false;

        // Properties
        public bool HasMoved
        {
            get { return hasMoved; }
            private set
            {
                if (hasMoved != true)
                    hasMoved = value;
            }
        }

        // Constructor
        public Rook(Player player) : base(PieceType.Rook, player) { 
            hasMoved = false;
        }

        // Copy Constructor
        public override Piece Clone(Piece piece)
        {
            Rook oldRook = (Rook)piece;
            return new Rook(Player)
            {
                hasMoved = oldRook.HasMoved
            };
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
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
                        if (board[i, move.FromCol].Player != Player.None)
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
                        if (board[i, move.FromCol].Player != Player.None)
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
                        if (board[move.FromRow, i].Player != Player.None)
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
                        if (board[move.FromRow, i].Player != Player.None)
                        {
                            return false;
                        }
                    }
                }
            }
            // Not moving in a straight line
            else
            {
                return false;
            }

            return true;
        }

        public override void UpdatePiece() {
            HasMoved = true;
        }

    }
}
