using Godot;

namespace LineForge.Models
{
    public class TextSettings
    {
        public string Content { get; set; } = "";
        public string FontName { get; set; } = "Arial";
        public int FontSize { get; set; } = 12;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0.0f;

        public static readonly string[] DefaultFonts = new[]
        {
            "Arial",
            "Times New Roman",
            "Courier New",
            "Georgia",
            "Verdana",
            "Helvetica",
            "Open Sans",
            "Roboto",
            "Montserrat",
            "Ubuntu"
        };
    }
}