using System.Windows;

namespace ScoreCard.Models
{
    public class SimplePrompt
    {
        public string Message { get; }
        public string Title { get; }
        public MessageBoxButton Button { get; }
        public MessageBoxImage Image { get; }

        public MessageBoxResult Result { get; set; } = MessageBoxResult.Cancel;

        public SimplePrompt(string message, string title, MessageBoxButton button, MessageBoxImage image)
        {
            Message = message;
            Title = title;
            Button = button;
            Image = image;
        }
    }
}
