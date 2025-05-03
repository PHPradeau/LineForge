using Godot;
using LineForge.Models;
using System.Collections.Generic;

namespace LineForge.Services
{
    public class TextRenderer
    {
        private readonly Dictionary<string, Font> _fontCache;

        public TextRenderer()
        {
            _fontCache = new Dictionary<string, Font>();
            PreloadDefaultFonts();
        }

        private void PreloadDefaultFonts()
        {
            foreach (var fontName in TextSettings.DefaultFonts)
            {
                if (!_fontCache.ContainsKey(fontName))
                {
                    var font = new SystemFont();
                    font.FontNames = new[] { fontName };
                    _fontCache[fontName] = font;
                }
            }
        }

        public Image RenderText(TextSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Content))
                return null;

            // Get or create font
            var font = GetFont(settings.FontName);
            
            // Create label for text measurement
            var label = new Label
            {
                Text = settings.Content,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Set font
            var theme = new Theme();
            theme.DefaultFont = font;
            theme.DefaultFontSize = settings.FontSize;
            label.Theme = theme;

            // Get text size
            var textSize = label.GetMinimumSize();
            
            // Create viewport for rendering
            var viewport = new SubViewport
            {
                Size = new Vector2I((int)textSize.X, (int)textSize.Y),
                TransparentBg = true,
                RenderTargetClearMode = SubViewport.ClearMode.Always,
                RenderTargetUpdateMode = SubViewport.UpdateMode.Once
            };
            viewport.AddChild(label);

            // Wait for viewport to render
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
            
            // Get the rendered image
            var image = viewport.GetTexture().GetImage();
            
            // Apply rotation if needed
            if (settings.Rotation != 0)
            {
                var rotatedImage = Image.CreateEmpty(image.GetWidth(), image.GetHeight(), false, Image.Format.Rgba8);
                
                var center = new Vector2(image.GetWidth() / 2.0f, image.GetHeight() / 2.0f);
                var rotation = Mathf.DegToRad(settings.Rotation);
                
                for (int y = 0; y < image.GetHeight(); y++)
                {
                    for (int x = 0; x < image.GetWidth(); x++)
                    {
                        var pos = new Vector2(x - center.X, y - center.Y);
                        var rotated = pos.Rotated(rotation);
                        var newX = (int)(rotated.X + center.X);
                        var newY = (int)(rotated.Y + center.Y);
                        
                        if (newX >= 0 && newX < image.GetWidth() && newY >= 0 && newY < image.GetHeight())
                        {
                            rotatedImage.SetPixel(x, y, image.GetPixel(newX, newY));
                        }
                    }
                }
                
                image = rotatedImage;
            }

            // Clean up
            viewport.QueueFree();
            
            return image;
        }

        private Font GetFont(string fontName)
        {
            if (_fontCache.TryGetValue(fontName, out var font))
                return font;

            // Try loading system font
            var newFont = new SystemFont();
            newFont.FontNames = new[] { fontName };
            _fontCache[fontName] = newFont;
            
            return newFont;
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