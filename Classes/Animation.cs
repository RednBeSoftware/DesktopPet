using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System.Collections.Generic;

namespace DesktopPet.Classes;

internal class Animation
{
    public Animation()
    {
        Name = "New Animation";
        Frames = new List<Bitmap>();
        FramesPerSecond = 10;
        Timer = new DispatcherTimer();
        IsPlaying = false;

        Timer.Interval = System.TimeSpan.FromSeconds(1.0 / FramesPerSecond);
    }

    public string Name { get; set; }
    public List<Bitmap> Frames { get; set; }
    public int FramesPerSecond { get; set; }
    public DispatcherTimer Timer { get; set; }
    public bool IsPlaying { get; set; }
}