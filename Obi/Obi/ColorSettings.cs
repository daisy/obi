using System;
using System.Drawing;

namespace Obi
{
    [Serializable()]
    public class ColorSettings
    {
        public Color ContentViewBackColor;
        public Color ProjectViewBackColor;
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
            settings.ContentViewBackColor = SystemColors.AppWorkspace;
            settings.ProjectViewBackColor = SystemColors.Control;
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
