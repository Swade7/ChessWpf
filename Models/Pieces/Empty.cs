﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWpf.Models.Pieces
{
    public class Empty : Piece
    {
        private static readonly Empty instance = new Empty();

        // Constructor
        public Empty() : base(PieceType.Empty, Player.None) { }

        public static Empty Instance
        {
            get { return instance; }
        }

        // Copy
        public override Piece Clone(Piece piece)
        {
            return new Empty();
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            return false;
        }

        public override void UpdatePiece() { }
    }
}
