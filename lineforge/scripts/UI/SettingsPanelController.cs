using Godot;
using LineForge.Models;
using System.Linq;

namespace LineForge.UI
{
    public class SettingsPanelController
    {
        private readonly OptionButton _paperSizeOptionButton;
        private readonly OptionButton _penTypeOptionButton;
        private readonly ColorPickerButton _penColorPickerButton;
        private readonly ColorPickerButton _paperColorPickerButton;
        private readonly Button _inputModeImageButton;
        private readonly Button _inputModeCodeButton;
        private readonly PaperSettings _settings;

        public delegate void SettingsChangedEventHandler(PaperSettings settings);
        public event SettingsChangedEventHandler OnSettingsChanged;

        public SettingsPanelController(
            OptionButton paperSizeOptionButton,
            OptionButton penTypeOptionButton,
            ColorPickerButton penColorPickerButton,
            ColorPickerButton paperColorPickerButton,
            Button inputModeImageButton,
            Button inputModeCodeButton)
        {
            _paperSizeOptionButton = paperSizeOptionButton;
            _penTypeOptionButton = penTypeOptionButton;
            _penColorPickerButton = penColorPickerButton;
            _paperColorPickerButton = paperColorPickerButton;
            _inputModeImageButton = inputModeImageButton;
            _inputModeCodeButton = inputModeCodeButton;
            _settings = new PaperSettings();

            PopulateDropdowns();
            ConnectSignals();
        }

        private void PopulateDropdowns()
        {
            // Populate paper sizes
            _paperSizeOptionButton.Clear();
            foreach (var size in PaperSettings.PaperSizes.Keys.OrderBy(s => s))
            {
                _paperSizeOptionButton.AddItem(size);
            }
            _paperSizeOptionButton.Selected = _paperSizeOptionButton.GetItemIndex("A4");

            // Populate pen types
            _penTypeOptionButton.Clear();
            foreach (var penType in PaperSettings.PenTypes.Keys.OrderBy(p => p))
            {
                _penTypeOptionButton.AddItem(penType);
            }
            _penTypeOptionButton.Selected = _penTypeOptionButton.GetItemIndex("Sakura Micron 01");

            // Set default colors
            _penColorPickerButton.Color = Colors.Black;
            _paperColorPickerButton.Color = Colors.White;
        }

        private void ConnectSignals()
        {
            _paperSizeOptionButton.ItemSelected += OnPaperSizeSelected;
            _penTypeOptionButton.ItemSelected += OnPenTypeSelected;
            _penColorPickerButton.ColorChanged += OnPenColorChanged;
            _paperColorPickerButton.ColorChanged += OnPaperColorChanged;
        }

        private void OnPaperSizeSelected(long index)
        {
            _settings.Size = _paperSizeOptionButton.GetItemText((int)index);
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnPenTypeSelected(long index)
        {
            _settings.PenType = _penTypeOptionButton.GetItemText((int)index);
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnPenColorChanged(Color color)
        {
            _settings.PenColor = color;
            OnSettingsChanged?.Invoke(_settings);
        }

        private void OnPaperColorChanged(Color color)
        {
            _settings.PaperColor = color;
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetInputMode(bool isImageMode)
        {
            _inputModeImageButton.ButtonPressed = isImageMode;
            _inputModeCodeButton.ButtonPressed = !isImageMode;
        }
    }
}