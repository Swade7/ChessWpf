﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ChessWpf.Models.Pieces
{
    public class Pawn : Piece
    {
        private const int BOARD_SIZE = 8;
        private bool hasMoved;

        // Constructor
        public Pawn(Player player) : base(PieceType.Pawn, player) {
            hasMoved = false;
        }

        // Copy
        public override Piece Clone()
        {

            return new Pawn(Player)
            {
                hasMoved = hasMoved
            };
        }

        public override bool CheckValidMove(Move move, Piece[,] board, Player currentPlayer, Move? lastMove)
        {
            // Check if the move is out of bounds
            if (move.ToRow > BOARD_SIZE - 1 || move.ToRow < 0 || move.ToCol > BOARD_SIZE - 1 || move.ToCol < 0)
            {
                System.Diagnostics.Debug.WriteLine("Out of bounds");
                return false;
            }

            // Check if the piece belongs to the current player
            if (board[move.FromRow, move.FromCol].Player != currentPlayer)
            {
                System.Diagnostics.Debug.WriteLine("Not current player");
                return false;
            }

            // Check if the space to be moved to is already occupied by the current player's piece
            if (board[move.ToRow, move.ToCol].Player == currentPlayer)
            {   
                System.Diagnostics.Debug.WriteLine("Already occupied");
                return false;
            }

            // Make sure they are actually moving
            else if (move.FromCol == move.ToCol && move.ToRow == move.FromRow)
            {
                System.Diagnostics.Debug.WriteLine("Not moving");
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
            if (Math.Abs(rowDifference) > maxMoveDistance)
            {
                System.Diagnostics.Debug.WriteLine("Pawn Moving Too far");
                return false;
            }

            // Prevent the pawn from moving sideways or backwards
            if (move.FromRow == move.ToRow
                || Math.Abs(colDifference) > 1
                || rowDifference * dir < 0)
            {
                System.Diagnostics.Debug.WriteLine("Pawn Moving sideways or backwards");
                return false;
            }

            // Prevent capturing and moving two places
            else if (Math.Abs(move.FromRow - move.ToRow) != 1 && move.FromCol != move.ToCol)
            {
                System.Diagnostics.Debug.WriteLine("Pawn Capturing and moving two places");
                return false;
            }

            // Only allow horizontal movement if the pawn is capturing a piece
            if (move.ToCol != move.FromCol)
            {
                // Check for en passant capturing
                if (board[move.ToCol, move.ToRow].Player != opponent)
                {
                    if (lastMove == null)
                    {
                        System.Diagnostics.Debug.WriteLine("No last move");
                        return false;
                    }
                    else if (lastMove.Value.ToCol != move.ToCol
                        || lastMove.Value.ToRow != move.ToRow - dir
                        || lastMove.Value.ToCol != move.ToCol
                        || lastMove.Value.ToRow != move.FromRow
                        || Math.Abs(lastMove.Value.FromRow - lastMove.Value.ToRow) != 2
                        || board[lastMove.Value.ToCol, lastMove.Value.ToRow].PieceType != PieceType.Pawn)
                    {
                        System.Diagnostics.Debug.WriteLine("Not en passant");
                        return false;
                    }
                }
                else if (board[move.ToCol, move.ToRow].Player != opponent)
                {
                    System.Diagnostics.Debug.WriteLine("Attempting to capture piece with pawn that isn't opponent's");
                    return false;
                }
            }
            else if (board[move.ToCol, move.ToRow].Player != Player.None)
            {
                System.Diagnostics.Debug.WriteLine("Attempting to move to a space that is already occupied");
                return false;
            }
            
            System.Diagnostics.Debug.WriteLine("Valid move");
            return true;
        }

        public override void UpdatePiece()
        {
            hasMoved = true;
        }
    }
}
