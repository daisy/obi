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

        /// <summary>
        /// Get the default color settings.
        /// </summary>
        public static ColorSettings DefaultColorSettings()
        {
            ColorSettings settings = new ColorSettings();
            settings.BlockBackColor = SystemColors.Window;
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
            return settings;
        }

        /// <summary>
        /// Get the default color settings for high-contrast mode.
        /// </summary>
        public static ColorSettings DefaultColorSettingsHC()
        {
            ColorSettings settings = DefaultColorSettings();
            // we do not need to change system colors.
            return settings;
        }
    }
}
