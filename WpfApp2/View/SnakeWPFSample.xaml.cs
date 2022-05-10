using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace WpfApp2.View
{
    /// <summary>
    /// SnakeWPFSample.xaml 的交互逻辑
    /// </summary>
    public partial class SnakeWPFSample : Window
    {
        const int SnakeSquareSize = 20;
        const int SnakeStartLength = 3;
        const int SnakeStartSpeed = 400;
        const int SnakeSpeedThreshold = 100;

        private SolidColorBrush snakeBodyBrush = Brushes.Green;
        private SolidColorBrush snakeHeadBrush = Brushes.YellowGreen;
        private List<SnakePart> snakeParts = new List<SnakePart>();
        /// <summary>
        /// snake move
        /// </summary>
        private DispatcherTimer gameTickTimer = new DispatcherTimer();

        private Random rnd = new Random();
        private UIElement snakeFood = null;
        private SolidColorBrush foodBrush = Brushes.Red;

        public enum SnakeDirection { Left,Right,Up,Down};
        private SnakeDirection snakeDirection = SnakeDirection.Right;
        private int snakeLength;
        private int currentScore;

        public SnakeWPFSample()
        {
            InitializeComponent();
            gameTickTimer.Tick += GameTickTimer_Tick;
            LoadHighScoreList();
        }

        private void StartNewGame()
        {
            bdrWelcomeMessage.Visibility = Visibility.Collapsed;
            bdrHighscorelist.Visibility = Visibility.Collapsed;
            bdrEndofGame.Visibility = Visibility.Collapsed;


            //remove potential dead snake parts and leftover food...
            foreach (SnakePart snakeBodyPart in snakeParts)
            {
                if (snakeBodyPart.UiElement != null)
                    GameArea.Children.Remove(snakeBodyPart.UiElement);
            }
            snakeParts.Clear();
            if(snakeFood != null)
                GameArea.Children.Remove(snakeFood);

            //Reset stuff
            currentScore = 0;
            snakeLength = SnakeStartLength;
            snakeDirection = SnakeDirection.Right;
            snakeParts.Add(new SnakePart() { Position = new Point(SnakeSquareSize * 5, SnakeSquareSize * 5) });
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);

            //Draw the snake
            DrawSnake();
            DrawSnakeFood();

            //Update Status
            UpdateGameStatus();

            //Go !
            gameTickTimer.IsEnabled = true;
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawGameArea();
            StartNewGame();
        }

        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool nextIsOdd = false;

            while(doneDrawingBackground == false)
            {
                Rectangle rect = new Rectangle
                {
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
                    Fill = nextIsOdd ? Brushes.White : Brushes.Black
                };
                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += SnakeSquareSize;
                if(nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += SnakeSquareSize;
                    rowCounter++;
                    nextIsOdd = (rowCounter % 2 != 0);
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }

        private void DrawSnake()
        {
            foreach (SnakePart snakePart in snakeParts)
            {
                if(snakePart.UiElement == null)
                {
                    snakePart.UiElement = new Rectangle() 
                    {
                        Width = SnakeSquareSize,
                        Height = SnakeSquareSize,
                        Fill = snakePart.IsHead? snakeHeadBrush:snakeBodyBrush
                    };
                    GameArea.Children.Add(snakePart.UiElement);
                    Canvas.SetTop(snakePart.UiElement, snakePart.Position.Y);
                    Canvas.SetLeft(snakePart.UiElement, snakePart.Position.X);
                }
            }
        }

        private void MoveSnake()
        {
            //remove the last part of the snake,in preparation of the new part added below
            while(snakeParts.Count >= snakeLength)
            {
                GameArea.Children.Remove(snakeParts[0].UiElement);
                snakeParts.RemoveAt(0);
            }
            /// Next up,we'll add a new element to the snake,which will be the (new) head
            /// Therefore, we make all existing paer as non-head(body) elements and then
            /// we make sure that they use th body brush
            foreach (SnakePart snakePart in snakeParts)
            {
                (snakePart.UiElement as Rectangle).Fill = snakeBodyBrush;
                snakePart.IsHead = false;
            }

            //Determine in which direction to expand the snake, based on the current direction
            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];
            double nextX = snakeHead.Position.X;
            double nextY = snakeHead.Position.Y;
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    nextX -= SnakeSquareSize;
                    break;
                case SnakeDirection.Right:
                    nextX += SnakeSquareSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= SnakeSquareSize;
                    break;
                case SnakeDirection.Down:
                    nextY += SnakeSquareSize;
                    break;
            }

            //Now add the new head part to our list of snake parts
            snakeParts.Add(new SnakePart()
            {
               Position = new Point(nextX,nextY),
               IsHead = true
            }); ;
            //...and then have it drawn.
            DrawSnake();

            DoCollisionCheck();
        }

        private Point GetNextFoodPosition()
        {
            int maxX = (int)(GameArea.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameArea.ActualHeight / SnakeSquareSize);
            int foodX = rnd.Next(0, maxX) * SnakeSquareSize;
            int foodY = rnd.Next(0, maxY) * SnakeSquareSize;

            foreach (SnakePart snakePart in snakeParts)
            {
                if ((snakePart.Position.X == foodX) && snakePart.Position.Y == foodY)
                    return GetNextFoodPosition();
            }

            return new Point(foodX, foodY);
        }

        private void DrawSnakeFood()
        {
            Point foodPosition = GetNextFoodPosition();
            snakeFood = new Ellipse()
            {
                Width = SnakeSquareSize,
                Height= SnakeSquareSize,
                Fill = foodBrush
            };
            GameArea.Children.Add(snakeFood);
            Canvas.SetTop(snakeFood, foodPosition.Y);
            Canvas.SetLeft(snakeFood, foodPosition.X);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SnakeDirection originalSnakeDirection = snakeDirection;
            switch (e.Key)
            {
                case Key.Up:
                    if (snakeDirection != SnakeDirection.Down)
                        snakeDirection = SnakeDirection.Up;
                    break;
                case Key.Down:
                    if (snakeDirection != SnakeDirection.Up)
                        snakeDirection = SnakeDirection.Down;
                    break;
                case Key.Right:
                    if (snakeDirection != SnakeDirection.Left)
                        snakeDirection = SnakeDirection.Right;
                    break;
                case Key.Left:
                    if (snakeDirection != SnakeDirection.Right)
                        snakeDirection = SnakeDirection.Left;
                    break;
                case Key.Space:
                    StartNewGame();
                    break;
            }
            if (snakeDirection != originalSnakeDirection)
                MoveSnake();
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void DoCollisionCheck() 
        {
            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];

            if(snakeHead.Position.X == Canvas.GetLeft(snakeFood) && snakeHead.Position.Y == Canvas.GetTop(snakeFood))
            {
                EatSnakeFood();
                return;
            }

            if(snakeHead.Position.Y < 0 || snakeHead.Position.Y >= GameArea.ActualHeight || snakeHead.Position.X <0 || snakeHead.Position.X >= GameArea.ActualWidth)
            {
                EndGame();
            }

            foreach (SnakePart snakePart in snakeParts.Take(snakeParts.Count - 1))
            {
                if (snakeHead.Position.X == snakePart.Position.X && snakeHead.Position.Y == snakePart.Position.Y)
                    EndGame();
            }
        }

        private void EndGame()
        {
            bool isNewHighScore = false;
            if(currentScore > 0)
            {
                int lowestHighscore = this.HighscoreList.Count > 0 ? this.HighscoreList.Min(x => x.Score) : 0;
                if(currentScore > lowestHighscore || this.HighscoreList.Count < MaxHighscoreListEntryCount)
                {
                    bdrNewHighScore.Visibility = Visibility.Visible;
                    txtPlayerName.Focus();
                    isNewHighScore = true;
                }
            }
            if (!isNewHighScore)
            {
                tbFinalScore.Text = currentScore.ToString();
                bdrEndofGame.Visibility = Visibility.Visible;
            }
            gameTickTimer.IsEnabled = false;
            //MessageBox.Show("Ooooops,You Died!\n\nTo Start a new Game,just press the Space bar...","SnakeWPF");
        }

        private void EatSnakeFood()
        {
            snakeLength++;
            currentScore++;
            int timeInterval = Math.Max(SnakeSpeedThreshold, (int)gameTickTimer.Interval.TotalMilliseconds - (currentScore * 2));
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(timeInterval);
            GameArea.Children.Remove(snakeFood);
            DrawSnakeFood();
            UpdateGameStatus();
        }

        private void UpdateGameStatus()
        {
            //this.Title = "SnakeWPF - Score: " + currentScore + " - Game Speed: " + gameTickTimer.Interval.TotalMilliseconds;
            this.tbStatusScore.Text = currentScore.ToString();
            tbStatusSpeed.Text = gameTickTimer.Interval.TotalMilliseconds.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnShowHighScoreList_Click(object sender, RoutedEventArgs e)
        {
            bdrWelcomeMessage.Visibility = Visibility.Collapsed;
            bdrHighscorelist.Visibility = Visibility.Visible;
        }

        public ObservableCollection<SnakeHighscore> HighscoreList { get; set; } = new ObservableCollection<SnakeHighscore>();


        private void btnAddToHighScoreList_Click(object sender, RoutedEventArgs e)
        {
            //排名
            int newIndex = 0;
            //where should the new entry be inserted?
            if(this.HighscoreList.Count > 0 && currentScore < this.HighscoreList.Max(x => x.Score))
            {
                SnakeHighscore justAbove = this.HighscoreList.OrderByDescending(x => x.Score).First(x => x.Score >= currentScore);
                if(justAbove != null)
                {
                    newIndex = this.HighscoreList.IndexOf(justAbove) + 1;
                }
            }
            //Create & insert the new entry
            this.HighscoreList.Insert(newIndex, new SnakeHighscore()
            {
                PlayerName = txtPlayerName.Text,
                Score = currentScore
            });
            //make sure that rhe amount of entries does not exceed the maximum
            while(this.HighscoreList.Count > MaxHighscoreListEntryCount)
            {
                this.HighscoreList.RemoveAt(MaxHighscoreListEntryCount);
            }

            SaveHighscoreList();

            bdrNewHighScore.Visibility = Visibility.Collapsed;
            bdrHighscorelist.Visibility = Visibility.Visible;
        }

        private int MaxHighscoreListEntryCount =5;


        private void LoadHighScoreList()
        {
            if (File.Exists("snake_highscorelist.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<SnakeHighscore>));
                using (Stream reader = new FileStream("snake_highscorelist.xml", FileMode.Open))
                {
                    List<SnakeHighscore> tempList = (List<SnakeHighscore>)serializer.Deserialize(reader);
                    this.HighscoreList.Clear();
                    foreach (var item in tempList.OrderByDescending(x=>x.Score))
                    {
                        this.HighscoreList.Add(item);
                    }
                }
            }
        }

        private void SaveHighscoreList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<SnakeHighscore>));
            using (Stream writer = new FileStream("snake_highsocrelist.xml", FileMode.Create))
            {
                serializer.Serialize(writer, this.HighscoreList);
            }
        }

    }

    public class SnakeHighscore
    {
        public string PlayerName { get; set; }

        public int Score { get; set; }
    }

    public class SnakePart
    {
        public UIElement UiElement { get; set; }

        public Point Position { get; set; }

        public bool IsHead { get; set; }
    }

}
