using System.Collections.Generic;
using Avalonia.Controls;

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
}