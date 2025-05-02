using Godot;
using LineForge.Models;

namespace LineForge.Services
{
    public class Effects3DService
    {
        private readonly SubViewport _viewport3D;
        private MeshInstance3D _paperMesh;
        private Camera3D _camera;
        private DirectionalLight3D _light;

        public Effects3DService()
        {
            _viewport3D = new SubViewport
            {
                RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
                Size = new Vector2I(800, 600)
            };

            SetupScene();
        }

        private void SetupScene()
        {
            var scene = new Node3D();
            _viewport3D.AddChild(scene);

            // Add camera
            _camera = new Camera3D
            {
                Position = new Vector3(0, 10, 20),
                RotationDegrees = new Vector3(-30, 0, 0)
            };
            scene.AddChild(_camera);

            // Add light
            _light = new DirectionalLight3D
            {
                Position = new Vector3(10, 10, 5),
                LightEnergy = 1.5f,
                RotationDegrees = new Vector3(-45, 45, 0)
            };
            scene.AddChild(_light);

            // Create paper mesh
            _paperMesh = new MeshInstance3D();
            scene.AddChild(_paperMesh);
        }

        public void UpdateEffect(Image sourceImage, PaperSettings paperSettings)
        {
            var heightMap = GenerateHeightMap(sourceImage);
            var mesh = GeneratePaperMesh(heightMap, paperSettings);
            _paperMesh.Mesh = mesh;
        }

        private float[,] GenerateHeightMap(Image sourceImage)
        {
            int width = sourceImage.GetWidth();
            int height = sourceImage.GetHeight();
            var heightMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = sourceImage.GetPixel(x, y);
                    float brightness = (pixel.R + pixel.G + pixel.B) / 3.0f;
                    heightMap[x, y] = 1.0f - brightness; // Darker pixels = higher elevation
                }
            }

            return heightMap;
        }

        private ArrayMesh GeneratePaperMesh(float[,] heightMap, PaperSettings paperSettings)
        {
            var surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float scale = 0.1f; // Scale factor for height

            // Generate vertices and UVs
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    // Calculate positions for quad vertices
                    var p1 = new Vector3(x, heightMap[x, y] * scale, y);
                    var p2 = new Vector3(x + 1, heightMap[x + 1, y] * scale, y);
                    var p3 = new Vector3(x, heightMap[x, y + 1] * scale, y + 1);
                    var p4 = new Vector3(x + 1, heightMap[x + 1, y + 1] * scale, y + 1);

                    // Calculate UVs
                    var uv1 = new Vector2((float)x / width, (float)y / height);
                    var uv2 = new Vector2((float)(x + 1) / width, (float)y / height);
                    var uv3 = new Vector2((float)x / width, (float)(y + 1) / height);
                    var uv4 = new Vector2((float)(x + 1) / width, (float)(y + 1) / height);

                    // Add first triangle
                    surfaceTool.SetUV(uv1);
                    surfaceTool.AddVertex(p1);
                    surfaceTool.SetUV(uv2);
                    surfaceTool.AddVertex(p2);
                    surfaceTool.SetUV(uv3);
                    surfaceTool.AddVertex(p3);

                    // Add second triangle
                    surfaceTool.SetUV(uv2);
                    surfaceTool.AddVertex(p2);
                    surfaceTool.SetUV(uv4);
                    surfaceTool.AddVertex(p4);
                    surfaceTool.SetUV(uv3);
                    surfaceTool.AddVertex(p3);
                }
            }

            // Generate normals and create mesh
            surfaceTool.GenerateNormals();
            surfaceTool.GenerateTangents();
            
            // Create material
            var material = new StandardMaterial3D
            {
                AlbedoColor = paperSettings.PaperColor,
                RoughnessTexture = GenerateRoughnessTexture(heightMap),
                Metallic = 0.0f,
                Roughness = 0.8f
            };
            surfaceTool.SetMaterial(material);

            return surfaceTool.Commit();
        }

        private ImageTexture GenerateRoughnessTexture(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            
            var image = Image.Create(width, height, false, Image.Format.Rgb8);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float roughness = Mathf.Lerp(0.6f, 1.0f, heightMap[x, y]);
                    var color = new Color(roughness, roughness, roughness);
                    image.SetPixel(x, y, color);
                }
            }

            return ImageTexture.CreateFromImage(image);
        }

        public SubViewport GetViewport()
        {
            return _viewport3D;
        }

        public void RotateView(float degrees)
        {
            var currentRot = _paperMesh.RotationDegrees;
            _paperMesh.RotationDegrees = new Vector3(currentRot.X, currentRot.Y + degrees, currentRot.Z);
        }
    }
}