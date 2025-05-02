using Godot;

namespace LineForge.Models
{
    public class PaperSettings
    {
        public string Size { get; set; } = "A4";
        public string PenType { get; set; } = "Sakura";
        public Color PenColor { get; set; } = Colors.Black;
        public Color PaperColor { get; set; } = Colors.White;
    }
}