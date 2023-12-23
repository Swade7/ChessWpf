using ChessWpf.Models;
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
            System.Diagnostics.Debug.WriteLine(game.Board[3, 3].GetType());
        }


        private void UpdateStatus()
        {
            gameStatus = game.UpdateStatus();

            if (gameStatus != Status.Active)
            {
                if (gameStatus == Status.WhiteWin)
                {
                    MessageBox.Show("Checkmate! " + game.CurrentPlayer.ToString() + " wins!");
                }
                else if (gameStatus == Status.BlackWin)
                {
                    MessageBox.Show("Checkmate! " + game.CurrentPlayer.ToString() + " wins!");
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
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Moving " + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].Player + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].GetType().Name + " from row " + game.SelectedLocation.X + " col " + game.SelectedLocation.Y + " to row " + row + " col " + col);
                Move move = new Move 
                {
                    FromCol = (int)game.SelectedLocation.Y,
                    FromRow = (int)game.SelectedLocation.X,
                    ToCol = col,
                    ToRow = row };
                bool moveSuccess = game.MakeMove(move);
                if (moveSuccess)
                {
                    System.Diagnostics.Debug.WriteLine("Move successful");
                    // Update the UI
                    DrawPieces();

                    game.SelectedLocation = new Point(-1, -1);

                }
                else
                {
                    if (game.Board[row, col].Player == game.CurrentPlayer)
                    {
                        game.SelectedLocation = new Point(row, col);
                    }
                    else
                    {
                        game.SelectedLocation = new Point(-1, -1);
                    } 
                }

                UpdateStatus();

            }
        }
    }
}
