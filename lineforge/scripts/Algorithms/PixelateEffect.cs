using Godot;

namespace LineForge.Algorithms
{
    public class PixelateEffect : IImageEffect
    {
        private readonly int _pixelSize;

        public PixelateEffect(int pixelSize)
        {
            _pixelSize = Mathf.Max(1, pixelSize);
        }

        public Image ProcessImage(Image input)
        {
            var output = Image.Create(input.GetWidth(), input.GetHeight(), false, Image.Format.Rgba8);

            for (int x = 0; x < input.GetWidth(); x += _pixelSize)
            {
                for (int y = 0; y < input.GetHeight(); y += _pixelSize)
                {
                    // Get the average color of the pixel block
                    Color avgColor = GetAverageColor(input, x, y, _pixelSize);

                    // Fill the pixel block with the average color
                    for (int px = x; px < Mathf.Min(x + _pixelSize, input.GetWidth()); px++)
                    {
                        for (int py = y; py < Mathf.Min(y + _pixelSize, input.GetHeight()); py++)
                        {
                            output.SetPixel(px, py, avgColor);
                        }
                    }
                }
            }

            return output;
        }

        private Color GetAverageColor(Image input, int startX, int startY, int size)
        {
            float r = 0, g = 0, b = 0, a = 0;
            int count = 0;

            int endX = Mathf.Min(startX + size, input.GetWidth());
            int endY = Mathf.Min(startY + size, input.GetHeight());

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

            if (count == 0) return Colors.Black;

            return new Color(r / count, g / count, b / count, a / count);
        }
    }
}