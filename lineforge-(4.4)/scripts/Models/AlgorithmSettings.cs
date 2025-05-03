using Godot;

namespace LineForge.Models
{
    public class AlgorithmSettings
    {
        public double ContourThreshold { get; set; } = 0.5;
        public int VoronoiPoints { get; set; } = 100;
        public double StipplingDensity { get; set; } = 0.5;
        public int PixelateSize { get; set; } = 10;
    }
}