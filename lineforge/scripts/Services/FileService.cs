using Godot;
using System;
using System.IO;

namespace LineForge.Services
{
    public class FileService
    {
        private readonly PreviewService _previewService;

        public FileService(PreviewService previewService)
        {
            _previewService = previewService;
        }

        public void LoadImage(string filePath)
        {
            try
            {
                var image = Image.LoadFromFile(filePath);
                if (image != null)
                {
                    _previewService.SetInputImage(image);
                }
                else
                {
                    GD.PrintErr($"Failed to load image: {filePath}");
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error loading image: {e.Message}");
            }
        }

        public void LoadCode(string filePath)
        {
            try
            {
                string code = File.ReadAllText(filePath);
                // TODO: Parse code and convert to image/preview
                GD.Print($"Code loaded: {code.Length} characters");
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error loading code: {e.Message}");
            }
        }

        public async void ShowFileDialog(bool isImageMode)
        {
            var fileDialog = new FileDialog();
            fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
            fileDialog.Access = FileDialog.AccessEnum.Filesystem;
            
            if (isImageMode)
            {
                fileDialog.Filters = new string[] { "*.png", "*.jpg", "*.jpeg", "*.svg" };
            }
            else
            {
                fileDialog.Filters = new string[] { "*.txt", "*.cs", "*.js", "*.py" };
            }

            fileDialog.FileSelected += (string path) =>
            {
                if (isImageMode)
                {
                    LoadImage(path);
                }
                else
                {
                    LoadCode(path);
                }
            };

            fileDialog.Show();
        }
    }
}