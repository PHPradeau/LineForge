using Godot;
using LineForge.Models;

namespace LineForge.Services
{
    public class TextRenderer
    {
        private const string DEFAULT_FONT = "res://assets/fonts/default.ttf";

        public Image RenderText(TextSettings settings, int width, int height)
        {
            // Create a viewport to render the text
            var viewport = new SubViewport();
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
            viewport.Size = new Vector2I(width, height);
            viewport.TransparentBg = true;

            // Create a label for the text
            var label = new Label();
            label.Text = settings.Content;
            label.Position = new Vector2(settings.Position.X, settings.Position.Y);
            label.RotationDegrees = (float)settings.Rotation;
            
            // Set font and size
            var fontData = new SystemFont(); // Default system font for now
            var font = new FontVariation();
            font.SetBase(fontData);
            font.SetSize(settings.Size);
            
            label.AddThemeFontSizeOverride("font_size", (int)settings.Size);
            label.AddThemeFontOverride("font", font);

            // Add label to viewport
            viewport.AddChild(label);

            // Wait for the viewport to render
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
            
            // Get the image
            var image = viewport.GetTexture().GetImage();
            
            // Cleanup
            viewport.QueueFree();
            
            return image;
        }

        public void BlendTextWithImage(Image targetImage, Image textImage, Color textColor)
        {
            for (int x = 0; x < textImage.GetWidth(); x++)
            {
                for (int y = 0; y < textImage.GetHeight(); y++)
                {
                    Color pixelColor = textImage.GetPixel(x, y);
                    if (pixelColor.A > 0)
                    {
                        // Blend text color with alpha from rendered text
                        Color blendedColor = new Color(
                            textColor.R,
                            textColor.G,
                            textColor.B,
                            pixelColor.A
                        );
                        targetImage.SetPixel(x, y, blendedColor);
                    }
                }
            }
        }
    }
}