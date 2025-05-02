using Godot;
using LineForge.Models;

namespace LineForge.UI
{
    public class AlgorithmPanelController
    {
        private readonly Button[] _headers;
        private readonly VBoxContainer[] _contents;
        private readonly HSlider _contourThresholdSlider;
        private readonly HSlider _voronoiPointsSlider;
        private readonly HSlider _stipplingDensitySlider;
        private readonly HSlider _pixelateSizeSlider;
        private readonly AlgorithmSettings _settings;

        public delegate void AlgorithmSettingsChangedEventHandler(AlgorithmSettings settings);
        public event AlgorithmSettingsChangedEventHandler OnAlgorithmSettingsChanged;

        public AlgorithmPanelController(
            (Button header, VBoxContainer content)[] sections,
            HSlider contourThresholdSlider,
            HSlider voronoiPointsSlider,
            HSlider stipplingDensitySlider,
            HSlider pixelateSizeSlider)
        {
            _headers = sections.Select(s => s.header).ToArray();
            _contents = sections.Select(s => s.content).ToArray();
            _contourThresholdSlider = contourThresholdSlider;
            _voronoiPointsSlider = voronoiPointsSlider;
            _stipplingDensitySlider = stipplingDensitySlider;
            _pixelateSizeSlider = pixelateSizeSlider;
            _settings = new AlgorithmSettings();

            ConnectSignals();
        }

        private void ConnectSignals()
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                int index = i; // Capture for lambda
                _headers[i].Pressed += () => ToggleSectionVisibility(index);
            }

            _contourThresholdSlider.ValueChanged += OnContourThresholdChanged;
            _voronoiPointsSlider.ValueChanged += OnVoronoiPointsChanged;
            _stipplingDensitySlider.ValueChanged += OnStipplingDensityChanged;
            _pixelateSizeSlider.ValueChanged += OnPixelateSizeChanged;
        }

        private void ToggleSectionVisibility(int index)
        {
            _contents[index].Visible = !_contents[index].Visible;
            string text = _headers[index].Text;
            _headers[index].Text = _contents[index].Visible ? 
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
    }
}