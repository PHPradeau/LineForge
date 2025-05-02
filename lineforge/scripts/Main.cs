using Godot;
using System;

public partial class Main : Control
{
	// Node references - Left Panel
	private OptionButton _paperSizeOptionButton;
	private OptionButton _penTypeOptionButton;
	private ColorPickerButton _penColorPickerButton;
	private ColorPickerButton _paperColorPickerButton;
	private Button _inputModeImageButton;
	private Button _inputModeCodeButton;
	// Add references for InputArea, Right Panel, Center Preview, Bottom Bar later

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Main scene ready.");

		// Get node references using unique names or paths
		_paperSizeOptionButton = GetNode<OptionButton>("%PaperSizeOptionButton");
		_penTypeOptionButton = GetNode<OptionButton>("%PenTypeOptionButton");
		_penColorPickerButton = GetNode<ColorPickerButton>("%PenColorPickerButton");
		_paperColorPickerButton = GetNode<ColorPickerButton>("%PaperColorPickerButton");
		_inputModeImageButton = GetNode<Button>("%InputModeImageButton");
		_inputModeCodeButton = GetNode<Button>("%InputModeCodeButton");

		// Connect signals
		_paperSizeOptionButton.ItemSelected += _on_paper_size_selected;
		_penTypeOptionButton.ItemSelected += _on_pen_type_selected;
		_penColorPickerButton.ColorChanged += _on_pen_color_changed;
		_paperColorPickerButton.ColorChanged += _on_paper_color_changed;

		// Connect toggle buttons to the same handler, passing an identifier
		_inputModeImageButton.Toggled += (pressed) => _on_input_mode_toggled(pressed, "Image");
		_inputModeCodeButton.Toggled += (pressed) => _on_input_mode_toggled(pressed, "Code");

		// Ensure only one toggle is active initially (based on scene setup)
		_ensure_input_mode_exclusive(_inputModeImageButton.ButtonPressed ? "Image" : "Code");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// --- Signal Handlers ---

	private void _on_paper_size_selected(long index)
	{
		string selectedSize = _paperSizeOptionButton.GetItemText((int)index);
		GD.Print($"Paper size selected: {selectedSize} (Index: {index})");
		// Add logic to store/use this value
	}

	private void _on_pen_type_selected(long index)
	{
		string selectedType = _penTypeOptionButton.GetItemText((int)index);
		GD.Print($"Pen type selected: {selectedType} (Index: {index})");
		// Add logic to store/use this value
	}

	private void _on_pen_color_changed(Color color)
	{
		GD.Print($"Pen color changed: {color}");
		// Add logic to store/use this value
	}

	private void _on_paper_color_changed(Color color)
	{
		GD.Print($"Paper color changed: {color}");
		// Add logic to store/use this value
		// Maybe update the background color of the preview area?
	}

	private void _on_input_mode_toggled(bool pressed, string mode)
	{
		GD.Print($"Input mode toggled: {mode}, Pressed: {pressed}");
		// If a button is pressed, ensure the other is unpressed
		if (pressed)
		{
			_ensure_input_mode_exclusive(mode);
			// Add logic to show/hide relevant input area (Image upload vs Code TextEdit)
		}
		else
		{
			// Prevent both buttons from being unpressed simultaneously.
			// If the user tries to unpress the currently active button,
			// re-press it.
			if (mode == "Image" && !_inputModeCodeButton.ButtonPressed)
			{
				_inputModeImageButton.ButtonPressed = true;
			}
			else if (mode == "Code" && !_inputModeImageButton.ButtonPressed)
			{
				_inputModeCodeButton.ButtonPressed = true;
			}
		}
	}

	private void _ensure_input_mode_exclusive(string activeMode)
	{
		if (activeMode == "Image")
		{
			if (_inputModeCodeButton.ButtonPressed)
			{
				_inputModeCodeButton.ButtonPressed = false; // Turn off Code button
			}
			if (!_inputModeImageButton.ButtonPressed) // Ensure Image button is on if it should be
            {
                _inputModeImageButton.ButtonPressed = true;
            }
			GD.Print("Input Mode set to Image");
			// Show Image input controls, hide Code input controls
		}
		else // activeMode == "Code"
		{
			if (_inputModeImageButton.ButtonPressed)
			{
				_inputModeImageButton.ButtonPressed = false; // Turn off Image button
			}
			if (!_inputModeCodeButton.ButtonPressed) // Ensure Code button is on if it should be
            {
                _inputModeCodeButton.ButtonPressed = true;
            }
			GD.Print("Input Mode set to Code");
			// Show Code input controls, hide Image input controls
		}
	}
}
