using System;
using System.Drawing;

namespace Obi
{
    [Serializable()]
    public class ColorSettings
    {
        /// <summary>
        /// TOC view background color.
        /// </summary>
        public Color TOCViewBackColor;

        /// <summary>
        /// Content view background color.
        /// </summary>
        public Color ContentViewBackColor;

        /// <summary>
        /// Get the default color settings.
        /// </summary>
        public static ColorSettings DefaultColorSettings()
        {
            ColorSettings settings = new ColorSettings();
            settings.TOCViewBackColor = SystemColors.Window;
            settings.ContentViewBackColor = SystemColors.ControlDark;
            return settings;
        }

        /// <summary>
        /// Get the default color settings for high-contrast mode.
        /// </summary>
        public static ColorSettings HighContrastColorSettings()
        {
            ColorSettings settings = DefaultColorSettings();
            // we do not need to change system colors.
            return settings;
        }
    }
}
