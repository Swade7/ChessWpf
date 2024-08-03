namespace ChessWpf.Models.Pieces
{ 
    public abstract class Piece
    {
        public const int BOARD_SIZE = 8;
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
            set
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
        public abstract Piece Clone(Piece piece);

    }
}
