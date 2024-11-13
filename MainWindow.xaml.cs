using ChessWpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

                    // Add click handlers
                    rect.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                    // Set the position of the rectangle
                    Canvas.SetLeft(rect, col * rectSize);
                    Canvas.SetTop(rect, row * rectSize);

                    // Add the rectangle to the canvas
                    boardCanvas.Children.Add(rect);
                }
            }
        }

        private void RemovePiecesFromBoard()
        {
            // Remove all images (pieces) from the board canvas
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
            // Remove existing pieces from the board
            RemovePiecesFromBoard();

            // Draw pieces to reflect the current state of the game
            int rectSize = (int)boardCanvas.Width / game.BoardSize;
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    if (game.Board[row, col].Player != Player.None)
                    {
                        // Create a new image for the piece
                        Image pieceImage = new Image();
                        pieceImage.Width = rectSize;
                        pieceImage.Height = rectSize;
                        pieceImage.Tag = new Point(row, col);

                        // Add click handlers
                        pieceImage.MouseLeftButtonDown += boardCanvas_MouseLeftButtonDown;

                        // Show the image associated with the player and piece type
                        string fileName = game.Board[row, col].Player.ToString() + game.Board[row, col].GetType().Name.ToString() + ".png";
                        pieceImage.Source = new BitmapImage(new Uri("Images/" + fileName, UriKind.Relative));

                        // Set the position of the image
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

            // If the game is over, display a message box showing the results
            if (gameStatus != Status.Active)
            {
                /*
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
                */

                switch (gameStatus)
                {
                    case Status.WhiteWin:
                        currentPlayerLabel.Content = "White wins!";
                        break;
                    case Status.BlackWin:
                        currentPlayerLabel.Content = "Black wins!";
                        break;
                    case Status.Stalemate:
                        currentPlayerLabel.Content = "Stalemate!";
                        break;
                    case Status.GameOver:
                        currentPlayerLabel.Content = "Game over!";
                        break;
                }

                // Unregister the event handlers for the rectangles and images
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

        // get a Piece at a specific row and column
        private Rectangle GetRectangleAt(int row, int col)
        {
            //int elementIndex = row * game.BoardSize + col;
            return boardCanvas.Children.OfType<Rectangle>().ElementAt(row * game.BoardSize + col);
        }

        // Select a Piece if it belongs to the current player
        private void SelectPiece(Point location)
        {
            // Check if the location is occupied by a piece belonging to the current player
            if (game.Board[(int)location.X, (int)location.Y].Player == game.CurrentPlayer)
            {
                // Highlight the selected piece
                Rectangle rect = GetRectangleAt((int)location.X, (int)location.Y);
                HighlightRectangle(rect, ((SolidColorBrush)rect.Fill).Color, Colors.Gold);

                // Set the selected location to the clicked location
                game.SelectedLocation = location;
            }         
        }

        // Show the possible moves for the selected piece
        private void HighlightPossibleMoves()
        {
            if (game.SelectedLocation != new Point(-1, -1))
            {
                foreach (Move move in game.PossibleMovesForSelectedPiece)
                {
                    Rectangle rect = GetRectangleAt(move.ToRow, move.ToCol);
                    HighlightRectangle(rect, ((SolidColorBrush)rect.Fill).Color, Colors.Yellow);
                }
            }           
        }

        // Highlight a rectangle with a tint color
        private void HighlightRectangle(Rectangle rect, Color originalColor, Color tintColor)
        {
            // Combine the original color and tint color with some opacity for the tint
            Color mixedColor = Color.Multiply(originalColor, (float)0.3) + Color.Multiply(tintColor, (float)0.7);
            SolidColorBrush mixedColorBrush = new SolidColorBrush(mixedColor);

            rect.Fill = mixedColorBrush;
        }

        // Set the color of all rectangles to the original color
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

        // Make a move on the board if allowed by the game logic
        private bool MakeMove(Move move)
        {
            if (game.MakeMove(move))
            {
                // Update the UI
                DrawPieces();

                game.ResetSelectedLocation();
                DeselectAllPieces();
                UpdateStatus();
                currentPlayerLabel.Content = $"{game.CurrentPlayer}'s turn";

                return true;
            }

            game.ResetSelectedLocation();
            return false;
        }

        private void boardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the row and column of the clicked rectangle
            Point mousePos = e.GetPosition(boardCanvas);
            int row = (int)(mousePos.Y / (boardCanvas.Width / game.BoardSize));
            int col = (int)(mousePos.X / (boardCanvas.Width / game.BoardSize));

            // Either select a piece or move a piece depending on if a piece is already selected
            if (game.SelectedLocation == new Point(-1, -1))
            {
                // Select and highlight the piece
                SelectPiece(new Point(row, col));
                HighlightPossibleMoves();
            }
            // Move the selected piece if the clicked location is a valid move
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    "Moving " + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].Player 
                    + game.Board[(int)game.SelectedLocation.X, (int)game.SelectedLocation.Y].GetType().Name
                    + " from row " + game.SelectedLocation.X + " col " + game.SelectedLocation.Y
                    + " to row " + row + " col " + col
                );

                // Make a new move object with the selected piece and the clicked location
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
                    // If the move is invalid, deselect the current piece
                    // If the clicked location is occupied by a piece belonging to the current player, select it
                    DeselectAllPieces();
                    if (game.Board[row, col].Player == game.CurrentPlayer)
                    {
                        game.SelectedLocation = new Point(row, col);
                        SelectPiece(game.SelectedLocation);
                        HighlightPossibleMoves();                    
                    }
                    // If the clicked location is not of a piece belonging to the current player, set the selected location to -1, -1
                    else
                    {
                        game.ResetSelectedLocation();
                    } 
                }             
            }
        }

        private void newGame_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmNewGame())
            {
                StartNewGame();
            }
        }

        private void StartNewGame()
        {
            // Reset the game
            game = new Chess();

            DeselectAllPieces();
            DrawPieces();

            gameStatus = Status.Active;
            currentPlayerLabel.Content = $"{game.CurrentPlayer}'s turn";
        }

        // Show a dialog box to confirm starting a new game
        private bool ConfirmNewGame()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to start a new game?", "New Game", MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }

        private void quitGame_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmQuitGame())
            {
                QuitGame();
            }
        }

        private bool ConfirmQuitGame()
        {
            // Show a dialog box to confirm quitting the game
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit the game?", "Quit Game", MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }

        private void QuitGame()
        {
            Application.Current.Shutdown();
        }
    }
}
