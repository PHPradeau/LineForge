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
        private float _currentRotation = 0;

        public PreviewService(TextureRect previewTextureRect)
        {
            _previewTextureRect = previewTextureRect;
            _textRenderer = new TextRenderer();
            _previewTexture = new ImageTexture();
            CreateDefaultImage();
        }

        private void CreateDefaultImage()
        {
            // Create a white canvas with default size
            _currentImage = Image.CreateEmpty(800, 600, false, Image.Format.Rgba8);
            _currentImage.Fill(Colors.White);
            UpdatePreviewTexture();
        }

        public void SetInputImage(Image newImage)
        {
            _currentImage = newImage.Duplicate() as Image;
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
            var processedImage = _currentImage.Duplicate() as Image;

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

        public Image GetCurrentImage()
        {
            return _currentImage?.Duplicate() as Image;
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

            var textImage = _textRenderer.RenderText(textSettings);

            _textRenderer.BlendTextWithImage(image, textImage, Colors.Black);
        }

        private void UpdatePreviewTexture()
        {
            _previewTexture = ImageTexture.CreateFromImage(_currentImage);
            _previewTextureRect.Texture = _previewTexture;
        }

        public void RotateCanvas(float degrees)
        {
            _currentRotation = (_currentRotation + degrees) % 360;
            if (_currentImage == null) return;

            // Create a larger image to accommodate rotation
            var diagonal = (int)Mathf.Sqrt(
                _currentImage.GetWidth() * _currentImage.GetWidth() +
                _currentImage.GetHeight() * _currentImage.GetHeight()
            );

            var rotated = Image.CreateEmpty(diagonal, diagonal, false, Image.Format.Rgba8);
            rotated.Fill(Colors.Transparent);

            // Calculate center points
            var centerNew = new Vector2(diagonal / 2, diagonal / 2);
            var centerOld = new Vector2(_currentImage.GetWidth() / 2, _currentImage.GetHeight() / 2);
            var radians = Mathf.DegToRad(_currentRotation);

            // Copy pixels with rotation
            for (int x = 0; x < _currentImage.GetWidth(); x++)
            {
                for (int y = 0; y < _currentImage.GetHeight(); y++)
                {
                    var pos = new Vector2(x - centerOld.X, y - centerOld.Y);
                    var rotatedPos = new Vector2(
                        pos.X * Mathf.Cos(radians) - pos.Y * Mathf.Sin(radians),
                        pos.X * Mathf.Sin(radians) + pos.Y * Mathf.Cos(radians)
                    );
                    var finalPos = rotatedPos + centerNew;

                    if (finalPos.X >= 0 && finalPos.X < diagonal &&
                        finalPos.Y >= 0 && finalPos.Y < diagonal)
                    {
                        rotated.SetPixel(
                            (int)finalPos.X,
                            (int)finalPos.Y,
                            _currentImage.GetPixel(x, y)
                        );
                    }
                }
            }

            _currentImage = rotated;
            UpdatePreviewTexture();
        }

        public float GetCurrentRotation()
        {
            return _currentRotation;
        }
    }
}