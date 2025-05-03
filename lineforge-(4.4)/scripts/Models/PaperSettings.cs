using Godot;
using System.Collections.Generic;

namespace LineForge.Models
{
    public class PaperSettings
    {
        public static readonly Dictionary<string, Vector2> PaperSizes = new()
        {
            { "A0", new Vector2(841, 1189) },
            { "A1", new Vector2(594, 841) },
            { "A2", new Vector2(420, 594) },
            { "A3", new Vector2(297, 420) },
            { "A4", new Vector2(210, 297) },
            { "A5", new Vector2(148, 210) },
            { "US Letter", new Vector2(215.9f, 279.4f) },
            { "US Legal", new Vector2(215.9f, 355.6f) },
            { "Tabloid", new Vector2(279.4f, 431.8f) }
        };

        public static readonly Dictionary<string, PenProperties> PenTypes = new()
        {
            { "Sakura Micron 005", new PenProperties { Width = 0.2f, Pressure = 0.8f } },
            { "Sakura Micron 01", new PenProperties { Width = 0.25f, Pressure = 0.8f } },
            { "Sakura Micron 02", new PenProperties { Width = 0.3f, Pressure = 0.8f } },
            { "Sakura Micron 03", new PenProperties { Width = 0.35f, Pressure = 0.8f } },
            { "Sakura Micron 04", new PenProperties { Width = 0.4f, Pressure = 0.8f } },
            { "Sakura Micron 05", new PenProperties { Width = 0.45f, Pressure = 0.8f } },
            { "Sakura Micron 08", new PenProperties { Width = 0.5f, Pressure = 0.8f } },
            { "Uni Pin 0.03", new PenProperties { Width = 0.03f, Pressure = 0.7f } },
            { "Uni Pin 0.05", new PenProperties { Width = 0.05f, Pressure = 0.7f } },
            { "Uni Pin 0.1", new PenProperties { Width = 0.1f, Pressure = 0.7f } },
            { "Uni Pin 0.3", new PenProperties { Width = 0.3f, Pressure = 0.7f } },
            { "Uni Pin 0.5", new PenProperties { Width = 0.5f, Pressure = 0.7f } },
            { "Uni Pin 0.8", new PenProperties { Width = 0.8f, Pressure = 0.7f } },
            { "Staedtler 0.05", new PenProperties { Width = 0.05f, Pressure = 0.75f } },
            { "Staedtler 0.1", new PenProperties { Width = 0.1f, Pressure = 0.75f } },
            { "Staedtler 0.3", new PenProperties { Width = 0.3f, Pressure = 0.75f } },
            { "Staedtler 0.5", new PenProperties { Width = 0.5f, Pressure = 0.75f } },
            { "Staedtler 0.7", new PenProperties { Width = 0.7f, Pressure = 0.75f } }
        };

        public string Size { get; set; } = "A4";
        public string PenType { get; set; } = "Sakura Micron 01";
        public Color PenColor { get; set; } = Colors.Black;
        public Color PaperColor { get; set; } = Colors.White;

        public Vector2 GetSizeInMillimeters()
        {
            return PaperSizes.GetValueOrDefault(Size, PaperSizes["A4"]);
        }

        public PenProperties GetPenProperties()
        {
            return PenTypes.GetValueOrDefault(PenType, PenTypes["Sakura Micron 01"]);
        }
    }

    public class PenProperties
    {
        public float Width { get; set; }
        public float Pressure { get; set; }
    }
}