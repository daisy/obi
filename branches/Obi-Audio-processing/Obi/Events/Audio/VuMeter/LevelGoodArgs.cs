using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Audio.VuMeter
    {
    public delegate void LevelGoodHandler ( object sender, LevelGoodArgs e );
    public class LevelGoodArgs : EventArgs
        {
        private double m_AmplitudeLevel;

        public LevelGoodArgs ( double Amplitude )
            {
            m_AmplitudeLevel = Amplitude;
            }

        public double AmplitudeValue
            {
            get { return m_AmplitudeLevel; }
            }
        }
    }
