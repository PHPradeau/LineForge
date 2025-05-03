using Godot;
using LineForge.Models;

namespace LineForge.Services
{
    public class Effects3DService
    {
        private readonly Material _heightMapMaterial;
        private readonly ShaderMaterial _lightingMaterial;
        private Image _currentHeightMap;

        public Effects3DService()
        {
            // Initialize height map material with default properties
            _heightMapMaterial = new StandardMaterial3D
            {
                RoughnessTexture = new NoiseTexture2D
                {
                    Width = 512,
                    Height = 512,
                    Seamless = true,
                    AsNormalMap = true
                },
                Metallic = 0.0f,
                Roughness = 0.8f
            };

            // Initialize lighting material with custom shader for dynamic lighting
            _lightingMaterial = new ShaderMaterial();
            var shader = GD.Load<Shader>("res://shaders/lighting.gdshader");
            _lightingMaterial.Shader = shader;
        }

        public void ApplyHeightMap(Image sourceImage, PaperSettings settings)
        {
            // Generate height map based on image intensity
            _currentHeightMap = sourceImage.Duplicate();
            _currentHeightMap.Convert(Image.Format.L8); // Convert to grayscale

            // Adjust height map based on pen properties
            var penProps = settings.GetPenProperties();
            float heightScale = penProps.Pressure * penProps.Width;
            
            for (int y = 0; y < _currentHeightMap.GetHeight(); y++)
            {
                for (int x = 0; x < _currentHeightMap.GetWidth(); x++)
                {
                    var color = _currentHeightMap.GetPixel(x, y);
                    var height = color.R * heightScale;
                    _currentHeightMap.SetPixel(x, y, new Color(height, height, height));
                }
            }

            // Update material properties
            ((StandardMaterial3D)_heightMapMaterial).HeightmapTexture = ImageTexture.CreateFromImage(_currentHeightMap);
        }

        public void UpdateLighting(Vector3 lightDirection, Color lightColor)
        {
            _lightingMaterial.SetShaderParameter("light_direction", lightDirection.Normalized());
            _lightingMaterial.SetShaderParameter("light_color", lightColor);
            _lightingMaterial.SetShaderParameter("height_map", _currentHeightMap);
        }

        public Material GetHeightMapMaterial()
        {
            return _heightMapMaterial;
        }

        public ShaderMaterial GetLightingMaterial()
        {
            return _lightingMaterial;
        }
    }
}