using Godot;
using LineForge.Models;
using LineForge.Algorithms;
using System.Collections.Generic;

namespace LineForge.Services
{
    public class PreviewService
    {
        private readonly TextureRect _previewTextureRect;
        private readonly TextRenderer _textRenderer;
        private Image _currentImage;
        private ImageTexture _previewTexture;

        public PreviewService(TextureRect previewTextureRect)
        {
            _previewTextureRect = previewTextureRect;
            _textRenderer = new TextRenderer();
            _previewTexture = ImageTexture.Create();
            CreateDefaultImage();
        }

        private void CreateDefaultImage()
        {
            // Create a white canvas with default size
            _currentImage = Image.Create(800, 600, false, Image.Format.Rgba8);
            _currentImage.Fill(Colors.White);
            UpdatePreviewTexture();
        }

        public void SetInputImage(Image newImage)
        {
            _currentImage = newImage;
            UpdatePreview(new PaperSettings(), new AlgorithmSettings(), new TextSettings());
        }

        public void UpdatePreview(PaperSettings paperSettings, AlgorithmSettings algoSettings, TextSettings textSettings)
        {
            if (_currentImage == null)
            {
                CreateDefaultImage();
                return;
            }

            // Create a working copy of the image
            var processedImage = _currentImage.Duplicate();

            // Create a list to store the active effects
            var effects = new List<IImageEffect>();

            // Add effects based on settings (we can make this more sophisticated later)
            if (algoSettings.ContourThreshold > 0)
            {
                effects.Add(new LineContoursEffect(algoSettings.ContourThreshold));
            }

            if (algoSettings.VoronoiPoints > 0)
            {
                effects.Add(new VoronoiEffect(algoSettings.VoronoiPoints));
            }

            if (algoSettings.StipplingDensity > 0)
            {
                effects.Add(new StipplingEffect(algoSettings.StipplingDensity));
            }

            if (algoSettings.PixelateSize > 1)
            {
                effects.Add(new PixelateEffect(algoSettings.PixelateSize));
            }

            // Apply each effect in sequence
            foreach (var effect in effects)
            {
                processedImage = effect.ProcessImage(processedImage);
            }

            // Apply paper color (blend with white background)
            ApplyPaperColor(processedImage, paperSettings.PaperColor);

            // Apply pen color (replace black with pen color)
            ApplyPenColor(processedImage, paperSettings.PenColor);

            // Apply text overlay using TextRenderer
            if (!string.IsNullOrEmpty(textSettings.Content))
            {
                ApplyTextOverlay(processedImage, textSettings);
            }

            // Update the preview
            _currentImage = processedImage;
            UpdatePreviewTexture();
        }

        private void ApplyPaperColor(Image image, Color paperColor)
        {
            for (int x = 0; x < image.GetWidth(); x++)
            {
                for (int y = 0; y < image.GetHeight(); y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    // Blend with paper color based on pixel alpha
                    float alpha = pixelColor.A;
                    Color blendedColor = new Color(
                        Mathf.Lerp(paperColor.R, pixelColor.R, alpha),
                        Mathf.Lerp(paperColor.G, pixelColor.G, alpha),
                        Mathf.Lerp(paperColor.B, pixelColor.B, alpha),
                        1.0f
                    );
                    image.SetPixel(x, y, blendedColor);
                }
            }
        }

        private void ApplyPenColor(Image image, Color penColor)
        {
            for (int x = 0; x < image.GetWidth(); x++)
            {
                for (int y = 0; y < image.GetHeight(); y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    // Replace black (or near-black) pixels with pen color
                    if (IsNearlyBlack(pixelColor))
                    {
                        float darkness = 1.0f - ((pixelColor.R + pixelColor.G + pixelColor.B) / 3.0f);
                        Color adjustedPenColor = new Color(
                            penColor.R * darkness,
                            penColor.G * darkness,
                            penColor.B * darkness,
                            pixelColor.A
                        );
                        image.SetPixel(x, y, adjustedPenColor);
                    }
                }
            }
        }

        private bool IsNearlyBlack(Color color)
        {
            const float threshold = 0.2f;
            return (color.R + color.G + color.B) / 3.0f < threshold;
        }

        private void ApplyTextOverlay(Image image, TextSettings textSettings)
        {
            if (string.IsNullOrEmpty(textSettings.Content))
                return;

            var textImage = _textRenderer.RenderText(
                textSettings,
                image.GetWidth(),
                image.GetHeight()
            );

            _textRenderer.BlendTextWithImage(image, textImage, Colors.Black);
        }

        private void UpdatePreviewTexture()
        {
            _previewTexture = ImageTexture.CreateFromImage(_currentImage);
            _previewTextureRect.Texture = _previewTexture;
        }
    }
}