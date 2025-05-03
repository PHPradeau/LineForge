using Godot;
using LineForge.Models;
using LineForge.Services;
using LineForge.UI;

namespace LineForge
{
    public partial class Main : Control
    {
        // Controllers
        private SettingsPanelController _settingsPanelController;
        private AlgorithmPanelController _algorithmPanelController;
        private TextPanelController _textPanelController;

        // Services
        private PreviewService _previewService;
        private ExportService _exportService;
        private FileService _fileService;
        private TextRenderer _textRenderer;
        private Effects3DService _effects3DService;

        // Current Rotation
        private float _currentRotation = 0;

        public override void _Ready()
        {
            GD.Print("Main scene ready - starting initialization...");
            InitializeServices();
            InitializeControllers();
            ConnectSignals();
            GD.Print("Main scene initialization complete.");
        }

        private void InitializeServices()
        {
            GD.Print("Initializing services...");
            _previewService = new PreviewService();
            _textRenderer = new TextRenderer();
            _fileService = new FileService(_previewService);
            _exportService = new ExportService(_previewService);
            _effects3DService = new Effects3DService();

            // Set the preview texture rect
            var previewTextureRect = GetNode<TextureRect>("%PreviewTextureRect");
            _previewService.SetPreviewTextureRect(previewTextureRect);
            GD.Print("Services initialized.");
        }

        private void InitializeControllers()
        {
            GD.Print("Initializing controllers...");
            // Settings Panel
            var paperSizeOption = GetNode<OptionButton>("%PaperSizeOptionButton");
            var penTypeOption = GetNode<OptionButton>("%PenTypeOptionButton");
            var penColorPicker = GetNode<ColorPickerButton>("%PenColorPickerButton");
            var paperColorPicker = GetNode<ColorPickerButton>("%PaperColorPickerButton");
            var inputModeImage = GetNode<Button>("%InputModeImageButton");
            var inputModeCode = GetNode<Button>("%InputModeCodeButton");

            if (paperSizeOption == null) GD.PrintErr("paperSizeOption not found!");
            if (penTypeOption == null) GD.PrintErr("penTypeOption not found!");
            if (penColorPicker == null) GD.PrintErr("penColorPicker not found!");
            if (paperColorPicker == null) GD.PrintErr("paperColorPicker not found!");
            if (inputModeImage == null) GD.PrintErr("inputModeImage not found!");
            if (inputModeCode == null) GD.PrintErr("inputModeCode not found!");

            GD.Print("Creating SettingsPanelController...");
            _settingsPanelController = new SettingsPanelController(
                paperSizeOption,
                penTypeOption,
                penColorPicker,
                paperColorPicker,
                inputModeImage,
                inputModeCode
            );
            GD.Print("SettingsPanelController created.");

            // Algorithm Panel
            _algorithmPanelController = new AlgorithmPanelController(
                GetNode<Button>("%LineContoursHeader"),
                GetNode<VBoxContainer>("%LineContoursContent"),
                GetNode<HSlider>("%ContourThresholdSlider"),
                GetNode<Button>("%VoronoiHeader"),
                GetNode<VBoxContainer>("%VoronoiContent"),
                GetNode<HSlider>("%VoronoiPointsSlider"),
                GetNode<Button>("%StipplingHeader"),
                GetNode<VBoxContainer>("%StipplingContent"),
                GetNode<HSlider>("%StipplingDensitySlider"),
                GetNode<Button>("%PixelateHeader"),
                GetNode<VBoxContainer>("%PixelateContent"),
                GetNode<HSlider>("%PixelateSizeSlider")
            );

            // Text Panel
            _textPanelController = new TextPanelController(
                GetNode<LineEdit>("%TextContentLineEdit"),
                GetNode<OptionButton>("%FontTypeOptionButton"),
                GetNode<SpinBox>("%SizeSpinBox"),
                GetNode<SpinBox>("%PositionXSpinBox"),
                GetNode<SpinBox>("%PositionYSpinBox"),
                GetNode<SpinBox>("%RotationSpinBox")
            );
        }

        private void ConnectSignals()
        {
            // Connect file dialog buttons
            GetNode<Button>("%InputModeImageButton").Pressed += () => OnInputModeChanged(true);
            GetNode<Button>("%InputModeCodeButton").Pressed += () => OnInputModeChanged(false);

            // Connect export buttons
            GetNode<Button>("%SaveSVGButton").Pressed += () => _exportService.ExportToSVG(
                _settingsPanelController.GetCurrentSettings(),
                _algorithmPanelController.GetCurrentSettings(),
                _textPanelController.GetCurrentSettings()
            );

            GetNode<Button>("%ExportGCodeButton").Pressed += () => _exportService.ExportToGCode(
                _settingsPanelController.GetCurrentSettings(),
                _algorithmPanelController.GetCurrentSettings(),
                _textPanelController.GetCurrentSettings()
            );

            // Connect 3D effects button
            GetNode<Button>("%Effects3DButton").Pressed += () => _effects3DService.ApplyHeightMap(
                _previewService.GetPreviewImage(),
                _settingsPanelController.GetCurrentSettings()
            );

            // Connect canvas rotation button
            GetNode<Button>("%RotateCanvasButton").Pressed += OnRotateCanvas;

            // Connect settings changed events
            _settingsPanelController.OnSettingsChanged += (_) => UpdatePreview();
            _algorithmPanelController.OnSettingsChanged += (_) => UpdatePreview();
            _textPanelController.OnSettingsChanged += (_) => UpdatePreview();
        }

        private void OnInputModeChanged(bool isImageMode)
        {
            _settingsPanelController.SetInputMode(isImageMode);
            _fileService.ShowFileDialog(isImageMode);
        }

        private void OnRotateCanvas()
        {
            _currentRotation = (_currentRotation + 90) % 360;
            _previewService.RotatePreview(_currentRotation);
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var settings = _settingsPanelController.GetCurrentSettings();
            var algoSettings = _algorithmPanelController.GetCurrentSettings();
            var textSettings = _textPanelController.GetCurrentSettings();

            _previewService.UpdatePreview(settings, algoSettings);

            if (!string.IsNullOrEmpty(textSettings.Content))
            {
                var textImage = _textRenderer.RenderText(textSettings);
                _previewService.ApplyText(textImage, textSettings.Position);
            }
        }
    }
}
