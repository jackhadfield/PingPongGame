using System;
using System.Threading;

namespace PingPongGame
{
    class Program
    {
        const int Width = 80;
        const int Height = 24;
        const char PaddleChar = '█';
        const char BallChar = '●';
        const char WallChar = '═';
        const char SideWallChar = '║';

        static int paddleX = Width / 2 - 2;
        static int paddleY = Height - 2;
        static int paddleWidth = 5;

        static double ballX = Width / 2.0;
        static double ballY = Height / 2.0;
        static double ballVelX = 0.5;
        static double ballVelY = -0.5;

        static bool gameRunning = true;
        static bool gameOver = false;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Width, Height);
            Console.SetBufferSize(Width, Height);

            // Game loop
            while (gameRunning)
            {
                if (!gameOver)
                {
                    HandleInput();
                    UpdateGame();
                    DrawGame();
                }
                else
                {
                    DrawGameOver();
                }

                Thread.Sleep(50); // ~20 FPS
            }
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.A && paddleX > 1)
                {
                    paddleX--;
                }
                else if (key.Key == ConsoleKey.D && paddleX + paddleWidth < Width - 1)
                {
                    paddleX++;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    gameRunning = false;
                }
            }
        }

        static void UpdateGame()
        {
            // Update ball position
            ballX += ballVelX;
            ballY += ballVelY;

            // Ball collision with side walls
            if (ballX <= 1 || ballX >= Width - 2)
            {
                ballVelX = -ballVelX;
                ballX = ballX <= 1 ? 1 : Width - 2;
            }

            // Ball collision with top wall
            if (ballY <= 1)
            {
                ballVelY = -ballVelY;
                ballY = 1;
            }

            // Ball collision with paddle
            if (ballY >= paddleY - 1 && ballY <= paddleY + 1)
            {
                int ballXInt = (int)Math.Round(ballX);
                if (ballXInt >= paddleX && ballXInt < paddleX + paddleWidth)
                {
                    ballVelY = -Math.Abs(ballVelY); // Always bounce up
                    ballY = paddleY - 1;

                    // Add some angle based on where the ball hits the paddle
                    double hitPosition = (ballXInt - paddleX) / (double)paddleWidth;
                    ballVelX = (hitPosition - 0.5) * 1.0; // -0.5 to 0.5 range
                }
            }

            // Check if ball is missed (went past the paddle)
            if (ballY >= Height - 1)
            {
                gameOver = true;
            }
        }

        static void DrawGame()
        {
            Console.Clear();

            // Draw top wall
            for (int x = 0; x < Width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write(WallChar);
            }

            // Draw side walls
            for (int y = 1; y < Height - 1; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write(SideWallChar);
                Console.SetCursorPosition(Width - 1, y);
                Console.Write(SideWallChar);
            }

            // Draw bottom wall (before paddle area)
            for (int x = 0; x < Width; x++)
            {
                Console.SetCursorPosition(x, Height - 1);
                Console.Write(WallChar);
            }

            // Draw paddle
            for (int x = 0; x < paddleWidth; x++)
            {
                Console.SetCursorPosition(paddleX + x, paddleY);
                Console.Write(PaddleChar);
            }

            // Draw ball
            int ballXInt = (int)Math.Round(ballX);
            int ballYInt = (int)Math.Round(ballY);
            if (ballXInt >= 0 && ballXInt < Width && ballYInt >= 0 && ballYInt < Height)
            {
                Console.SetCursorPosition(ballXInt, ballYInt);
                Console.Write(BallChar);
            }
        }

        static void DrawGameOver()
        {
            Console.Clear();
            string message = "GAME OVER - You missed the ball!";
            string restartMessage = "Press ESC to exit";
            int messageX = (Width - message.Length) / 2;
            int restartX = (Width - restartMessage.Length) / 2;

            Console.SetCursorPosition(messageX, Height / 2 - 1);
            Console.Write(message);
            Console.SetCursorPosition(restartX, Height / 2);
            Console.Write(restartMessage);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    gameRunning = false;
                }
            }
        }
    }
}
