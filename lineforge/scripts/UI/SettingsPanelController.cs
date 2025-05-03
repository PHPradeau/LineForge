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
            GD.Print("Populating paper size dropdown...");
            _paperSizeOptionButton.Clear();
            foreach (var size in PaperSettings.PaperSizes.Keys.OrderBy(s => s))
            {
                GD.Print($"Adding paper size: {size}");
                _paperSizeOptionButton.AddItem(size);
            }
            // Godot 4.x: GetItemIndex(string) does not exist, so find index manually
            int a4Index = GetOptionIndexByText(_paperSizeOptionButton, "A4");
            if (a4Index >= 0) _paperSizeOptionButton.Selected = a4Index;

            GD.Print("Populating pen type dropdown...");
            _penTypeOptionButton.Clear();
            foreach (var penType in PaperSettings.PenTypes.Keys.OrderBy(p => p))
            {
                GD.Print($"Adding pen type: {penType}");
                _penTypeOptionButton.AddItem(penType);
            }
            int micronIndex = GetOptionIndexByText(_penTypeOptionButton, "Sakura Micron 01");
            if (micronIndex >= 0) _penTypeOptionButton.Selected = micronIndex;

            // Set default colors
            _penColorPickerButton.Color = Colors.Black;
            _paperColorPickerButton.Color = Colors.White;
        }

        // Helper to find the index of an item by its text in an OptionButton
        private int GetOptionIndexByText(OptionButton optionButton, string text)
        {
            for (int i = 0; i < optionButton.ItemCount; i++)
            {
                if (optionButton.GetItemText(i) == text)
                    return i;
            }
            return -1;
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

        public PaperSettings GetCurrentSettings()
        {
            return _settings;
        }
    }
}