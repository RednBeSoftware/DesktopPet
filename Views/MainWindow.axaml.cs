using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using DesktopPet.Classes;
using Google.GenAI;
using DotNetEnv;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace DesktopPet.Views;

public partial class MainWindow : Window
{
    Pet _stickman = new Pet { Name = "Stickman" };

    public MainWindow()
    {
        InitializeComponent();
        _stickman.Image = ImageStickman;
        _stickman.CreateAnimation("StickmanWave", 1, 3, "png");
        PlayAnimationPingPong("StickmanWave", _stickman);

        this.Opened += SetAIOnWindowOpened;
    }

    private async void SetAIOnWindowOpened(object sender, EventArgs e)
    {
         _stickman.Gemini = new Gemini();
        string promt = "Say 'this is a test'";
        string geminiResponse = await _stickman.Gemini.GetResponse(promt);
        AiTest.Text = geminiResponse;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        int step = 10;

        var currentPosition = this.Position;
        int newX = currentPosition.X;
        int newY = currentPosition.Y;

        switch (e.Key)
        {
            case Key.A:
                newX -= step;
                break;
            case Key.D:
                newX += step;
                break;
            case Key.W:
                newY -= step;
                break;
            case Key.S:
                newY += step;
                break;
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
            if (currentFrameIndex < 0 ) currentFrameIndex = 0; 
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