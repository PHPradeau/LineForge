using Godot;

namespace LineForge.Models
{
    public class TextSettings
    {
        public string Content { get; set; } = "";
        public string FontType { get; set; } = "Default";
        public double Size { get; set; } = 12.0;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public double Rotation { get; set; } = 0.0;
    }
}