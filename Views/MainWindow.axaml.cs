using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DesktopPet.Classes;
using Velopack;
using Velopack.Sources;

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
        
        //PAT Test
        Console.WriteLine("HelloWorld");
        Console.WriteLine("HelloWorld");
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

    private void OpenChatMenuItem_OnClick(object? sender, RoutedEventArgs e)
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
    }

    private async void Update_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var souce = new GithubSource
            ("https://github.com/RednBeSoftware/DesktopPet.git",
                null,
                false,
                null);

            var mgr = new UpdateManager(souce);
            var newVersion = await mgr.CheckForUpdatesAsync();
        }
        catch (Exception exception)
        {
            return;
        }
    }
}