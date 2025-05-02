using Godot;
using System;

public partial class Main : Control
{
	// --- Node References ---
	// We use GetNode<T> in _Ready() for nodes added in the scene file.
	// Unique names (%) help ensure we get the correct node.

	// Left Panel
	private OptionButton _paperSizeOptionButton;
	private OptionButton _penTypeOptionButton;
	private ColorPickerButton _penColorPickerButton;
	private ColorPickerButton _paperColorPickerButton;
	private Button _inputModeImageButton;
	private Button _inputModeCodeButton;
	// InputAreaPlaceholder - might need specific nodes later

	// Center Preview
	private TextureRect _previewTextureRect;

	// Right Panel - Algo/FX
	private Button _lineContoursHeader;
	private VBoxContainer _lineContoursContent;
	private HSlider _contourThresholdSlider;

	private Button _voronoiHeader;
	private VBoxContainer _voronoiContent;
	private HSlider _voronoiPointsSlider;

	private Button _stipplingHeader;
	private VBoxContainer _stipplingContent;
	private HSlider _stipplingDensitySlider;

	private Button _pixelateHeader;
	private VBoxContainer _pixelateContent;
	private HSlider _pixelateSizeSlider;

	// Right Panel - Text Tool
	private LineEdit _textContentLineEdit;
	private OptionButton _fontTypeOptionButton;
	private SpinBox _sizeSpinBox;
	private SpinBox _positionXSpinBox;
	private SpinBox _positionYSpinBox;
	private SpinBox _rotationSpinBox;

	// Bottom Bar
	private Button _saveSVGButton;
	private Button _exportGCodeButton;
	private Button _effects3DButton;
	private Button _rotateCanvasButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Main scene ready.");

		// Get Node References using Unique Names (%)
		// Left Panel
		_paperSizeOptionButton = GetNode<OptionButton>("%PaperSizeOptionButton");
		_penTypeOptionButton = GetNode<OptionButton>("%PenTypeOptionButton");
		_penColorPickerButton = GetNode<ColorPickerButton>("%PenColorPickerButton");
		_paperColorPickerButton = GetNode<ColorPickerButton>("%PaperColorPickerButton");
		_inputModeImageButton = GetNode<Button>("%InputModeImageButton");
		_inputModeCodeButton = GetNode<Button>("%InputModeCodeButton");

		// Center Preview
		_previewTextureRect = GetNode<TextureRect>("%PreviewTextureRect");

		// Right Panel - Algo/FX
		_lineContoursHeader = GetNode<Button>("%LineContoursHeader");
		_lineContoursContent = GetNode<VBoxContainer>("%LineContoursContent");
		_contourThresholdSlider = GetNode<HSlider>("%ContourThresholdSlider");

		_voronoiHeader = GetNode<Button>("%VoronoiHeader");
		_voronoiContent = GetNode<VBoxContainer>("%VoronoiContent");
		_voronoiPointsSlider = GetNode<HSlider>("%VoronoiPointsSlider");

		_stipplingHeader = GetNode<Button>("%StipplingHeader");
		_stipplingContent = GetNode<VBoxContainer>("%StipplingContent");
		_stipplingDensitySlider = GetNode<HSlider>("%StipplingDensitySlider");

		_pixelateHeader = GetNode<Button>("%PixelateHeader");
		_pixelateContent = GetNode<VBoxContainer>("%PixelateContent");
		_pixelateSizeSlider = GetNode<HSlider>("%PixelateSizeSlider");

		// Right Panel - Text Tool
		_textContentLineEdit = GetNode<LineEdit>("%TextContentLineEdit");
		_fontTypeOptionButton = GetNode<OptionButton>("%FontTypeOptionButton");
		_sizeSpinBox = GetNode<SpinBox>("%SizeSpinBox");
		_positionXSpinBox = GetNode<SpinBox>("%PositionXSpinBox");
		_positionYSpinBox = GetNode<SpinBox>("%PositionYSpinBox");
		_rotationSpinBox = GetNode<SpinBox>("%RotationSpinBox");

		// Bottom Bar
		_saveSVGButton = GetNode<Button>("%SaveSVGButton");
		_exportGCodeButton = GetNode<Button>("%ExportGCodeButton");
		_effects3DButton = GetNode<Button>("%Effects3DButton");
		_rotateCanvasButton = GetNode<Button>("%RotateCanvasButton");

		// Connect Signals
		ConnectAlgoFXSignals();
		ConnectInputSignals();
		ConnectTextToolSignals();
		ConnectBottomBarSignals();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// --- Signal Connection Methods ---
	private void ConnectAlgoFXSignals()
	{
		_lineContoursHeader.Pressed += () => ToggleSectionVisibility(_lineContoursContent, _lineContoursHeader);
		_voronoiHeader.Pressed += () => ToggleSectionVisibility(_voronoiContent, _voronoiHeader);
		_stipplingHeader.Pressed += () => ToggleSectionVisibility(_stipplingContent, _stipplingHeader);
		_pixelateHeader.Pressed += () => ToggleSectionVisibility(_pixelateContent, _pixelateHeader);

		// Connect sliders value_changed signals (add methods later)
		_contourThresholdSlider.ValueChanged += OnContourThresholdChanged;
		_voronoiPointsSlider.ValueChanged += OnVoronoiPointsChanged;
		_stipplingDensitySlider.ValueChanged += OnStipplingDensityChanged;
		_pixelateSizeSlider.ValueChanged += OnPixelateSizeChanged;
	}

