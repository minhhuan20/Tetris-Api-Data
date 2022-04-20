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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Tetrisgame.Model;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/huan.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
            getHighScore();

        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.W:
                    gameState.RotateBlockCW();
                    break;
                case Key.S:
                    gameState.MoveBlockDown();
                    break;
                case Key.A:
                    gameState.MoveBlockLeft();
                    break;
                case Key.D:
                    gameState.MoveBlockRight();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            saveHighScore();
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void getHighScore()
        {
            List<ScoreModel> scoreModels = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7009/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync("Tetris");
                responseMessage.EnsureSuccessStatusCode();
                var constring = await responseMessage.Content.ReadAsStringAsync();
                scoreModels = JsonConvert.DeserializeObject<List<ScoreModel>>(constring);
                var res = "";
                foreach (ScoreModel score in scoreModels)
                {
                    string day = score.GameTime.Day.ToString() + "/" + score.GameTime.Month.ToString() + "/" + score.GameTime.Year.ToString();
                    res += score.NickName + ": \t" + score.Score + "pts" + "   " + score.GameTime + Environment.NewLine;
                }
                txtHighScore.Text = res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private async void saveHighScore()
        {

            HttpClient client = new HttpClient();
            string user = txtName.Text;
            ScoreModel score1 = new ScoreModel();
            score1.NickName = user;
            DateTime localDate = DateTime.Now;
            int score = gameState.Score;
            if (score > 0)
            {
                client.BaseAddress = new Uri("https://localhost:7009/api/");

                var payload = "{\"NickName\": \"" + score1.NickName + "\",\"score\": " + score + ",\"GameTime\": \"" + localDate.ToString("yyyy-MM-ddTHH:mm:ss") + "\"}";
                //var payload = "{\"Nickname\": \"" + score1.NickName + "\",\"score\": " + score + "}";
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

                    HttpResponseMessage responseMessage = client.PostAsync("Tetris", c).GetAwaiter().GetResult();
                    responseMessage.EnsureSuccessStatusCode(); // throws if not 200-299
                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
