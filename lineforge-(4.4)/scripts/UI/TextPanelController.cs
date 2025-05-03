using Godot;
using LineForge.Models;

namespace LineForge.UI
{
    public class TextPanelController
    {
        private readonly LineEdit _textContentLineEdit;
        private readonly OptionButton _fontTypeOptionButton;
        private readonly SpinBox _sizeSpinBox;
        private readonly SpinBox _positionXSpinBox;
        private readonly SpinBox _positionYSpinBox;
        private readonly SpinBox _rotationSpinBox;
        private readonly TextSettings _settings;

        public delegate void TextSettingsChangedEventHandler(TextSettings settings);
        public event TextSettingsChangedEventHandler OnSettingsChanged;

        public TextPanelController(
            LineEdit textContentLineEdit,
            OptionButton fontTypeOptionButton,
            SpinBox sizeSpinBox,
            SpinBox positionXSpinBox,
            SpinBox positionYSpinBox,
            SpinBox rotationSpinBox)
        {
            _textContentLineEdit = textContentLineEdit;
            _fontTypeOptionButton = fontTypeOptionButton;
            _sizeSpinBox = sizeSpinBox;
            _positionXSpinBox = positionXSpinBox;
            _positionYSpinBox = positionYSpinBox;
            _rotationSpinBox = rotationSpinBox;
            _settings = new TextSettings();

            PopulateFonts();
            ConnectSignals();
            InitializeDefaults();
        }

        private void PopulateFonts()
        {
            _fontTypeOptionButton.Clear();
            foreach (var font in TextSettings.DefaultFonts)
            {
                _fontTypeOptionButton.AddItem(font);
            }
            _fontTypeOptionButton.Selected = _fontTypeOptionButton.GetItemIndex("Arial");
        }

        private void ConnectSignals()
        {
            _textContentLineEdit.TextChanged += OnTextContentChanged;
            _fontTypeOptionButton.ItemSelected += OnFontSelected;
            _sizeSpinBox.ValueChanged += OnSizeChanged;
            _positionXSpinBox.ValueChanged += OnPositionChanged;
            _positionYSpinBox.ValueChanged += OnPositionChanged;
            _rotationSpinBox.ValueChanged += OnRotationChanged;
        }

        private void InitializeDefaults()
        {
            _sizeSpinBox.Value = _settings.FontSize;
            _positionXSpinBox.Value = _settings.Position.X;
            _positionYSpinBox.Value = _settings.Position.Y;
            _rotationSpinBox.Value = _settings.Rotation;
        }

        private void OnTextContentChanged(string newText)
        {
            _settings.Content = newText;
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnFontSelected(long index)
        {
            _settings.FontName = _fontTypeOptionButton.GetItemText((int)index);
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnSizeChanged(double value)
        {
            _settings.FontSize = (int)value;
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnPositionChanged(double value)
        {
            _settings.Position = new Vector2(
                (float)_positionXSpinBox.Value,
                (float)_positionYSpinBox.Value
            );
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnRotationChanged(double value)
        {
            _settings.Rotation = (float)value;
            OnSettingsChanged?.Invoke(_settings);
        }

        public TextSettings GetCurrentSettings()
        {
            return _settings;
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            _positionXSpinBox.Value = newPosition.X;
            _positionYSpinBox.Value = newPosition.Y;
            // This will trigger OnPositionChanged which will update settings and invoke the event
        }
    }
}