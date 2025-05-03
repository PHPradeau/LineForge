using Godot;
using LineForge.Models;

namespace LineForge.Algorithms
{
    public class LineContoursEffect : IImageEffect
    {
        private readonly double _threshold;

        public LineContoursEffect(double threshold)
        {
            _threshold = threshold;
        }

        public Image ProcessImage(Image input)
        {
            var output = Image.CreateEmpty(input.GetWidth(), input.GetHeight(), false, Image.Format.Rgba8);
            
            // Convert to grayscale and detect edges using Sobel operator
            for (int x = 1; x < input.GetWidth() - 1; x++)
            {
                for (int y = 1; y < input.GetHeight() - 1; y++)
                {
                    // Sobel operators for edge detection
                    float gx = 
                        GetGrayScale(input.GetPixel(x + 1, y - 1)) + 2 * GetGrayScale(input.GetPixel(x + 1, y)) + GetGrayScale(input.GetPixel(x + 1, y + 1)) -
                        GetGrayScale(input.GetPixel(x - 1, y - 1)) - 2 * GetGrayScale(input.GetPixel(x - 1, y)) - GetGrayScale(input.GetPixel(x - 1, y + 1));

                    float gy = 
                        GetGrayScale(input.GetPixel(x - 1, y + 1)) + 2 * GetGrayScale(input.GetPixel(x, y + 1)) + GetGrayScale(input.GetPixel(x + 1, y + 1)) -
                        GetGrayScale(input.GetPixel(x - 1, y - 1)) - 2 * GetGrayScale(input.GetPixel(x, y - 1)) - GetGrayScale(input.GetPixel(x + 1, y - 1));

                    float magnitude = Mathf.Sqrt(gx * gx + gy * gy);
                    
                    // Apply threshold
                    Color outputColor = magnitude > _threshold ? Colors.Black : Colors.White;
                    output.SetPixel(x, y, outputColor);
                }
            }

            return output;
        }

        private float GetGrayScale(Color color)
        {
            return (color.R + color.G + color.B) / 3.0f;
        }
    }
}