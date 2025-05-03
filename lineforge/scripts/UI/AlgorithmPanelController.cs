using Godot;
using LineForge.Models;
using System.Linq;

namespace LineForge.UI
{
    public class AlgorithmPanelController
    {
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
        private readonly AlgorithmSettings _settings;

        public delegate void AlgorithmSettingsChangedEventHandler(AlgorithmSettings settings);
        public event AlgorithmSettingsChangedEventHandler OnAlgorithmSettingsChanged;

        public AlgorithmPanelController(
            Button lineContoursHeader,
            VBoxContainer lineContoursContent,
            HSlider contourThresholdSlider,
            Button voronoiHeader,
            VBoxContainer voronoiContent,
            HSlider voronoiPointsSlider,
            Button stipplingHeader,
            VBoxContainer stipplingContent,
            HSlider stipplingDensitySlider,
            Button pixelateHeader,
            VBoxContainer pixelateContent,
            HSlider pixelateSizeSlider)
        {
            _lineContoursHeader = lineContoursHeader;
            _lineContoursContent = lineContoursContent;
            _contourThresholdSlider = contourThresholdSlider;
            _voronoiHeader = voronoiHeader;
            _voronoiContent = voronoiContent;
            _voronoiPointsSlider = voronoiPointsSlider;
            _stipplingHeader = stipplingHeader;
            _stipplingContent = stipplingContent;
            _stipplingDensitySlider = stipplingDensitySlider;
            _pixelateHeader = pixelateHeader;
            _pixelateContent = pixelateContent;
            _pixelateSizeSlider = pixelateSizeSlider;
            _settings = new AlgorithmSettings();

            ConnectSignals();
        }

        private void ConnectSignals()
        {
            _lineContoursHeader.Pressed += () => ToggleSectionVisibility(_lineContoursContent, _lineContoursHeader);
            _voronoiHeader.Pressed += () => ToggleSectionVisibility(_voronoiContent, _voronoiHeader);
            _stipplingHeader.Pressed += () => ToggleSectionVisibility(_stipplingContent, _stipplingHeader);
            _pixelateHeader.Pressed += () => ToggleSectionVisibility(_pixelateContent, _pixelateHeader);

            _contourThresholdSlider.ValueChanged += OnContourThresholdChanged;
            _voronoiPointsSlider.ValueChanged += OnVoronoiPointsChanged;
            _stipplingDensitySlider.ValueChanged += OnStipplingDensityChanged;
            _pixelateSizeSlider.ValueChanged += OnPixelateSizeChanged;
        }

        private void ToggleSectionVisibility(VBoxContainer content, Button header)
        {
            content.Visible = !content.Visible;
            string text = header.Text;
            header.Text = content.Visible ? 
                text.Replace("▼", "▲") : 
                text.Replace("▲", "▼");
        }

        private void OnContourThresholdChanged(double value)
        {
            _settings.ContourThreshold = value;
            OnAlgorithmSettingsChanged?.Invoke(_settings);
        }

        private void OnVoronoiPointsChanged(double value)
        {
            _settings.VoronoiPoints = (int)value;
            OnAlgorithmSettingsChanged?.Invoke(_settings);
        }

        private void OnStipplingDensityChanged(double value)
        {
            _settings.StipplingDensity = value;
            OnAlgorithmSettingsChanged?.Invoke(_settings);
        }

        private void OnPixelateSizeChanged(double value)
        {
            _settings.PixelateSize = (int)value;
            OnAlgorithmSettingsChanged?.Invoke(_settings);
        }

        public AlgorithmSettings GetCurrentSettings()
        {
            return _settings;
        }
    }
}