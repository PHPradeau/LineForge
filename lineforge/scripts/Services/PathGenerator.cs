using Godot;
using System.Collections.Generic;
using System.Text;

namespace LineForge.Services
{
    public class PathGenerator
    {
        private readonly bool[,] _visited;
        private readonly Image _image;
        private readonly float _threshold;

        public PathGenerator(Image image, float threshold = 0.5f)
        {
            _image = image;
            _threshold = threshold;
            _visited = new bool[image.GetWidth(), image.GetHeight()];
        }

        public List<string> GenerateSVGPaths()
        {
            var paths = new List<string>();
            ResetVisited();

            for (int y = 0; y < _image.GetHeight(); y++)
            {
                for (int x = 0; x < _image.GetWidth(); x++)
                {
                    if (!_visited[x, y] && IsEdgePixel(x, y))
                    {
                        var contour = TraceContour(x, y);
                        paths.Add(ContourToSVGPath(contour));
                    }
                }
            }

            return paths;
        }

        public List<List<Vector2>> GeneratePlotterPaths()
        {
            var paths = new List<List<Vector2>>();
            ResetVisited();

            for (int y = 0; y < _image.GetHeight(); y++)
            {
                for (int x = 0; x < _image.GetWidth(); x++)
                {
                    if (!_visited[x, y] && IsEdgePixel(x, y))
                    {
                        paths.Add(TraceContour(x, y));
                    }
                }
            }

            return paths;
        }

        private void ResetVisited()
        {
            for (int x = 0; x < _image.GetWidth(); x++)
                for (int y = 0; y < _image.GetHeight(); y++)
                    _visited[x, y] = false;
        }

        private bool IsEdgePixel(int x, int y)
        {
            Color pixel = _image.GetPixel(x, y);
            float brightness = (pixel.R + pixel.G + pixel.B) / 3.0f;
            return brightness < _threshold;
        }

        private List<Vector2> TraceContour(int startX, int startY)
        {
            var contour = new List<Vector2>();
            var current = new Vector2(startX, startY);
            var direction = 0; // 0=right, 1=down, 2=left, 3=up

            do
            {
                contour.Add(current);
                _visited[(int)current.X, (int)current.Y] = true;

                // Moore neighborhood tracing
                bool found = false;
                int count = 0;
                while (!found && count < 8)
                {
                    var next = GetNextPixel(current, direction);
                    if (IsValidPixel(next) && IsEdgePixel((int)next.X, (int)next.Y))
                    {
                        current = next;
                        found = true;
                    }
                    else
                    {
                        direction = (direction + 1) % 4;
                        count++;
                    }
                }

                if (!found) break;

            } while ((int)current.X != startX || (int)current.Y != startY);

            return contour;
        }

        private Vector2 GetNextPixel(Vector2 current, int direction)
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

        private bool IsValidPixel(Vector2 pos)
        {
            return pos.X >= 0 && pos.X < _image.GetWidth() &&
                   pos.Y >= 0 && pos.Y < _image.GetHeight();
        }

        private string ContourToSVGPath(List<Vector2> contour)
        {
            if (contour.Count == 0) return "";

            var path = new StringBuilder();
            path.Append($"M {contour[0].X} {contour[0].Y}");

            for (int i = 1; i < contour.Count; i++)
            {
                path.Append($" L {contour[i].X} {contour[i].Y}");
            }

            path.Append("Z");
            return path.ToString();
        }
    }
}