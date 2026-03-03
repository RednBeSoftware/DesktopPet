using Avalonia.Controls;
using DesktopPet.Classes;
using Avalonia.Media.Imaging;
using System;

namespace DesktopPet.Views;

public partial class MainWindow : Window
{
    Pet _stickman = new Pet { Name = "Stickman" };

    public MainWindow()
    {
        InitializeComponent();
        _stickman.Image = ImageStickman;
        CreateAnimation("StickmanWave", _stickman, 1, 3, "png");
        PlayAnimationPingPong("StickmanWave", _stickman);
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

    private void CreateAnimation(string animationName, Pet pet, int firstFrameNumber, int lastFrameNumber, string extension)
    {
        Animation animation = new Animation{ Name = animationName, FramesPerSecond = 10 };
        for (int i = firstFrameNumber; i <= lastFrameNumber; i++)
        {
            string uriPath = $"avares://DesktopPet/Assets/{pet.Name}/{animationName}/{animationName}{i}.{extension}";
            
            var asset = Avalonia.Platform.AssetLoader.Open(new Uri(uriPath));
            animation.Frames.Add(new Bitmap(asset));
        }
        pet.Animations.Add(animationName, animation);
    }
}