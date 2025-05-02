using Godot;
using LineForge.Models;

namespace LineForge.Services
{
    public class PreviewService
    {
        private TextureRect _previewTextureRect;
        
        public PreviewService(TextureRect previewTextureRect)
        {
            _previewTextureRect = previewTextureRect;
        }

        public void UpdatePreview(PaperSettings paperSettings, AlgorithmSettings algoSettings, TextSettings textSettings)
        {
            GD.Print("Updating Preview...");
            // TODO: Implement preview generation logic
            // 1. Apply paper settings (size, colors)
            // 2. Apply algorithms based on settings
            // 3. Apply text overlay
            // 4. Update preview texture
        }
    }
}