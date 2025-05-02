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
        public event TextSettingsChangedEventHandler OnTextSettingsChanged;

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

            ConnectSignals();
        }

        private void ConnectSignals()
        {
            _textContentLineEdit.TextChanged += OnTextContentChanged;
            _fontTypeOptionButton.ItemSelected += OnFontTypeSelected;
            _sizeSpinBox.ValueChanged += OnSizeChanged;
            _positionXSpinBox.ValueChanged += OnPositionChanged;
            _positionYSpinBox.ValueChanged += OnPositionChanged;
            _rotationSpinBox.ValueChanged += OnRotationChanged;
        }

        private void OnTextContentChanged(string newText)
        {
            _settings.Content = newText;
            OnTextSettingsChanged?.Invoke(_settings);
        }

        private void OnFontTypeSelected(long index)
        {
            _settings.FontType = _fontTypeOptionButton.GetItemText((int)index);
            OnTextSettingsChanged?.Invoke(_settings);
        }

        private void OnSizeChanged(double value)
        {
            _settings.Size = value;
            OnTextSettingsChanged?.Invoke(_settings);
        }

        private void OnPositionChanged(double _)
        {
            _settings.Position = new Vector2(
                (float)_positionXSpinBox.Value,
                (float)_positionYSpinBox.Value
            );
            OnTextSettingsChanged?.Invoke(_settings);
        }

        private void OnRotationChanged(double value)
        {
            _settings.Rotation = value;
            OnTextSettingsChanged?.Invoke(_settings);
        }
    }
}