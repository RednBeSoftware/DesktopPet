using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DesktopPet.Classes;

namespace DesktopPet.Views;

public partial class MainWindow : Window
{
    private readonly Pet _stickman = new Pet { Name = "Stickman" };
    private ChatWindow? _chatWindow;
    private readonly Random _random = new Random();
    private CancellationTokenSource _moveRandomCancellationTokenSource = new CancellationTokenSource();
    private Task? _moveRandomTask;
    private int _screenWidth = 0;
    private int _screenHeight = 0;

    public MainWindow()
    {
        InitializeComponent();

        Opened += (_, _) => UpdateScreenSize();

        _stickman.Image = ImageStickman;
        try
        {
            _stickman.CreateAnimation("StickmanWave", 1, 3, "png");
            _stickman.CreateAnimation("StickmanWalkLeft", 1, 4, "png");
            _stickman.CreateAnimation("StickmanWalkRight", 1, 4, "png");
            _stickman.CreateAnimation("StickmanTurnRight", 1, 4, "png");
            _stickman.CreateAnimation("StickmanTurnLeft", 1, 4, "png");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        

        PlayAnimationPingPong("StickmanWave", _stickman);

        _moveRandomTask = MoveRandom(_moveRandomCancellationTokenSource);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        int step = 10;

        var currentPosition = this.Position;
        int newX = currentPosition.X;
        int newY = currentPosition.Y;

        if (e.Key == Key.W || e.Key == Key.Up)
        {
            newY -= step;
        }

        if (e.Key == Key.S || e.Key == Key.Down)
        {
            newY += step;
        }

        if (e.Key == Key.D || e.Key == Key.Right)
        {
            newX += step;
        }

        if (e.Key == Key.A || e.Key == Key.Left)
        {
            newX -= step;
        }

        this.Position = new PixelPoint(newX, newY);
    }

    private async Task MoveRandom(CancellationTokenSource cancellationTokenSource)
    {
        while (true)
        {
            try
            {
                double waitTime = _random.NextDouble() * (5.0 - 1.0) + 1.0;
                await Task.Delay(TimeSpan.FromSeconds(waitTime), cancellationTokenSource.Token);

                var currentPos = this.Position;

                int targetDeltaX = _random.Next(-250, 251);
                int targetDeltaY = _random.Next(-250, 251);

                int targetX = currentPos.X + targetDeltaX;
                int targetY = currentPos.Y + targetDeltaY;

                int currentX = currentPos.X;
                int currentY = currentPos.Y;

                while (currentX != targetX || currentY != targetY)
                {
                    if (currentX < targetX) currentX++;
                    else if (currentX > targetX) currentX--;

                    if (currentY < targetY) currentY++;
                    else if (currentY > targetY) currentY--;

                    this.Position = new PixelPoint(currentX, currentY);

                    await Task.Delay(20, cancellationTokenSource.Token);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error: {e.Message}");
                Console.ResetColor();
                break;
            }
        }
    }

    private void PlayAnimationPingPong(string animationName, Pet pet)
    {
        Animation animation = pet.Animations[animationName];
        if (animation.IsPlaying) return;
        animation.IsPlaying = true;

        int currentFrameIndex = 0;
        int direction = 1;
        int lastFrameIndex = animation.Frames.Count - 1;

        // Clear any existing event handlers to prevent accumulation
        animation.Timer.Tick -= null;
        animation.Timer.Tick += (sender, e) =>
        {
            if (animation.Frames.Count == 0) return;
            if (currentFrameIndex < 0) currentFrameIndex = 0;
            if (currentFrameIndex > lastFrameIndex) currentFrameIndex = lastFrameIndex;

            pet.Image.Source = animation.Frames[currentFrameIndex];
            currentFrameIndex += direction;

            if (currentFrameIndex > lastFrameIndex || currentFrameIndex < 0)
            {
                direction *= -1;
                currentFrameIndex += direction;
            }
        };
        animation.Timer.Start();
    }

    private void OpenChat_OnClick(object? sender, RoutedEventArgs e)
    {
        //if (Environment.GetEnvironmentVariable("GeminiApiKey") is null) return;

        Dispatcher.UIThread.Post(() =>
        {
            if (_chatWindow == null || !_chatWindow.IsVisible)
            {
                _chatWindow = new ChatWindow();
                _chatWindow.Show();
            }
            else
            {
                _chatWindow.Activate();
            }
        });
    }

    private async void Update_OnClick(object? sender, RoutedEventArgs e)
    {
        return;

        //try
        //{
        //     // var source = new GithubSource
        // ("https://github.com/RednBeSoftware/DesktopPet.git",
        //     null,
        //     false,
        //    null);

        // var mgr = new UpdateManager(source);
        // var newVersion = await mgr.CheckForUpdatesAsync();
        // }
        //catch (Exception exception)
        // {
        //     return;
        // }
    }

    private void CloseApp_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void StopAnimation_OnClick(object? sender, RoutedEventArgs e)
    {
        var stickmansAnimation = _stickman.Animations["StickmanWave"];
        try
        {
            stickmansAnimation.Timer.Stop();
            stickmansAnimation.IsPlaying = false;
            _stickman.Image.Source = new Bitmap(AssetLoader.Open(new Uri
                ("avares://DesktopPet/Assets/Stickman/StickmanDefault.png", UriKind.RelativeOrAbsolute)));
        }
        catch (Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Error: {exception}");
            Console.ResetColor();
        }
    }

    private void StopMove_OnClick(object? sender, RoutedEventArgs e)
    {
        _moveRandomCancellationTokenSource.Cancel();
    }

    private void MoveRandom_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_moveRandomCancellationTokenSource.IsCancellationRequested)
        {
            _moveRandomCancellationTokenSource = new CancellationTokenSource();
        }

        _moveRandomTask = MoveRandom(_moveRandomCancellationTokenSource);
    }

    private void UpdateScreenSize()
    {
        var screen = this.Screens.Primary;
        if (screen != null && screen.Bounds.Width > 0 && screen.Bounds.Height > 0)
        {
            _screenWidth = screen.Bounds.Width;
            _screenHeight = screen.Bounds.Height;
            return;
        }

        if (Screens.All.Count > 0)
        {
            var firstScreen = Screens.All[0];
            _screenWidth = firstScreen.Bounds.Width;
            _screenHeight = firstScreen.Bounds.Height;
            return;
        }

        _screenWidth = (int)Bounds.Width;
        _screenHeight = (int)Bounds.Height;
    }

    private void StartWaveMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_stickman.Animations["StickmanWave"].IsPlaying) return;
        PlayAnimationPingPong("StickmanWave", _stickman);
    }

    private void TestMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        UpdateScreenSize();
        Console.WriteLine($"Screen Size: {_screenWidth}x{_screenHeight}");
    }
}