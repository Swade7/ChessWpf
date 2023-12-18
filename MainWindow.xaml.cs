using ChessWpf.Models;
using System;
using System.Collections.Generic;
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
        private ChessWpf.Models.Chess game;
        int rectSize = 0;
        public MainWindow()
        {
            InitializeComponent();

            game = new ChessWpf.Models.Chess();
            //CreateGrid();
            CreateBoard();
            DrawBoard();

        }

        private void CreateGrid()
        {
            rectSize = (int)boardCanvas.Width / game.BoardSize;
            // Create Rectangles for the grid
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = rectSize;
                    rect.Height = rectSize;
                    rect.Fill = (row + col) % 2 == 0 ? Brushes.AntiqueWhite : Brushes.DarkOliveGreen;
                    rect.Tag = new Point(row, col); 
                    // TODO: Add event handler for when the rectangle is clicked
                    rect.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;
                    Canvas.SetLeft(rect, col * rectSize);
                    Canvas.SetTop(rect, row * rectSize);
                    boardCanvas.Children.Add(rect);
                }
            }
        }

        // Show the pieces on the board
        private void CreateBoard()
        {
            // Clear the board
            boardCanvas.Children.Clear();
            

            // Loop through the board and draw the pieces at the center of each square using the images from the Images folder

            int rectSize = (int)boardCanvas.Width / game.BoardSize;

            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = rectSize;
                    rect.Height = rectSize;
                    rect.Fill = (row + col) % 2 == 0 ? Brushes.AntiqueWhite : Brushes.DarkOliveGreen;
                    rect.Tag = new Point(row, col); // Store the row and col in the Tag property so we can get it later

                    rect.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                    Canvas.SetLeft(rect, col * rectSize);
                    Canvas.SetTop(rect, row * rectSize);
                    boardCanvas.Children.Add(rect);

                    
                }
            }


        }

        private void DrawBoard()
        {
            CreateBoard();
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    if (game.Board[row, col].Player != Player.None)
                    {
                        if (game.Board[row, col].Player == Player.Black)
                        {
                            System.Diagnostics.Debug.WriteLine("Black piece at row " + row + " col " + col);
                        }
                        else if (game.Board[row, col].Player == Player.White)
                        {
                            System.Diagnostics.Debug.WriteLine("White piece at row " + row + " col " + col);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No piece at row " + row + " col " + col);
                        }
                        Image pieceImage = new Image();
                        pieceImage.Width = rectSize;
                        pieceImage.Height = rectSize;
                        pieceImage.Tag = new Point(row, col); // Store the row and col in the Tag property so we can get it later
                                                              //pieceImage.MouseLeftButtonDown += PieceImage_MouseLeftButtonDown;
                        pieceImage.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                        string fileName = game.Board[row, col].Player.ToString() + game.Board[row, col].GetType().Name.ToString() + ".png";
                        fileName = "WhitePawn.png";
                        pieceImage.Source = new BitmapImage(new Uri("Images/" + fileName, UriKind.Relative));

                        Canvas.SetLeft(pieceImage, col * rectSize);
                        Canvas.SetTop(pieceImage, row * rectSize);
                        boardCanvas.Children.Add(pieceImage);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No piece at row " + row + " col " + col);
                    }
                }
            }
            // Draw the pieces
           
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
                    // TODO: Update the UI
                    DrawBoard();
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
            }
        }

        private void DisplaySelectedPiece()
        {
            // Show the selected piece with a blue tint behind it on the square


        }
    }
}
