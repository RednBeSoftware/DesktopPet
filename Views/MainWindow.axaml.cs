using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

        var openChatMenuItem = this.FindControl<MenuItem>("OpenChatMenuItem");
        if (openChatMenuItem != null)
        {
            openChatMenuItem.Click += (sender, e) => OpenChatWindow();
        }
    }

    private void OpenChatWindow()
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
}