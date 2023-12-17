using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class Empty : Pieces
    {
        // Constructor
        public Empty() : base(PieceType.Empty, Player.None) { }

        // Copy
        public override Pieces Clone()
        {
            return new Empty();
        }

        public override bool CheckValidMove(Move move, Pieces[,] board, Player currentPlayer, Move? lastMove)
        {
            return false;
        }

        public override void UpdatePiece() { }
    }
}
