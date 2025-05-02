using Godot;
using System;
using System.IO;

namespace LineForge.Services
{
    public class FileService
    {
        private readonly PreviewService _previewService;
        private readonly CodeVisualizer _codeVisualizer;
        private readonly FileDialog _fileDialog;

        public FileService(PreviewService previewService)
        {
            _previewService = previewService;
            _codeVisualizer = new CodeVisualizer();
            _fileDialog = new FileDialog
            {
                Access = FileDialog.AccessEnum.Filesystem,
                FileMode = FileDialog.FileModeEnum.OpenFile
            };

            ConnectSignals();
        }

        private void ConnectSignals()
        {
            _fileDialog.FileSelected += HandleFileSelected;
        }

        public void ShowFileDialog(bool isImageMode)
        {
            if (isImageMode)
            {
                _fileDialog.Filters = new[] { "*.png, *.jpg, *.jpeg, *.bmp" };
                _fileDialog.Title = "Open Image File";
            }
            else
            {
                _fileDialog.Filters = new[] { "*.cs, *.py, *.js, *.ts" };
                _fileDialog.Title = "Open Code File";
            }
            
            _fileDialog.Show();
        }

        private void HandleFileSelected(string path)
        {
            try
            {
                if (IsImageFile(path))
                {
                    LoadImageFile(path);
                }
                else if (IsCodeFile(path))
                {
                    LoadCodeFile(path);
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error loading file: {e.Message}");
            }
        }

        private void LoadImageFile(string path)
        {
            var image = Image.LoadFromFile(path);
            if (image != null)
            {
                _previewService.SetInputImage(image);
            }
        }

        private void LoadCodeFile(string path)
        {
            var code = File.ReadAllText(path);
            var language = Path.GetExtension(path).TrimStart('.').ToLower();
            var codeImage = _codeVisualizer.CreateCodeImage(code, language);
            _previewService.SetInputImage(codeImage);
        }

        private bool IsImageFile(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp";
        }

        private bool IsCodeFile(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return ext == ".cs" || ext == ".py" || ext == ".js" || ext == ".ts";
        }
    }
}