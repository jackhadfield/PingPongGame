using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PingPongGame
{
    public partial class MainWindow : Window
    {
        const double GameWidth = 800;
        const double GameHeight = 600;
        const double PaddleWidth = 100;
        const double PaddleHeight = 15;
        const double BallSize = 20;
        const double WallThickness = 10;
        const double PaddleSpeed = 5;
        const double BallSpeed = 3;

        double paddleX = (GameWidth - PaddleWidth) / 2;
        double paddleY = GameHeight - WallThickness - PaddleHeight - 20;

        double ballX = GameWidth / 2;
        double ballY = GameHeight / 2;
        double ballVelX = BallSpeed;
        double ballVelY = -BallSpeed;

        bool gameOver = false;
        bool keyLeftPressed = false;
        bool keyRightPressed = false;

        DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GameCanvas.Focus();
            InitializeGame();
            StartGameLoop();
        }

        void InitializeGame()
        {
            paddleX = (GameWidth - PaddleWidth) / 2;
            paddleY = GameHeight - WallThickness - PaddleHeight - 20;
            ballX = GameWidth / 2;
            ballY = GameHeight / 2;
            ballVelX = BallSpeed;
            ballVelY = -BallSpeed;
            gameOver = false;
            GameOverOverlay.Visibility = Visibility.Collapsed;
        }

        void StartGameLoop()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                HandleInput();
                UpdateGame();
                DrawGame();
            }
        }

        void HandleInput()
        {
            if (keyLeftPressed && paddleX > WallThickness)
            {
                paddleX -= PaddleSpeed;
            }
            if (keyRightPressed && paddleX + PaddleWidth < GameWidth - WallThickness)
            {
                paddleX += PaddleSpeed;
            }
        }

        void UpdateGame()
        {
            ballX += ballVelX;
            ballY += ballVelY;

            // Ball collision with side walls
            if (ballX <= WallThickness || ballX + BallSize >= GameWidth - WallThickness)
            {
                ballVelX = -ballVelX;
                ballX = ballX <= WallThickness ? WallThickness : GameWidth - WallThickness - BallSize;
            }

            // Ball collision with top wall
            if (ballY <= WallThickness)
            {
                ballVelY = -ballVelY;
                ballY = WallThickness;
            }

            // Ball collision with paddle
            if (ballY + BallSize >= paddleY && ballY <= paddleY + PaddleHeight)
            {
                if (ballX + BallSize >= paddleX && ballX <= paddleX + PaddleWidth)
                {
                    ballVelY = -Math.Abs(ballVelY);
                    ballY = paddleY - BallSize;

                    // Add some angle based on where the ball hits the paddle
                    double hitPosition = (ballX - paddleX) / PaddleWidth;
                    ballVelX = (hitPosition - 0.5) * BallSpeed * 2;
                }
            }

            // Check if ball is missed
            if (ballY >= GameHeight - WallThickness)
            {
                gameOver = true;
                GameOverOverlay.Visibility = Visibility.Visible;
                GameCanvas.Focus();
            }
        }

        void DrawGame()
        {
            Canvas.SetLeft(Paddle, paddleX);
            Canvas.SetTop(Paddle, paddleY);

            Canvas.SetLeft(Ball, ballX);
            Canvas.SetTop(Ball, ballY);
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                keyLeftPressed = true;
            }
            else if (e.Key == Key.D)
            {
                keyRightPressed = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (e.Key == Key.R && gameOver)
            {
                ResetGame();
            }
        }

        void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                keyLeftPressed = false;
            }
            else if (e.Key == Key.D)
            {
                keyRightPressed = false;
            }
        }

        void ResetGame()
        {
            InitializeGame();
        }
    }
}

