using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Dsp;


namespace Obi.ProjectView
{
    class MyWaveProvider : ISampleProvider
    {
        private ISampleProvider sourceProvider;
        private float bandPassFreqency;
        private bool isLowPassFilter;

        private int channels;
        private BiQuadFilter[] filters;

        public MyWaveProvider(ISampleProvider sourceProvider,bool IsLowPass = false)
        {
            this.sourceProvider = sourceProvider;

            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels];
            isLowPassFilter = IsLowPass;
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int n = 0; n < channels; n++)
            {
                //filters[n] = BiQuadFilter.BandPassFilterConstantPeakGain(44100, bandPassFreqency, 1);
                if (!isLowPassFilter)
                {
                    filters[n] = BiQuadFilter.HighPassFilter(44100, 200, 1);
                }
                else
                {
                    filters[n] = BiQuadFilter.LowPassFilter(44100, 3000, 1);
                }

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
