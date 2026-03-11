using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;

namespace DesktopPet.Classes;

internal class Pet
{
    public Pet()
    {
        Name = "New Pet";
        Animations = new Dictionary<string, Animation>();

        Image = new Image();
    }

    public string Name { get; set; }
    public Dictionary<string, Animation> Animations { get; set; }
    public Image Image { get; set; }
    public Gemini Gemini { get; set; }

    public void CreateAnimation(string animationName, int firstFrameNumber, int lastFrameNumber, string extension)
    {
        Pet pet = this;
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