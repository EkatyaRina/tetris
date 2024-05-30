using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tetris
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Массив, содержащий изображение плиток
        /// </summary>
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Resources/TileEmpty.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileCyan.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileBlue.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TilePink.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileYellow.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileGreen.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileViolet.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/TileRed.png",UriKind.Relative)),
        };
        /// <summary>
        /// Массив, содержащий изображение блоков
        /// </summary>
        private readonly ImageSource[] blockImages = new ImageSource[]
       {
            new BitmapImage(new Uri("Resources/BlockEmpty.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-I.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-J.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-L.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-O.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-S.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-T.png",UriKind.Relative)),
            new BitmapImage(new Uri("Resources/Block-Z.png",UriKind.Relative)),
       };
        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000; //максимальная скорость
        private readonly int minDelay = 75; //минимальная скорость
        private readonly int delayDecrease = 25;
        private GameState gameState = new GameState(); //объект состояния игры
        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }
        /// <summary>
        /// Настройка элементов на холсте
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns]; //создание поля 22 на 10 
            int cellSize = 25; //размер каждой ячейки
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };
                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }
        /// <summary>
        /// Создание игровой сетки
        /// </summary>
        /// <param name="grid"></param>
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
        /// <summary>
        /// Создание текущего блока
        /// </summary>
        /// <param name="block"></param>
        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePosition())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.ID];
            }
        }
        /// <summary>
        /// Вывод следующего блока 
        /// </summary>
        /// <param name="blockQueue"></param>
        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.ID];
        }
        /// <summary>
        /// Сохранение блока для дальнейшего использования
        /// </summary>
        /// <param name="heldBlock"></param>
        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.ID];
            }
        }
        /// <summary>
        /// Траектория движения блока
        /// </summary>
        /// <param name="block"></param>
        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePosition())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.ID];
            }
        }

        /// <summary>
        /// Создание игрового поля
        /// </summary>
        /// <param name="gameState"></param>
        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Счет: {gameState.Score}";
        }
        /// <summary>
        /// Падение блоков вниз
        /// </summary>
        /// <returns></returns>
        private async Task GameLoop()
        {
            Draw(gameState);
            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Счет: {gameState.Score}";
        }
        /// <summary>
        /// управление блоками с помощью клавиатуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();

        }
        /// <summary>
        /// Обработчи события кпопки повтора игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
            
        }
        /// <summary>
        /// ну норм
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            StartWindow start= new StartWindow();
            start.Show();
            Close();
        }
        /// <summary>
        /// хуйня полная
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       
    }
}
