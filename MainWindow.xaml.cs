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
        public MainWindow()
        {
            InitializeComponent();

            game = new ChessWpf.Models.Chess();


        }

        private void CreateGrid()
        {
            int rectSize = (int)boardCanvas.Width / game.BoardSize;
            // Create Rectangles for the grid
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = rectSize;
                    rect.Height = rectSize;
                    rect.Fill = (row + col) % 2 == 0 ? Brushes.White : Brushes.Black;
                    rect.Tag = new Point(row, col); 
                    // TODO: Add event handler for when the rectangle is clicked

                    Canvas.SetLeft(rect, col * rectSize);
                    Canvas.SetTop(rect, row * rectSize);
                    boardCanvas.Children.Add(rect);
                }
            }
        }
    }
}
