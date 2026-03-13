using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using DesktopPet.Classes;

namespace DesktopPet.Views;

public partial class ChatWindow : Window
{
    private Gemini _gemini;
    private StackPanel? _chatPanel;
    private TextBox? _inputTextBox;
    private Button? _sendButton;
    private bool _isLoading = false;

    public ChatWindow()
    {
        InitializeComponent();
        
        _chatPanel = this.FindControl<StackPanel>("ChatPanel");
        _inputTextBox = this.FindControl<TextBox>("InputTextBox");
        _sendButton = this.FindControl<Button>("SendButton");

        _gemini = new Gemini();
        
        if (_sendButton != null)
            _sendButton.Click += SendButton_Click;
        if (_inputTextBox != null)
            _inputTextBox.KeyDown += InputTextBox_KeyDown;
    }

    private void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return && !_isLoading)
        {
            e.Handled = true;
            SendMessage();
        }
    }

    private void SendButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!_isLoading)
        {
            SendMessage();
        }
    }

    private async void SendMessage()
    {
        if (_inputTextBox == null)
            return;

        string? message = _inputTextBox.Text?.Trim();
        
        if (string.IsNullOrEmpty(message))
            return;

        _isLoading = true;
        if (_sendButton != null)
            _sendButton.IsEnabled = false;
        if (_inputTextBox != null)
            _inputTextBox.IsEnabled = false;

        AddMessageToChat(message, true);
        if (_inputTextBox != null)
            _inputTextBox.Text = "";

        try
        {
            string response = await _gemini.GetResponse(message);
            AddMessageToChat(response, false);
        }
        catch (Exception ex)
        {
            AddMessageToChat($"Error: {ex.Message}", false);
        }
        finally
        {
            _isLoading = false;
            if (_sendButton != null)
                _sendButton.IsEnabled = true;
            if (_inputTextBox != null)
            {
                _inputTextBox.IsEnabled = true;
                _inputTextBox.Focus();
            }
        }
    }

    private void AddMessageToChat(string text, bool isUserMessage)
    {
        if (_chatPanel == null)
            return;

        var message = new Border
        {
            Background = isUserMessage ? new Avalonia.Media.SolidColorBrush(new Avalonia.Media.Color(255, 59, 89, 152)) : new Avalonia.Media.SolidColorBrush(new Avalonia.Media.Color(255, 230, 230, 230)),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(12),
            Margin = new Thickness(5, 5, 5, 5),
            HorizontalAlignment = isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            MaxWidth = 300
        };

        var textBlock = new TextBlock
        {
            Text = text,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Foreground = isUserMessage ? new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White) : new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Black)
        };

        message.Child = textBlock;
        _chatPanel.Children.Add(message);

        var scrollViewer = this.FindControl<ScrollViewer>("ChatScrollViewer");
        if (scrollViewer != null)
        {
            scrollViewer.ScrollToEnd();
        }
    }
}