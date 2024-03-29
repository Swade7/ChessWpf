﻿using ChessWpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Chess game;

        Status gameStatus;

        public MainWindow()
        {
            InitializeComponent();

            game = new Chess();

            CreateBoard();
            DrawPieces();

            gameStatus = Status.Active;
            currentPlayerLabel.Content = $"{game.CurrentPlayer}'s turn";
        }

        private void CreateBoard()
        {
            // Clear the board
            boardCanvas.Children.Clear();

            // Draw squares
            int rectSize = (int)boardCanvas.Width / game.BoardSize;
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = rectSize;
                    rect.Height = rectSize;
                    rect.Fill = (row + col) % 2 == 0 ? Brushes.AntiqueWhite : Brushes.DarkOliveGreen;
                    rect.Tag = new Point(row, col);

                    rect.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                    Canvas.SetLeft(rect, col * rectSize);
                    Canvas.SetTop(rect, row * rectSize);
                    boardCanvas.Children.Add(rect);
                }
            }
        }

        private void RemovePiecesFromBoard()
        {
            List<UIElement> piecesToRemove = new List<UIElement>();
            foreach (UIElement element in boardCanvas.Children)
            {
                if (element is Image)
                {
                    piecesToRemove.Add(element);
                }
            }

            foreach (UIElement element in piecesToRemove)
            {
                boardCanvas.Children.Remove(element);
            }
        }
        private void DrawPieces()
        {
            // Remove existing pieces
            RemovePiecesFromBoard();

            // Draw pieces
            int rectSize = (int)boardCanvas.Width / game.BoardSize;
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    if (game.Board[row, col].Player != Player.None)
                    {
                        Image pieceImage = new Image();
                        pieceImage.Width = rectSize;
                        pieceImage.Height = rectSize;
                        pieceImage.Tag = new Point(row, col);
                        pieceImage.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                        // Show the image associated with the player and piece type
                        string fileName = game.Board[row, col].Player.ToString() + game.Board[row, col].GetType().Name.ToString() + ".png";
                        pieceImage.Source = new BitmapImage(new Uri("Images/" + fileName, UriKind.Relative));

                        Canvas.SetLeft(pieceImage, col * rectSize);
                        Canvas.SetTop(pieceImage, row * rectSize);
                        boardCanvas.Children.Add(pieceImage);
                    }
                }
            }
        }

        private void UpdateStatus()
        {
            gameStatus = game.UpdateStatus();

            if (gameStatus != Status.Active)
            {
                if (gameStatus == Status.WhiteWin || gameStatus == Status.BlackWin)
                {
                    MessageBox.Show("Checkmate! " + game.Opponent.ToString() + " wins!");
                }
                else if (gameStatus == Status.Stalemate)
                {
                    MessageBox.Show("Stalemate!");
                }
                else if (gameStatus == Status.GameOver)
                {
                    MessageBox.Show("Game over!");
                }

                // Unregister the event handler
                foreach (UIElement element in boardCanvas.Children)
                {
                    if (element is Rectangle)
                    {
                        (element as Rectangle).MouseLeftButtonDown -= boardCanvas_MouseLeftButtonDown;
                    }
                    else if (element is Image)
                    {
                        (element as Image).MouseLeftButtonDown -= boardCanvas_MouseLeftButtonDown;
                    }
                }
            }           
        }

        private Rectangle GetRectangleAt(int row, int col)
        {
            int elementIndex = row * game.BoardSize + col;
            return boardCanvas.Children.OfType<Rectangle>().ElementAt(elementIndex);
        }

        private void SelectPiece(Point location)
        {
            Rectangle rect = GetRectangleAt((int)location.X, (int)location.Y);
            HighlightRectangle(rect, ((SolidColorBrush)rect.Fill).Color, Colors.Gold);
        }

        private void HighlightPossibleMoves()
        {
            foreach (Move move in game.PossibleMovesForSelectedPiece)
            {
                Rectangle rect = GetRectangleAt(move.ToRow, move.ToCol);
                HighlightRectangle(rect, ((SolidColorBrush)rect.Fill).Color, Colors.Yellow);
            }
        }

        private void HighlightRectangle(Rectangle rect, Color originalColor, Color tintColor)
        {
            // Combine the original color and tint color with some opacity for the tint
            Color mixedColor = Color.Multiply(originalColor, (float)0.3) + Color.Multiply(tintColor, (float)0.7);
            SolidColorBrush mixedColorBrush = new SolidColorBrush(mixedColor);

            rect.Fill = mixedColorBrush;
        }


        private void DeselectAllPieces()
        {
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    Rectangle element = GetRectangleAt(row, col);
                    element.Fill = (row + col) % 2 == 0 ? Brushes.AntiqueWhite : Brushes.DarkOliveGreen;
                }
            }
        }

        private bool MakeMove(Move move)
        {
            if (game.MakeMove(move))
            {
                // Update the UI
                DrawPieces();

                game.SelectedLocation = new Point(-1, -1);
                UpdateStatus();
                DeselectAllPieces();
                currentPlayerLabel.Content = $"{game.CurrentPlayer}'s turn";

                return true;
            }

            return false;
        }

        private void boardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(boardCanvas);
            int row = (int)(mousePos.Y / (boardCanvas.Width / game.BoardSize));
            int col = (int)(mousePos.X / (boardCanvas.Width / game.BoardSize));

            // Either select a piece or move a piece
            if (game.SelectedLocation == new Point(-1, -1))
            {
                game.SelectedLocation = new Point(row, col);
                System.Diagnostics.Debug.WriteLine("Selected " + game.Board[row, col].Player + game.Board[row, col].GetType().Name + " at row " + row + " col " + col);

                // Highlight the selected piece
                SelectPiece(game.SelectedLocation);
                HighlightPossibleMoves();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Moving " + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].Player + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].GetType().Name + " from row " + game.SelectedLocation.X + " col " + game.SelectedLocation.Y + " to row " + row + " col " + col);
                Move move = new Move
                {
                    FromCol = (int)game.SelectedLocation.Y,
                    FromRow = (int)game.SelectedLocation.X,
                    ToCol = col,
                    ToRow = row
                };
                
                // Make the move if valid
                if (MakeMove(move))
                {
                    return;
                }
                else
                {
                    DeselectAllPieces();
                    if (game.Board[row, col].Player == game.CurrentPlayer)
                    {
                        game.SelectedLocation = new Point(row, col);
                        SelectPiece(game.SelectedLocation);
                        HighlightPossibleMoves();                    
                    }
                    else
                    {
                        game.SelectedLocation = new Point(-1, -1);
                    } 
                }             
            }
        }

        private void newGame_Btn_Click(object sender, RoutedEventArgs e)
        {
            game = new Chess();

            DeselectAllPieces();
            DrawPieces();

            gameStatus = Status.Active;
            currentPlayerLabel.Content = $"{game.CurrentPlayer}'s turn";
        }
    }
}
