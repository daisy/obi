using System;
using System.Drawing;
using System.Collections.Generic;

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
        public Color BlockBackColor_Anchor;

        public Color BlockForeColor_Custom;
        public Color BlockForeColor_Empty;
        public Color BlockForeColor_Heading;
        public Color BlockForeColor_Page;
        public Color BlockForeColor_Plain;
        public Color BlockForeColor_Selected;
        public Color BlockForeColor_Silence;
        public Color BlockForeColor_TODO;
        public Color BlockForeColor_Anchor;
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
        public Color BlockLayoutSelectedColor;
        public Color WaveformBaseLineColor;

        private Color mWaveformChannel1Color;
        private Color mWaveformChannel2Color;
        private Color mWaveformCursorColor;
        private Color mWaveFormHighlightedForeColor;
        private Color mWaveformMonoColor;
        private Color mWaveformSelectionColor;
        public Color FineNavigationColor;
        public Color RecordingHighlightPhraseColor;
        public Color EmptySectionBackgroundColor;
        public Color HighlightedSectionNodeWithoutSelectionColor;

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
        [NonSerialized()] public Dictionary<string, Color> ColorSetting;
        /// <summary>
        /// Create the brushes and pens that were not serialized.
        /// </summary>
        public void CreateBrushesAndPens()
        {
            this.BlockLayoutSelectedBrush = new SolidBrush(this.BlockLayoutSelectedColor);
            this.WaveformBaseLinePen = new Pen(this.WaveformBaseLineColor);
            this.WaveformChannel1Pen = new Pen(this.mWaveformChannel1Color);
            this.WaveformChannel2Pen = new Pen(this.mWaveformChannel2Color);
            this.WaveformCursorPen = new Pen(this.mWaveformCursorColor);
            this.WaveformCursorBrush = new SolidBrush(this.mWaveformCursorColor);
            this.WaveformHighlightedPen = new Pen(this.mWaveFormHighlightedForeColor);
            this.WaveformHighlightedTextBrush = new SolidBrush(this.mWaveFormHighlightedForeColor);
            this.WaveformMonoPen = new Pen(this.mWaveformMonoColor);
            this.WaveformSelectionBrush = new SolidBrush(this.mWaveformSelectionColor);
            this.WaveformSelectionPen = new Pen(this.mWaveformSelectionColor);
            this.WaveformTextBrush = new SolidBrush(this.WaveformBaseLineColor);
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
            settings.BlockBackColor_Anchor = Color.Chocolate;
            settings.BlockBackColor_Unused = SystemColors.ControlDark;

            settings.BlockForeColor_Custom = SystemColors.HighlightText;
            settings.BlockForeColor_Empty = SystemColors.ControlText;
            settings.BlockForeColor_Heading = SystemColors.ControlText;
            settings.BlockForeColor_Page = SystemColors.ControlText;
            settings.BlockForeColor_Plain = SystemColors.ControlText;
            settings.BlockForeColor_Selected = SystemColors.HighlightText;
            settings.BlockForeColor_Silence = SystemColors.HighlightText;
            settings.BlockForeColor_TODO = Color.Yellow;
            settings.BlockForeColor_Anchor = SystemColors.HighlightText;
            settings.BlockForeColor_Unused = SystemColors.HighlightText;

            settings.BlockLayoutSelectedColor = SystemColors.Highlight;
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
            settings.WaveformBaseLineColor = SystemColors.ControlText;
            settings.WaveformHighlightedBackColor = SystemColors.Highlight;
            settings.mWaveFormHighlightedForeColor = SystemColors.HighlightText;

            settings.mWaveformChannel1Color = Color.FromArgb(127, Color.Blue);
            settings.mWaveformChannel2Color = Color.FromArgb(127, Color.Red);
            settings.mWaveformMonoColor = Color.FromArgb(127, Color.Blue);
            settings.mWaveformSelectionColor = SystemColors.Highlight;
            settings.mWaveformCursorColor = Color.Red;
            settings.FineNavigationColor = Color.Aqua;
            settings.RecordingHighlightPhraseColor = Color.DarkSeaGreen;
            settings.EmptySectionBackgroundColor = Color.LightPink;
            settings.HighlightedSectionNodeWithoutSelectionColor = System.Drawing.SystemColors.Control;
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
            settings.BlockBackColor_Anchor = SystemColors.Window;
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
            settings.EmptySectionBackgroundColor = Color.Green;
            settings.HighlightedSectionNodeWithoutSelectionColor = System.Drawing.SystemColors.Control;
            return settings;
        }

        public void PopulateColorSettingsDictionary()
        {
            ColorSetting = new Dictionary<string, Color>();
            ColorSetting.Add("BlockBackColor_Custom", BlockBackColor_Custom);
            ColorSetting.Add("BlockBackColor_Empty", BlockBackColor_Empty);
            ColorSetting.Add("BlockBackColor_Heading", BlockBackColor_Heading);
            ColorSetting.Add("BlockBackColor_Page", BlockBackColor_Page);
            ColorSetting.Add("BlockBackColor_Plain", BlockBackColor_Plain);
            ColorSetting.Add("BlockBackColor_Selected", BlockBackColor_Selected);
            ColorSetting.Add("BlockBackColor_Silence", BlockBackColor_Silence);
            ColorSetting.Add("BlockBackColor_TODO", BlockBackColor_TODO);
            ColorSetting.Add("BlockBackColor_Unused", BlockBackColor_Unused);
            ColorSetting.Add("BlockBackColor_Anchor", BlockBackColor_Anchor);
            ColorSetting.Add("BlockForeColor_Custom", BlockForeColor_Custom);
            ColorSetting.Add("BlockForeColor_Empty", BlockForeColor_Empty);
            ColorSetting.Add("BlockForeColor_Heading", BlockForeColor_Heading);
            ColorSetting.Add("BlockForeColor_Page", BlockForeColor_Page);
            ColorSetting.Add("BlockForeColor_Plain", BlockForeColor_Plain);
            ColorSetting.Add("BlockForeColor_Selected", BlockForeColor_Selected);
            ColorSetting.Add("BlockForeColor_Silence", BlockForeColor_Silence);
            ColorSetting.Add("BlockForeColor_TODO", BlockForeColor_TODO);
            ColorSetting.Add("BlockForeColor_Anchor", BlockForeColor_Anchor);
            ColorSetting.Add("BlockForeColor_Unused", BlockForeColor_Unused);
            ColorSetting.Add("BlockLayoutSelectedColor", BlockLayoutSelectedColor);
            ColorSetting.Add("ContentViewBackColor", ContentViewBackColor);
            ColorSetting.Add("EditableLabelTextBackColor", EditableLabelTextBackColor);
            ColorSetting.Add("ProjectViewBackColor", ProjectViewBackColor);
            ColorSetting.Add("StripBackColor",StripBackColor);
            ColorSetting.Add("StripCursorSelectedBackColor",StripCursorSelectedBackColor);
            ColorSetting.Add("StripForeColor", StripForeColor);
            ColorSetting.Add("StripSelectedBackColor", StripSelectedBackColor);
            ColorSetting.Add("StripSelectedForeColor", StripSelectedForeColor);
            ColorSetting.Add("StripUnusedBackColor", StripUnusedBackColor);
            ColorSetting.Add("StripUnusedForeColor", StripUnusedForeColor);
            ColorSetting.Add("StripWithoutPhraseBackColor", StripWithoutPhrasesBackcolor);
            ColorSetting.Add("TOCViewBackColor", TOCViewBackColor);
            ColorSetting.Add("TOCViewForeColor", TOCViewForeColor);
            ColorSetting.Add("TOCViewUnusedColor", TOCViewUnusedColor);
            ColorSetting.Add("ToolTipForeColor", ToolTipForeColor);
            ColorSetting.Add("TransportBarBackColor", TransportBarBackColor);
            ColorSetting.Add("TransportBarLabelBackColor", TransportBarLabelBackColor);
            ColorSetting.Add("TransportBarLabelForeColor", TransportBarLabelForeColor);
            ColorSetting.Add("WaveformBackColor", WaveformBackColor);
            ColorSetting.Add("WaveformBaseLineColor", WaveformBaseLineColor);
            ColorSetting.Add("WaveformHighlightedBackColor", WaveformHighlightedBackColor);
            ColorSetting.Add("FineNavigationColor", FineNavigationColor);
            ColorSetting.Add("RecordingHighlightPhrase", RecordingHighlightPhraseColor);
            ColorSetting.Add("EmptySectionBackground", EmptySectionBackgroundColor);
            ColorSetting.Add("HighlightedSectionNodeWithoutSelectionColor", HighlightedSectionNodeWithoutSelectionColor);
        }
          
    }
}
