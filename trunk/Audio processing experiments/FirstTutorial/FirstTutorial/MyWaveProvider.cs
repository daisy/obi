using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Dsp;


namespace FirstTutorial
{
    class MyWaveProvider : ISampleProvider
    {
        private ISampleProvider sourceProvider;
        private float lowCutOffFreq;
        private float highCutOffFreq;

        private int channels;
        private BiQuadFilter[] filters;
        private BiQuadFilter[] highPassFilters;

        public MyWaveProvider(ISampleProvider sourceProvider, int LowCutOffFreq, int HighCutOffFreqency)
        {
            this.sourceProvider = sourceProvider;
            this.lowCutOffFreq = LowCutOffFreq;
            this.highCutOffFreq = HighCutOffFreqency;

            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels];
            highPassFilters = new BiQuadFilter[channels];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int n = 0; n < channels; n++)
                if (filters[n] == null)
                {
                    highPassFilters[n] = BiQuadFilter.HighPassFilter(44100, highCutOffFreq, 1);
                    filters[n] = BiQuadFilter.LowPassFilter(44100, lowCutOffFreq, 1);
                }
                else
                {
                    highPassFilters[n].SetHighPassFilter(44100, highCutOffFreq, 1);
                    filters[n].SetLowPassFilter(44100, lowCutOffFreq, 1);
                }
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                buffer[offset + i] = filters[(i % channels)].Transform(buffer[offset + i]);
                buffer[offset + i] = highPassFilters[(i % channels)].Transform(buffer[offset + i]);
            }

            return samplesRead;
        }
    }
}
