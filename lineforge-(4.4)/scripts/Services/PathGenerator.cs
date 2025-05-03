using Godot;
using System.Collections.Generic;

namespace LineForge.Services
{
    public class PathGenerator
    {
        private readonly Image _image;
        private bool[,] _visited;

        public PathGenerator(Image image)
        {
            _image = image;
        }

        public List<string> GenerateSVGPaths()
        {
            var paths = new List<string>();
            _visited = new bool[_image.GetWidth(), _image.GetHeight()];

            // Find contours and convert them to SVG paths
            for (int y = 0; y < _image.GetHeight(); y++)
            {
                for (int x = 0; x < _image.GetWidth(); x++)
                {
                    if (!_visited[x, y] && IsBlackPixel(_image.GetPixel(x, y)))
                    {
                        var contour = TraceContour(x, y);
                        if (contour.Count > 0)
                        {
                            paths.Add(ConvertToSVGPath(contour));
                        }
                    }
                }
            }

            return paths;
        }

        public List<List<Vector2>> GeneratePlotterPaths()
        {
            var paths = new List<List<Vector2>>();
            _visited = new bool[_image.GetWidth(), _image.GetHeight()];

            // Find contours and convert them to plotter coordinates
            for (int y = 0; y < _image.GetHeight(); y++)
            {
                for (int x = 0; x < _image.GetWidth(); x++)
                {
                    if (!_visited[x, y] && IsBlackPixel(_image.GetPixel(x, y)))
                    {
                        var contour = TraceContour(x, y);
                        if (contour.Count > 0)
                        {
                            paths.Add(contour);
                        }
                    }
                }
            }

            return paths;
        }

        private List<Vector2> TraceContour(int startX, int startY)
        {
            var contour = new List<Vector2>();
            var current = new Vector2(startX, startY);
            var direction = 0; // 0: right, 1: down, 2: left, 3: up

            do
            {
                contour.Add(current);
                _visited[(int)current.X, (int)current.Y] = true;

                // Moore neighborhood tracing
                bool found = false;
                int initialDirection = direction;

                do
                {
                    var next = GetNextPoint(current, direction);
                    if (IsValidPoint(next) && IsBlackPixel(_image.GetPixel((int)next.X, (int)next.Y)))
                    {
                        current = next;
                        found = true;
                        break;
                    }

                    direction = (direction + 1) % 4;
                } while (direction != initialDirection);

                if (!found) break;

            } while (current != new Vector2(startX, startY) && contour.Count < 10000); // Safety limit

            return contour;
        }

        private Vector2 GetNextPoint(Vector2 current, int direction)
        {
            return direction switch
            {
                0 => current + Vector2.Right,
                1 => current + Vector2.Down,
                2 => current + Vector2.Left,
                3 => current + Vector2.Up,
                _ => current
            };
        }

        private bool IsValidPoint(Vector2 point)
        {
            return point.X >= 0 && point.X < _image.GetWidth() &&
                   point.Y >= 0 && point.Y < _image.GetHeight();
        }

        private bool IsBlackPixel(Color color)
        {
            const float threshold = 0.5f;
            return (color.R + color.G + color.B) / 3.0f < threshold;
        }

        private string ConvertToSVGPath(List<Vector2> points)
        {
            if (points.Count == 0) return "";

            var path = new System.Text.StringBuilder();
            path.Append($"M {points[0].X},{points[0].Y}");

            for (int i = 1; i < points.Count; i++)
            {
                path.Append($" L {points[i].X},{points[i].Y}");
            }

            path.Append(" Z"); // Close the path
            return path.ToString();
        }
    }
}