using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Bobi
{
    public class Settings
    {
        private ColorSettings colorScheme;
        private ColorSettings colorScheme_HighContrast;

        public Settings()
        {
            this.colorScheme = ColorSettings.DefaultColorScheme();
            this.colorScheme_HighContrast = ColorSettings.HighContrastColorScheme();
        }

        public ColorSettings ColorScheme { get { return this.colorScheme; } }
        public ColorSettings ColorScheme_HighContrast { get { return this.colorScheme_HighContrast; } }
    }

    public class ColorSettings
    {
        public Color ProjectViewBackColor;
        public Color TrackBackColor;
        public Color TrackSelectedBackColor;

        public static ColorSettings DefaultColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = Color.White;
            scheme.TrackBackColor = Color.CornflowerBlue;
            scheme.TrackSelectedBackColor = Color.Yellow;
            return scheme;
        }

        public static ColorSettings HighContrastColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = Color.Black;
            scheme.TrackBackColor = Color.White;
            scheme.TrackSelectedBackColor = Color.Green;
            return scheme;
        }
    }
}
