using Godot;
using System.Collections.Generic;

namespace LineForge.Models
{
    public class PaperSettings
    {
        // Paper properties
        public string Size { get; set; } = "A4";
        public string PenType { get; set; } = "Sakura";
        public Color PenColor { get; set; } = Colors.Black;
        public Color PaperColor { get; set; } = Colors.White;

        // Standard paper sizes in millimeters (width, height)
        public static readonly Dictionary<string, Vector2> PaperSizes = new Dictionary<string, Vector2>
        {
            { "A0", new Vector2(841, 1189) },
            { "A1", new Vector2(594, 841) },
            { "A2", new Vector2(420, 594) },
            { "A3", new Vector2(297, 420) },
            { "A4", new Vector2(210, 297) },
            { "A5", new Vector2(148, 210) },
            { "US Letter", new Vector2(215.9f, 279.4f) },
            { "US Legal", new Vector2(215.9f, 355.6f) },
            { "US Tabloid", new Vector2(279.4f, 431.8f) }
        };

        // Pen types with their properties
        public static readonly Dictionary<string, PenProperties> PenTypes = new Dictionary<string, PenProperties>
        {
            { "Sakura Micron", new PenProperties { LineWidth = 0.25f, MinPressure = 10, MaxPressure = 100 } },
            { "Sakura Pigma", new PenProperties { LineWidth = 0.35f, MinPressure = 15, MaxPressure = 120 } },
            { "Staedtler Fineliner", new PenProperties { LineWidth = 0.3f, MinPressure = 12, MaxPressure = 110 } },
            { "Faber-Castell Pitt", new PenProperties { LineWidth = 0.4f, MinPressure = 20, MaxPressure = 150 } },
            { "Uni Pin", new PenProperties { LineWidth = 0.2f, MinPressure = 8, MaxPressure = 90 } },
            { "Copic Multiliner", new PenProperties { LineWidth = 0.35f, MinPressure = 15, MaxPressure = 130 } }
        };
    }

    public class PenProperties
    {
        public float LineWidth { get; set; }  // Line width in millimeters
        public int MinPressure { get; set; }  // Minimum pressure for the pen (for G-code)
        public int MaxPressure { get; set; }  // Maximum pressure for the pen (for G-code)
    }
}