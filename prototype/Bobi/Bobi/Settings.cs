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
        public Color TrackLayoutBackColor;
        public Color AudioBlockBackColor;
        public Color AudioBlockSelectedBackColor;
        public Pen AudioSelectionPen;
        public SolidBrush AudioSelectionBrush;
        public Pen AudioPlaybackPen;
        public SolidBrush AudioPlaybackBrush;
        public Pen WaveformBaseLinePen;
        public Pen WaveformChannel1Pen;
        public Pen WaveformChannel2Pen;
        public Brush TrackLayoutSelectedBrush;

        public static ColorSettings DefaultColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = Color.White;
            scheme.TrackBackColor = Color.CornflowerBlue;
            scheme.TrackSelectedBackColor = Color.Yellow;
            scheme.TrackForeColor = Color.Black;
            scheme.TrackSelectedForeColor = Color.Red;
            scheme.TrackLayoutBackColor = Color.LightBlue;
            scheme.AudioBlockBackColor = Color.White;
            scheme.AudioBlockSelectedBackColor = scheme.TrackSelectedBackColor;
            scheme.AudioSelectionPen = new Pen(Color.FromArgb(128, 128, 255, 128));
            scheme.AudioSelectionBrush = new SolidBrush(scheme.AudioSelectionPen.Color);
            scheme.AudioPlaybackPen = new Pen(Color.FromArgb(128, 255, 128, 128));
            scheme.AudioPlaybackBrush = new SolidBrush(scheme.AudioPlaybackPen.Color);
            scheme.WaveformBaseLinePen = Pens.BlueViolet;
            scheme.WaveformChannel1Pen = new Pen(Color.FromArgb(128, 0, 0, 255));
            scheme.WaveformChannel2Pen = new Pen(Color.FromArgb(128, 255, 0, 255));
            scheme.TrackLayoutSelectedBrush = Brushes.Yellow;
            return scheme;
        }

        public static ColorSettings HighContrastColorScheme()
        {
            ColorSettings scheme = new ColorSettings();
            scheme.ProjectViewBackColor = Color.Black;
            scheme.TrackBackColor = Color.White;
            scheme.TrackSelectedBackColor = Color.Green;
            scheme.TrackForeColor = Color.Black;
            scheme.TrackSelectedForeColor = Color.White;
            scheme.TrackLayoutBackColor = Color.LightBlue;
            scheme.AudioBlockBackColor = Color.Orange;
            scheme.AudioBlockSelectedBackColor = scheme.TrackSelectedBackColor;
            scheme.AudioSelectionPen = new Pen(Color.FromArgb(128, 0, 255, 0));
            scheme.AudioSelectionBrush = new SolidBrush(scheme.AudioSelectionPen.Color);
            scheme.AudioPlaybackPen = new Pen(Color.FromArgb(128, 255, 0, 0));
            scheme.AudioPlaybackBrush = new SolidBrush(scheme.AudioPlaybackPen.Color);
            scheme.WaveformBaseLinePen = Pens.White;
            scheme.WaveformChannel1Pen = new Pen(Color.FromArgb(128, 255, 255, 0));
            scheme.WaveformChannel2Pen = new Pen(Color.FromArgb(128, 255, 0, 0));
            scheme.TrackLayoutSelectedBrush = Brushes.Green;
            return scheme;
        }
    }
}
