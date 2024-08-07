﻿using System;

namespace ChessWpf.Models.Pieces
{
    public class Pawn : Piece
    {
        private bool hasMoved;

        // Constructor
        public Pawn(Player player) : base(PieceType.Pawn, player) {
            hasMoved = false;
        }

        // Copy Constructor
        public override Piece Clone(Piece piece)
        {
            Pawn oldPawn = (Pawn)piece;
            return new Pawn(Player)
            {
                hasMoved = oldPawn.hasMoved
            };
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Generic move validations
            if (!CheckGenericMoveValidations(move, board, currentPlayer))
            {
                return false;
            }

            // Pawn specific checks
            int dir = Player == Player.White ? 1 : -1;
            int startRow = Player == Player.White ? 1 : 6;
            Player opponent = Player == Player.White ? Player.Black : Player.White;
            int rowDifference = move.ToRow - move.FromRow;
            int colDifference = move.ToCol - move.FromCol;
            int maxMoveDistance = hasMoved ? 1 : 2;

            // Prevent the pawn from moving more than the max distance
            if (Math.Abs(rowDifference) > maxMoveDistance || (Math.Abs(rowDifference) != 1 && move.FromRow != startRow))
            {
                return false;
            }

            // Prevent the pawn from moving sideways or backwards
            if (move.FromRow == move.ToRow
                || Math.Abs(colDifference) > 1
                || rowDifference * dir < 0)
            {
                return false;
            }

            // Prevent capturing and moving two places
            else if (Math.Abs(move.FromRow - move.ToRow) != 1 && move.FromCol != move.ToCol)
            {
                return false;
            }

            // Only allow horizontal movement if the pawn is capturing a piece
            if (move.ToCol != move.FromCol)
            {
                // Check for en passant capturing
                if (board[move.ToRow, move.ToCol].Player != opponent)
                {
                    if (lastMove == null)
                    {
                        return false;
                    }
                    else if (lastMove.Value.ToRow != move.FromRow
                        || Math.Abs(move.FromCol - lastMove.Value.ToCol) != 1
                        || Math.Abs(lastMove.Value.FromRow - lastMove.Value.ToRow) != 2
                        || board[lastMove.Value.ToRow, lastMove.Value.ToCol].PieceType != PieceType.Pawn
                        || lastMove.Value.ToCol != move.ToCol)
                    {
                        return false;
                    }
                }
                else if (board[move.ToRow, move.ToCol].Player != opponent)
                {
                    return false;
                }
            }
            else if (board[move.ToRow, move.ToCol].Player != Player.None)
            {
                return false;
            }
            
            return true;
        }

        public override void UpdatePiece()
        {
            hasMoved = true;
        }
    }
}
