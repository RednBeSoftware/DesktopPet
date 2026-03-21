using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DesktopPet.Classes;

namespace DesktopPet.Views;

public partial class MainWindow : Window
{
    private readonly Pet _stickman = new Pet { Name = "Stickman" };
    private ChatWindow? _chatWindow;

    public MainWindow()
    {
        InitializeComponent();
        _stickman.Image = ImageStickman;
        _stickman.CreateAnimation("StickmanWave", 1, 3, "png");
        PlayAnimationPingPong("StickmanWave", _stickman);

        MoveRandom();
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

    private async Task MoveRandom()
    {
        try
        {
            var random = new Random();

            int stepX = random.Next(10, 51);
            int stepY = random.Next(10, 51);
            int directionX = random.Next(-1, 2);
            int directionY = random.Next(-1, 2);
            double startTimeMax = 1;
            double startTimeMin = 5;
            double startTime = random.NextDouble() * (startTimeMax - startTimeMin) + startTimeMin;

            var currentPosition = this.Position;
            var newX = currentPosition.X;
            var newY = currentPosition.Y;

            await Task.Delay((int)startTime * 1000);
            
            newX += stepX * directionX;
            newY += stepY * directionY;

            this.Position = new PixelPoint(newX, newY);
        }
        catch (Exception e)
        {
            return;
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
        if (Environment.GetEnvironmentVariable("GeminiApiKey") is null) return;

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
}