using Godot;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace LineForge.Services
{
    public class CodeVisualizer
    {
        private readonly int _fontSize = 12;
        private readonly int _lineHeight = 16;
        private readonly int _padding = 20;
        private readonly Color _textColor = Colors.Black;
        private readonly Color _keywordColor = new Color(0.6f, 0.0f, 0.6f); // Purple
        private readonly Color _stringColor = new Color(0.8f, 0.2f, 0.0f);  // Red-Orange
        private readonly Color _commentColor = new Color(0.0f, 0.6f, 0.0f); // Green

        public Image CreateCodeImage(string code, string language)
        {
            var lines = code.Split('\n');
            var width = CalculateImageWidth(lines) + 2 * _padding;
            var height = lines.Length * _lineHeight + 2 * _padding;

            var image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
            image.Fill(Colors.White);

            // Create viewport for text rendering
            var viewport = new SubViewport();
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
            viewport.Size = new Vector2I(width, height);
            viewport.TransparentBg = true;

            // Add text label for each line
            var yPos = _padding;
            foreach (var line in lines)
            {
                var coloredLine = ApplySyntaxHighlighting(line, language);
                var label = new RichTextLabel
                {
                    Position = new Vector2(_padding, yPos),
                    Size = new Vector2(width - 2 * _padding, _lineHeight),
                    BbcodeEnabled = true,
                    Text = coloredLine
                };

                viewport.AddChild(label);
                yPos += _lineHeight;
            }

            // Get the rendered result
            var viewportTexture = viewport.GetTexture();
            var renderedImage = viewportTexture.GetImage();
            image.BlendRect(renderedImage, new Rect2I(0, 0, width, height), Vector2I.Zero);

            // Cleanup
            viewport.QueueFree();

            return image;
        }

        private int CalculateImageWidth(string[] lines)
        {
            int maxWidth = 0;
            foreach (var line in lines)
            {
                maxWidth = Math.Max(maxWidth, line.Length * (_fontSize / 2));
            }
            return maxWidth;
        }

        private string ApplySyntaxHighlighting(string line, string language)
        {
            // Common patterns
            var patterns = new (string pattern, Color color)[]
            {
                // Keywords
                (@"\b(public|private|protected|class|void|int|string|float|bool|var|return|if|else|for|while)\b", _keywordColor),
                // Strings
                (@"""[^""\\]*(?:\\.[^""\\]*)*""", _stringColor),
                // Comments
                (@"//.*$|/\*.*?\*/", _commentColor)
            };

            // Add language-specific patterns
            switch (language.ToLower())
            {
                case "cs":
                case "csharp":
                    patterns = patterns.Concat(new[]
                    {
                        (@"\b(namespace|using|static|readonly|override|partial|new)\b", _keywordColor)
                    }).ToArray();
                    break;

                case "py":
                case "python":
                    patterns = patterns.Concat(new[]
                    {
                        (@"\b(def|import|from|as|None|True|False)\b", _keywordColor),
                        (@"#.*$", _commentColor)
                    }).ToArray();
                    break;
            }

            // Apply syntax highlighting
            string result = line;
            foreach (var (pattern, color) in patterns)
            {
                result = Regex.Replace(
                    result,
                    pattern,
                    match => $"[color=#{ColorToHex(color)}]{match.Value}[/color]"
                );
            }

            return result;
        }

        private string ColorToHex(Color color)
        {
            return $"{(int)(color.R * 255):X2}{(int)(color.G * 255):X2}{(int)(color.B * 255):X2}";
        }
    }
}