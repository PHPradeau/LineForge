using Godot;
using System;

namespace LineForge.Algorithms
{
    public class StipplingEffect : IImageEffect
    {
        private readonly double _density;
        private readonly Random _random;

        public StipplingEffect(double density)
        {
            _density = density;
            _random = new Random();
        }

        public Image ProcessImage(Image input)
        {
            var output = Image.Create(input.GetWidth(), input.GetHeight(), false, Image.Format.Rgba8);
            output.Fill(Colors.White); // Start with white background

            // Convert to grayscale and apply stippling
            for (int x = 0; x < input.GetWidth(); x++)
            {
                for (int y = 0; y < input.GetHeight(); y++)
                {
                    Color pixelColor = input.GetPixel(x, y);
                    float brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3.0f;
                    
                    // Inverse brightness (darker areas get more dots)
                    float dotProbability = (1.0f - brightness) * _density;
                    
                    // Random sampling based on brightness and density
                    if (_random.NextDouble() < dotProbability)
                    {
                        output.SetPixel(x, y, Colors.Black);
                    }
                }
            }

            return output;
        }
    }
}