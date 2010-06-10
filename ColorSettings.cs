using System;
using System.Drawing;

namespace Obi
{
    [Serializable()]
    public class ColorSettings
    {
        public Color BlockBackColor_Custom;
        public Color BlockBackColor_Empty;
        public Color BlockBackColor_Heading;
        public Color BlockBackColor_Page;
        public Color BlockBackColor_Plain;
        public Color BlockBackColor_Selected;
        public Color BlockBackColor_Silence;
        public Color BlockBackColor_TODO;
        public Color BlockBackColor_Unused;

        public Color BlockForeColor_Custom;
        public Color BlockForeColor_Empty;
        public Color BlockForeColor_Heading;
        public Color BlockForeColor_Page;
        public Color BlockForeColor_Plain;
        public Color BlockForeColor_Selected;
        public Color BlockForeColor_Silence;
        public Color BlockForeColor_TODO;
        public Color BlockForeColor_Unused;

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
        public Color StripWithoutPhrasesBackcolor;
        public Color TOCViewBackColor;
        public Color TOCViewForeColor;
        public Color TOCViewUnusedColor;
        public Color ToolTipForeColor;
        public Color TransportBarBackColor;
        public Color TransportBarLabelBackColor;
        public Color TransportBarLabelForeColor;
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
        [NonSerialized()] public SolidBrush WaveformHighlightedTextBrush;
        [NonSerialized()] public Pen WaveformMonoPen;
        [NonSerialized()] public SolidBrush WaveformSelectionBrush;
        [NonSerialized()] public Pen WaveformSelectionPen;
        [NonSerialized()] public SolidBrush WaveformTextBrush;

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
            this.WaveformHighlightedTextBrush = new SolidBrush(this.mWaveFormHighlightedForeColor);
            this.WaveformMonoPen = new Pen(this.mWaveformMonoColor);
            this.WaveformSelectionBrush = new SolidBrush(this.mWaveformSelectionColor);
            this.WaveformSelectionPen = new Pen(this.mWaveformSelectionColor);
            this.WaveformTextBrush = new SolidBrush(this.mWaveformBaseLineColor);
        }

        /// <summary>
        /// Get the default color settings.
        /// </summary>
        public static ColorSettings DefaultColorSettings()
        {
            ColorSettings settings = new ColorSettings();
            settings.BlockBackColor_Custom = Color.Orange;
            settings.BlockBackColor_Empty = Color.LightSkyBlue;
            settings.BlockBackColor_Heading = Color.LightGreen;
            settings.BlockBackColor_Page = Color.LightSalmon;
            settings.BlockBackColor_Plain = SystemColors.Window;
            settings.BlockBackColor_Silence = Color.Purple;
            settings.BlockBackColor_Selected = SystemColors.Highlight;
            settings.BlockBackColor_TODO = Color.Red;
            settings.BlockBackColor_Unused = SystemColors.ControlDark;

            settings.BlockForeColor_Custom = SystemColors.HighlightText;
            settings.BlockForeColor_Empty = SystemColors.ControlText;
            settings.BlockForeColor_Heading = SystemColors.ControlText;
            settings.BlockForeColor_Page = SystemColors.ControlText;
            settings.BlockForeColor_Plain = SystemColors.ControlText;
            settings.BlockForeColor_Selected = SystemColors.HighlightText;
            settings.BlockForeColor_Silence = SystemColors.HighlightText;
            settings.BlockForeColor_TODO = Color.Yellow;
            settings.BlockForeColor_Unused = SystemColors.HighlightText;

            settings.mBlockLayoutSelectedColor = SystemColors.Highlight;
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
            settings.StripWithoutPhrasesBackcolor = SystemColors.GradientActiveCaption;
            settings.TOCViewBackColor = SystemColors.Window;
            settings.TOCViewForeColor = SystemColors.ControlText;
            settings.TOCViewUnusedColor = SystemColors.InactiveCaptionText;
            settings.ToolTipForeColor = SystemColors.ControlText;
            settings.TransportBarBackColor = SystemColors.Control;
            settings.TransportBarLabelBackColor = Color.Azure;
            settings.TransportBarLabelForeColor = SystemColors.ControlText;
            settings.WaveformBackColor = SystemColors.Window;
            settings.mWaveformBaseLineColor = SystemColors.ControlText;
            settings.WaveformHighlightedBackColor = SystemColors.Highlight;
            settings.mWaveFormHighlightedForeColor = SystemColors.HighlightText;

            settings.mWaveformChannel1Color = Color.FromArgb(127, Color.Blue);
            settings.mWaveformChannel2Color = Color.FromArgb(127, Color.Red);
            settings.mWaveformMonoColor = Color.FromArgb(127, Color.Blue);
            settings.mWaveformSelectionColor = SystemColors.Highlight;
            settings.mWaveformCursorColor = Color.Red;
            return settings;
        }

        /// <summary>
        /// Get the default color settings for high-contrast mode.
        /// </summary>
        public static ColorSettings DefaultColorSettingsHC()
        {
            ColorSettings settings = DefaultColorSettings();

            // Blocks don't change color much in high contrast settings.
            settings.BlockBackColor_Custom = SystemColors.Window;
            settings.BlockBackColor_Empty = SystemColors.Window;
            settings.BlockBackColor_Heading = SystemColors.Window;
            settings.BlockBackColor_Page = SystemColors.Window;
            settings.BlockBackColor_Silence = SystemColors.Window;
            settings.BlockBackColor_TODO = SystemColors.Window;

            settings.BlockForeColor_Custom = SystemColors.ControlText;
            settings.BlockForeColor_Empty = SystemColors.ControlText;
            settings.BlockForeColor_Heading = SystemColors.ControlText;
            settings.BlockForeColor_Page = SystemColors.ControlText;
            settings.BlockForeColor_TODO = SystemColors.ControlText;

            settings.TransportBarLabelBackColor = Color.DarkSlateGray;
            settings.mWaveformChannel1Color = Color.Green;
            settings.mWaveformChannel2Color = Color.Green;
            settings.mWaveformMonoColor = Color.Green;
            settings.mWaveformCursorColor = Color.Yellow;
            
            return settings;
        }
    }
}
