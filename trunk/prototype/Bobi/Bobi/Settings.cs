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
        public Color TrackForeColor;
        public Color TrackSelectedForeColor;

        public Color AudioBlockBackColor;
        public Color AudioBlockSelectedBackColor;
        public Pen AudioSelectionPen;
        public SolidBrush AudioSelectionBrush;
        public Pen AudioPlaybackPen;
        public SolidBrush AudioPlaybackBrush;
        public Pen WaveformBaseLinePen;
        public Pen WaveformBaseLineSelectedPen;
        public Pen WaveformMonoPen;
        public Pen WaveformChannel1Pen;
        public Pen WaveformChannel2Pen;
        public Pen WaveformChannelSelectedPen;
        public Brush TrackLayoutSelectedBrush;

        public static ColorSettings DefaultColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = System.Drawing.SystemColors.ControlDark;
            scheme.TrackBackColor = System.Drawing.SystemColors.Control;
            scheme.TrackSelectedBackColor = System.Drawing.SystemColors.Highlight;
            scheme.TrackForeColor = System.Drawing.SystemColors.ControlText;
            scheme.TrackSelectedForeColor = System.Drawing.SystemColors.HighlightText;
            scheme.AudioBlockBackColor = System.Drawing.SystemColors.ControlLightLight;
            scheme.AudioBlockSelectedBackColor = scheme.TrackSelectedBackColor;
            scheme.TrackLayoutSelectedBrush = new SolidBrush(System.Drawing.SystemColors.Highlight);
            scheme.WaveformChannelSelectedPen = new Pen(System.Drawing.SystemColors.HighlightText);
            scheme.WaveformBaseLinePen = new Pen(System.Drawing.SystemColors.ControlText);
            scheme.WaveformBaseLineSelectedPen = scheme.WaveformBaseLinePen;

            scheme.WaveformMonoPen = new Pen(Color.Blue);
            
            scheme.AudioSelectionPen = new Pen(Color.FromArgb(128, 128, 255, 128));
            scheme.AudioSelectionBrush = new SolidBrush(scheme.AudioSelectionPen.Color);
            scheme.AudioPlaybackPen = new Pen(Color.FromArgb(128, 255, 128, 128));
            scheme.AudioPlaybackBrush = new SolidBrush(scheme.AudioPlaybackPen.Color);
            scheme.WaveformChannel1Pen = new Pen(Color.FromArgb(128, 0, 0, 255));
            scheme.WaveformChannel2Pen = new Pen(Color.FromArgb(128, 255, 0, 255));
            return scheme;
        }

        public static ColorSettings HighContrastColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = System.Drawing.SystemColors.ControlDark;
            scheme.TrackBackColor = System.Drawing.SystemColors.Control;
            scheme.TrackSelectedBackColor = System.Drawing.SystemColors.Highlight;
            scheme.TrackForeColor = System.Drawing.SystemColors.ControlText;
            scheme.TrackSelectedForeColor = System.Drawing.SystemColors.HighlightText;
            scheme.AudioBlockBackColor = System.Drawing.SystemColors.ControlLightLight;
            scheme.AudioBlockSelectedBackColor = scheme.TrackSelectedBackColor;
            scheme.WaveformMonoPen = new Pen(System.Drawing.SystemColors.ControlText);
            scheme.TrackLayoutSelectedBrush = new SolidBrush(System.Drawing.SystemColors.Highlight);
            scheme.WaveformChannelSelectedPen = new Pen(System.Drawing.SystemColors.HighlightText);
            scheme.WaveformBaseLinePen = new Pen(System.Drawing.SystemColors.ControlText);
            scheme.WaveformBaseLineSelectedPen = scheme.WaveformBaseLinePen;
            return scheme;
        }
    }
}
