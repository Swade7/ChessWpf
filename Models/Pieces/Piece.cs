using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChessWpf.Models.Pieces
{ 

    public abstract class Piece
    {
        private PieceType pieceType;
        private Player player;
        // Constructor
        public Piece(PieceType pieceType, Player player)
        {
            PieceType = pieceType;
            Player = player;
        }

        // Getters
        public PieceType PieceType
        {
            get
            {
                return pieceType;
            }
            private set
            {
                pieceType = value;
            }
        }
        public Player Player
        {
            get
            {
                return player;
            }
            private set
            {
                player = value;
            }
        }

        // Static methods for adding and removing pieces
        public static void AddToCapturedPieces(PieceType capturedPiece, Player player)
        {
            // Implement your logic here
        }

        public static void RemoveFromPieces(PieceType piece, Player player)
        {
            // Implement your logic here
        }

        public abstract bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove);
        public abstract void UpdatePiece();

        // Copy method
        public abstract Piece Clone();

    }
}
