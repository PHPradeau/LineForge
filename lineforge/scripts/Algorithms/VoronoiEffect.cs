using Godot;
using System;
using System.Collections.Generic;

namespace LineForge.Algorithms
{
    public class VoronoiEffect : IImageEffect
    {
        private readonly int _pointCount;
        private readonly Random _random;

        public VoronoiEffect(int pointCount)
        {
            _pointCount = pointCount;
            _random = new Random();
        }

        public Image ProcessImage(Image input)
        {
            var output = Image.Create(input.GetWidth(), input.GetHeight(), false, Image.Format.Rgba8);
            var points = GenerateRandomPoints(input.GetWidth(), input.GetHeight());
            var colors = GenerateRandomColors();

            // For each pixel, find the closest point and color accordingly
            for (int x = 0; x < input.GetWidth(); x++)
            {
                for (int y = 0; y < input.GetHeight(); y++)
                {
                    int closestPointIndex = FindClosestPoint(new Vector2(x, y), points);
                    output.SetPixel(x, y, colors[closestPointIndex]);
                }
            }

            return output;
        }

        private List<Vector2> GenerateRandomPoints(int width, int height)
        {
            var points = new List<Vector2>();
            for (int i = 0; i < _pointCount; i++)
            {
                float x = (float)_random.NextDouble() * width;
                float y = (float)_random.NextDouble() * height;
                points.Add(new Vector2(x, y));
            }
            return points;
        }

        private List<Color> GenerateRandomColors()
        {
            var colors = new List<Color>();
            for (int i = 0; i < _pointCount; i++)
            {
                colors.Add(new Color(
                    (float)_random.NextDouble(),
                    (float)_random.NextDouble(),
                    (float)_random.NextDouble()
                ));
            }
            return colors;
        }

        private int FindClosestPoint(Vector2 pixel, List<Vector2> points)
        {
            float minDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < points.Count; i++)
            {
                float distance = pixel.DistanceTo(points[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }
    }
}