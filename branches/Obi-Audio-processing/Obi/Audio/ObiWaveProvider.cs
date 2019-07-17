using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Dsp;

namespace Obi.Audio
{
    class ObiWaveProvider : ISampleProvider
    {
         private ISampleProvider sourceProvider;
        private float bandPassFreqency;

        private int channels;
        private BiQuadFilter[] filters;


        public ObiWaveProvider(ISampleProvider sourceProvider, float BandPassFreqency)
        {
            this.sourceProvider = sourceProvider;
            this.bandPassFreqency = BandPassFreqency;

            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int n = 0; n < channels; n++)
            {
                filters[n] = BiQuadFilter.BandPassFilterConstantPeakGain(44100, bandPassFreqency, 1);
                //filters[n] = BiQuadFilter.LowPassFilter(44100, bandPassFreqency, 1);

            }
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                buffer[offset + i] = filters[(i % channels)].Transform(buffer[offset + i]);
            }

            return samplesRead;
        }
    }
}
