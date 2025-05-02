using Godot;
using LineForge.Models;
using LineForge.Services;
using LineForge.UI;

public partial class Main : Control
{
    // Controllers
    private SettingsPanelController _settingsController;
    private AlgorithmPanelController _algorithmController;
    private TextPanelController _textController;

    // Services
    private PreviewService _previewService;
    private ExportService _exportService;

    // Current Settings
    private PaperSettings _paperSettings;
    private AlgorithmSettings _algorithmSettings;
    private TextSettings _textSettings;

    public override void _Ready()
    {
        GD.Print("Main scene ready.");
        InitializeControllers();
        InitializeServices();
        ConnectSignals();
    }

    private void InitializeControllers()
    {
        // Settings Panel
        _settingsController = new SettingsPanelController(
            GetNode<OptionButton>("%PaperSizeOptionButton"),
            GetNode<OptionButton>("%PenTypeOptionButton"),
            GetNode<ColorPickerButton>("%PenColorPickerButton"),
            GetNode<ColorPickerButton>("%PaperColorPickerButton"),
            GetNode<Button>("%InputModeImageButton"),
            GetNode<Button>("%InputModeCodeButton")
        );

        // Algorithm Panel
        var algoSections = new[]
        {
            (GetNode<Button>("%LineContoursHeader"), GetNode<VBoxContainer>("%LineContoursContent")),
            (GetNode<Button>("%VoronoiHeader"), GetNode<VBoxContainer>("%VoronoiContent")),
            (GetNode<Button>("%StipplingHeader"), GetNode<VBoxContainer>("%StipplingContent")),
            (GetNode<Button>("%PixelateHeader"), GetNode<VBoxContainer>("%PixelateContent"))
        };

        _algorithmController = new AlgorithmPanelController(
            algoSections,
            GetNode<HSlider>("%ContourThresholdSlider"),
            GetNode<HSlider>("%VoronoiPointsSlider"),
            GetNode<HSlider>("%StipplingDensitySlider"),
            GetNode<HSlider>("%PixelateSizeSlider")
        );

        // Text Panel
        _textController = new TextPanelController(
            GetNode<LineEdit>("%TextContentLineEdit"),
            GetNode<OptionButton>("%FontTypeOptionButton"),
            GetNode<SpinBox>("%SizeSpinBox"),
            GetNode<SpinBox>("%PositionXSpinBox"),
            GetNode<SpinBox>("%PositionYSpinBox"),
            GetNode<SpinBox>("%RotationSpinBox")
        );
    }

    private void InitializeServices()
    {
        _previewService = new PreviewService(GetNode<TextureRect>("%PreviewTextureRect"));
        _exportService = new ExportService();

        // Initialize settings
        _paperSettings = new PaperSettings();
        _algorithmSettings = new AlgorithmSettings();
        _textSettings = new TextSettings();
    }

    private void ConnectSignals()
    {
        // Connect settings changed events
        _settingsController.OnSettingsChanged += OnPaperSettingsChanged;
        _algorithmController.OnAlgorithmSettingsChanged += OnAlgorithmSettingsChanged;
        _textController.OnTextSettingsChanged += OnTextSettingsChanged;

        // Connect export buttons
        var saveSVGButton = GetNode<Button>("%SaveSVGButton");
        var exportGCodeButton = GetNode<Button>("%ExportGCodeButton");
        var effects3DButton = GetNode<Button>("%Effects3DButton");
        var rotateCanvasButton = GetNode<Button>("%RotateCanvasButton");

        saveSVGButton.Pressed += OnSaveSVGPressed;
        exportGCodeButton.Pressed += OnExportGCodePressed;
        effects3DButton.Pressed += OnEffects3DPressed;
        rotateCanvasButton.Pressed += OnRotateCanvasPressed;
    }

    // Event Handlers
    private void OnPaperSettingsChanged(PaperSettings settings)
    {
        _paperSettings = settings;
        UpdatePreview();
    }

    private void OnAlgorithmSettingsChanged(AlgorithmSettings settings)
    {
        _algorithmSettings = settings;
        UpdatePreview();
    }

    private void OnTextSettingsChanged(TextSettings settings)
    {
        _textSettings = settings;
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        _previewService.UpdatePreview(_paperSettings, _algorithmSettings, _textSettings);
    }

    // Export Actions
    private void OnSaveSVGPressed()
    {
        _exportService.ExportToSVG(_paperSettings, _algorithmSettings, _textSettings);
    }

    private void OnExportGCodePressed()
    {
        _exportService.ExportToGCode(_paperSettings, _algorithmSettings, _textSettings);
    }

    private void OnEffects3DPressed()
    {
        _exportService.Apply3DEffects();
    }

    private void OnRotateCanvasPressed()
    {
        _exportService.RotateCanvas(90); // Default 90-degree rotation
    }
}
