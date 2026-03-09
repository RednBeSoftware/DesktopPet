using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using DesktopPet.Classes;
using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;

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

        this.Loaded += async (s, e) => await TestAI();
    }

    public async Task TestAI() 
{
    try 
    {
        AiTest.Text = "Bağlanıyor...";
        
        var apiKey = System.Environment.GetEnvironmentVariable("GeminiApiKey");
        var client = new Client(apiKey: apiKey);

        var response = await client.Models.GenerateContentAsync(
            model: "models/gemini-2.5-flash", 
            contents: "Türkiye'nin başkenti neresi"
        );

        if (response?.Candidates?[0]?.Content?.Parts?[0] != null)
        {
            AiTest.Text = response.Candidates[0].Content.Parts[0].Text;
        }
    }
    catch (Exception ex)
    {
        AiTest.Text = $"Hata: {ex.Message}";
        
        if (ex.Message.Contains("429")) {
            AiTest.Text = "Kota doldu, 1 dakika bekleyin.";
        }
    }
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