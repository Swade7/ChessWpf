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

        public abstract bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove);
        public abstract void UpdatePiece();

        // Copy method
        public abstract Piece Clone(Piece piece);

    }
}