	private void ConnectInputSignals()
	{
		_paperSizeOptionButton.ItemSelected += OnPaperSizeSelected;
		_penTypeOptionButton.ItemSelected += OnPenTypeSelected;
		_penColorPickerButton.ColorChanged += OnPenColorChanged;
		_paperColorPickerButton.ColorChanged += OnPaperColorChanged;
		_inputModeImageButton.Pressed += () => SetInputMode(true);
		_inputModeCodeButton.Pressed += () => SetInputMode(false);
	}

	private void ConnectTextToolSignals()
	{
		_textContentLineEdit.TextChanged += OnTextContentChanged;
		_fontTypeOptionButton.ItemSelected += OnFontTypeSelected;
		_sizeSpinBox.ValueChanged += OnTextSizeChanged;
		_positionXSpinBox.ValueChanged += OnTextPositionChanged;
		_positionYSpinBox.ValueChanged += OnTextPositionChanged;
		_rotationSpinBox.ValueChanged += OnTextRotationChanged;
	}

	private void ConnectBottomBarSignals()
	{
		_saveSVGButton.Pressed += OnSaveSVGPressed;
		_exportGCodeButton.Pressed += OnExportGCodePressed;
		_effects3DButton.Pressed += OnEffects3DPressed;
		_rotateCanvasButton.Pressed += OnRotateCanvasPressed;
	}

	// --- Signal Handler Methods (Placeholders) ---

	// Algo/FX Section Toggling
	private void ToggleSectionVisibility(VBoxContainer sectionContent, Button headerButton)
	{
		sectionContent.Visible = !sectionContent.Visible;
		headerButton.Text = sectionContent.Visible ? headerButton.Text.Replace("▼", "▲") : headerButton.Text.Replace("▲", "▼");
	}

	// Algo/FX Sliders
	private void OnContourThresholdChanged(double value) { GD.Print($"Contour Threshold: {value}"); UpdatePreview(); }
	private void OnVoronoiPointsChanged(double value) { GD.Print($"Voronoi Points: {value}"); UpdatePreview(); }
	private void OnStipplingDensityChanged(double value) { GD.Print($"Stippling Density: {value}"); UpdatePreview(); }
	private void OnPixelateSizeChanged(double value) { GD.Print($"Pixelate Size: {value}"); UpdatePreview(); }

	// Input Section
	private void OnPaperSizeSelected(long index) { GD.Print($"Paper Size selected: index {index}, text {_paperSizeOptionButton.GetItemText((int)index)}"); UpdatePreview(); }
	private void OnPenTypeSelected(long index) { GD.Print($"Pen Type selected: index {index}, text {_penTypeOptionButton.GetItemText((int)index)}"); UpdatePreview(); }
	private void OnPenColorChanged(Color color) { GD.Print($"Pen Color changed: {color}"); UpdatePreview(); }
	private void OnPaperColorChanged(Color color) { GD.Print($"Paper Color changed: {color}"); UpdatePreview(); }
	private void SetInputMode(bool isImageMode)
	{
		GD.Print($"Input Mode set to: {(isImageMode ? "Image" : "Code")}");
		// Ensure only one button is pressed (basic toggle group logic)
		if (isImageMode)
		{
			_inputModeCodeButton.ButtonPressed = false;
			_inputModeImageButton.ButtonPressed = true; // Ensure it stays pressed if clicked again
		}
		else
		{
			_inputModeImageButton.ButtonPressed = false;
			_inputModeCodeButton.ButtonPressed = true;
		}
		// Add logic here to show/hide relevant input area (e.g., FileDialog or TextEdit)
	}

	// Text Tool
	private void OnTextContentChanged(string newText) { GD.Print($"Text Content: {newText}"); UpdatePreview(); }
	private void OnFontTypeSelected(long index) { GD.Print($"Font Type selected: index {index}"); UpdatePreview(); }
	private void OnTextSizeChanged(double value) { GD.Print($"Text Size: {value}"); UpdatePreview(); }
	private void OnTextPositionChanged(double value) { GD.Print($"Text Position: X={_positionXSpinBox.Value}, Y={_positionYSpinBox.Value}"); UpdatePreview(); }
	private void OnTextRotationChanged(double value) { GD.Print($"Text Rotation: {value}"); UpdatePreview(); }

	// Bottom Bar
	private void OnSaveSVGPressed() { GD.Print("Save SVG pressed"); /* Add save logic */ }
	private void OnExportGCodePressed() { GD.Print("Export G-code pressed"); /* Add export logic */ }
	private void OnEffects3DPressed() { GD.Print("3D Effects pressed"); /* Add 3D logic */ }
	private void OnRotateCanvasPressed() { GD.Print("Rotate Canvas pressed"); /* Add rotation logic */ }

	// --- Core Logic Placeholder ---
	private void UpdatePreview()
	{
		GD.Print("Updating Preview...");
		// This method will eventually:
		// 1. Get the current input (image or code)
		// 2. Get all the current settings (paper, pen, algo params, text params)
		// 3. Run the generation algorithm(s)
		// 4. Display the result in _previewTextureRect
	}
}
