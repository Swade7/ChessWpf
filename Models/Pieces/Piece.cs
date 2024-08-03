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

        // Generic move validations
        private bool CheckMoveInBounds(Move move)
        {
            return !(move.ToRow > BOARD_SIZE - 1 || move.ToRow < 0 || move.ToCol > BOARD_SIZE - 1 || move.ToCol < 0);
        }

        private bool CheckPieceBelongsToPlayer(Move move, Piece[,] board, Player currentPlayer)
        {
            return board[move.FromRow, move.FromCol].Player == currentPlayer;
        }

        private bool CheckToSpaceUnoccupiedByCurrentPlayer(Move move, Piece[,] board, Player currentPlayer)
        {
            return board[move.ToRow, move.ToCol].Player != currentPlayer;
        }

        private bool CheckIfMoving(Move move)
        {
            return move.FromCol != move.ToCol || move.ToRow != move.FromRow;
        }
        public bool CheckGenericMoveValidations(Move move, Piece[,] board, Player currentPlayer)
        {
            return
                CheckMoveInBounds(move)
                && CheckPieceBelongsToPlayer(move, board, currentPlayer)
                && CheckToSpaceUnoccupiedByCurrentPlayer(move, board, currentPlayer)
                && CheckIfMoving(move);
        }
    }
}
