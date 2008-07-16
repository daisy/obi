using System;
using System.Drawing;

namespace Obi
{
    [Serializable()]
    public class ColorSettings
    {
        public Color BlockBackColor;
        public Color BlockForeColor;
        public Color BlockSelectedBackColor;
        public Color BlockSelectedForeColor;
        public Color BlockUnusedBackColor;
        public Color BlockUnusedForeColor;
        public Color ContentViewBackColor;
        public Color EditableLabelTextBackColor;
        public Color ProjectViewBackColor;
        public Color StripBackColor;
        public Color StripCursorSelectedBackColor;
        public Color StripForeColor;
        public Color StripSelectedBackColor;
        public Color StripSelectedForeColor;
        public Color StripUnusedBackColor;
        public Color StripUnusedForeColor;
        public Color TOCViewBackColor;
        public Color TOCViewForeColor;
        public Color TOCViewUnusedColor;
        public Color TransportBarBackColor;
        public Color WaveformBackColor;
        public Color WaveformHighlightedBackColor;

        private Color mBlockLayoutSelectedColor;
        private Color mWaveformBaseLineColor;
        private Color mWaveformChannel1Color;
        private Color mWaveformChannel2Color;
        private Color mWaveformCursorColor;
        private Color mWaveFormHighlightedForeColor;
        private Color mWaveformMonoColor;
        private Color mWaveformSelectionColor;

        [NonSerialized()] public SolidBrush BlockLayoutSelectedBrush;
        [NonSerialized()] public Pen WaveformBaseLinePen;
        [NonSerialized()] public Pen WaveformChannel1Pen;
        [NonSerialized()] public Pen WaveformChannel2Pen;
        [NonSerialized()] public Pen WaveformCursorPen;
        [NonSerialized()] public SolidBrush WaveformCursorBrush;
        [NonSerialized()] public Pen WaveformHighlightedPen;
        [NonSerialized()] public Pen WaveformMonoPen;
        [NonSerialized()] public SolidBrush WaveformSelectionBrush;
        [NonSerialized()] public Pen WaveformSelectionPen;

        /// <summary>
        /// Create the brushes and pens that were not serialized.
        /// </summary>
        public void CreateBrushesAndPens()
        {
            this.BlockLayoutSelectedBrush = new SolidBrush(this.mBlockLayoutSelectedColor);
            this.WaveformBaseLinePen = new Pen(this.mWaveformBaseLineColor);
            this.WaveformChannel1Pen = new Pen(this.mWaveformChannel1Color);
            this.WaveformChannel2Pen = new Pen(this.mWaveformChannel2Color);
            this.WaveformCursorPen = new Pen(this.mWaveformCursorColor);
            this.WaveformCursorBrush = new SolidBrush(this.mWaveformCursorColor);
            this.WaveformHighlightedPen = new Pen(this.mWaveFormHighlightedForeColor);
            this.WaveformMonoPen = new Pen(this.mWaveformMonoColor);
            this.WaveformSelectionBrush = new SolidBrush(this.mWaveformSelectionColor);
            this.WaveformSelectionPen = new Pen(this.mWaveformSelectionColor);
        }

        /// <summary>
        /// Get the default color settings.
        /// </summary>
        public static ColorSettings DefaultColorSettings()
        {
            ColorSettings settings = new ColorSettings();
            settings.BlockBackColor = SystemColors.Window;
            settings.mBlockLayoutSelectedColor = SystemColors.Highlight;
            settings.BlockForeColor = SystemColors.ControlText;
            settings.BlockSelectedBackColor = SystemColors.Highlight;
            settings.BlockSelectedForeColor = SystemColors.HighlightText;
            settings.BlockUnusedBackColor = SystemColors.ControlDark;
            settings.BlockUnusedForeColor = SystemColors.ControlText;
            settings.ContentViewBackColor = SystemColors.AppWorkspace;
            settings.EditableLabelTextBackColor = SystemColors.Window;
            settings.ProjectViewBackColor = SystemColors.Control;
            settings.StripBackColor = SystemColors.Control;
            settings.StripCursorSelectedBackColor = SystemColors.Highlight;
            settings.StripForeColor = SystemColors.ControlText;
            settings.StripSelectedBackColor = SystemColors.Highlight;
            settings.StripSelectedForeColor = SystemColors.HighlightText;
            settings.StripUnusedBackColor = SystemColors.ControlDark;
            settings.StripUnusedForeColor = SystemColors.ControlText;
            settings.TOCViewBackColor = SystemColors.Window;
            settings.TOCViewForeColor = SystemColors.ControlText;
            settings.TOCViewUnusedColor = SystemColors.InactiveCaptionText;
            settings.TransportBarBackColor = SystemColors.Control;
            settings.WaveformBackColor = SystemColors.Window;
            settings.mWaveformBaseLineColor = SystemColors.ControlText;
            settings.WaveformHighlightedBackColor = SystemColors.Highlight;
            settings.mWaveFormHighlightedForeColor = SystemColors.HighlightText;

            settings.mWaveformChannel1Color = Color.FromArgb(127, Color.Blue);
            settings.mWaveformChannel2Color = Color.FromArgb(127, Color.Red);
            settings.mWaveformMonoColor = Color.FromArgb(127, Color.Blue);
            settings.mWaveformSelectionColor = Color.FromArgb(127, Color.Green);
            settings.mWaveformCursorColor = Color.FromArgb(127, Color.Orange);

            return settings;
        }

        /// <summary>
        /// Get the default color settings for high-contrast mode.
        /// </summary>
        public static ColorSettings DefaultColorSettingsHC()
        {
            ColorSettings settings = DefaultColorSettings();
            // we do not need to change system colors.

            settings.mWaveformChannel1Color = Color.Green;
            settings.mWaveformChannel2Color = Color.Green;
            settings.mWaveformMonoColor = Color.Green;
            settings.mWaveformSelectionColor = Color.FromArgb(127, Color.Black);
            settings.mWaveformCursorColor = Color.Purple;
            
            return settings;
        }
    }
}
