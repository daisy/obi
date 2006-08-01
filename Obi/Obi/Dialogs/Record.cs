using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VirtualAudioBackend;

namespace Obi.Dialogs
{
    /// <summary>
    /// The record dialog.
    /// Start listening as soon as it is open.
    /// 
    /// </summary>
    /// <remarks>JQ</remarks>
    public partial class Record : Form
    {
        private int mChannels;                  // required number of channels
        private int mSampleRate;                // required sample rate
        private int mBitDepth;                  // required bit depth
        private AssetManager mAssManager;       // the asset manager (for creating new assets)

        private List<AudioMediaAsset> mAssets;  // the list of assets created while recording

        /// <summary>
        /// The list of assets created.
        /// </summary>
        public List<AudioMediaAsset> Assets
        {
            get
            {
                return mAssets;
            }
        }

        public Record(int channels, int sampleRate, int bitDepth, AssetManager assManager)
        {
            InitializeComponent();
            mChannels = channels;
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            mAssManager = assManager;
            mAssets = new List<AudioMediaAsset>();
        }
    }
}