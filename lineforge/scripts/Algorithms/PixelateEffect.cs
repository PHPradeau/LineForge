using Godot;

namespace LineForge.Algorithms
{
    public class PixelateEffect : IImageEffect
    {
        private readonly int _pixelSize;

        public PixelateEffect(int pixelSize)
        {
            _pixelSize = Mathf.Max(1, pixelSize); // Ensure minimum pixel size of 1
        }

        public Image ProcessImage(Image input)
        {
            var output = Image.CreateEmpty(input.GetWidth(), input.GetHeight(), false, Image.Format.Rgba8);
            
            // Process the image in blocks of pixel_size x pixel_size
            for (int x = 0; x < input.GetWidth(); x += _pixelSize)
            {
                for (int y = 0; y < input.GetHeight(); y += _pixelSize)
                {
                    // Calculate the average color for this block
                    Color avgColor = CalculateAverageColor(input, x, y);
                    
                    // Fill the block with the average color
                    FillBlock(output, x, y, avgColor);
                }
            }

            return output;
        }

        private Color CalculateAverageColor(Image input, int startX, int startY)
        {
            float r = 0, g = 0, b = 0, a = 0;
            int count = 0;

            // Calculate boundaries for the current block
            int endX = Mathf.Min(startX + _pixelSize, input.GetWidth());
            int endY = Mathf.Min(startY + _pixelSize, input.GetHeight());

            // Sum up all colors in the block
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Color pixel = input.GetPixel(x, y);
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    a += pixel.A;
                    count++;
                }
            }

            // Calculate average
            if (count > 0)
            {
                return new Color(r / count, g / count, b / count, a / count);
            }

            return Colors.Transparent;
        }

        private void FillBlock(Image output, int startX, int startY, Color color)
        {
            // Calculate boundaries for the current block
            int endX = Mathf.Min(startX + _pixelSize, output.GetWidth());
            int endY = Mathf.Min(startY + _pixelSize, output.GetHeight());

            // Fill the block with the average color
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    output.SetPixel(x, y, color);
                }
            }
        }
    }
}