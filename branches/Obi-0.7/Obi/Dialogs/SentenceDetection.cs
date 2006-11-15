using System;
using System.Collections ;
using System.Collections.Generic ;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for sentence detection parameter settting.
    /// </summary>
    public partial class SentenceDetection : Form
    {
        private long mThreshold;
        private double mGap;
        private double mLeadingSilence;

        private Assets.AudioMediaAsset mSilenceAsset;
        private Assets.AudioMediaAsset mPhraseAsset;
        private List<Assets.AudioMediaAsset> mPhraseList;


        public long Threshold
        {
            get { return mThreshold; }
        }

        public double Gap
        {
            get { return mGap; }
        }

        public double LeadingSilence
        {
            get { return mLeadingSilence; }
        }

        public List<Assets.AudioMediaAsset> PhraseList
        {
            get
            {
                return mPhraseList;
            }
        }

        public SentenceDetection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instantiate the dialog.
        /// </summary>
        /// <param name="silence">The silence phrase.</param>
        /// <param name="phrase">The phrase to split into sentences.</param>
        public SentenceDetection(CoreNode silence, CoreNode phrase)
        {
            InitializeComponent();

            mSilenceAsset = Project.GetAudioMediaAsset ( silence );
            mPhraseAsset = Project.GetAudioMediaAsset( phrase );



            mGap = 700;
            mLeadingSilence = 100;
        }

        private void SentenceDetection_Load(object sender, EventArgs e)
        {
            mThreshold = mSilenceAsset.GetSilenceAmplitude(mSilenceAsset);
            mThresholdBox.Text = mThreshold.ToString();
            mGapBox.Text = mGap.ToString();
            mLeadingSilenceBox.Text = mLeadingSilence.ToString();
        }

        private void mOKButton_Click(object sender, EventArgs e)
        {
            mThreshold = Convert.ToInt64 ( mThresholdBox.Text ) ;
            mGap = Convert.ToDouble(mGapBox.Text);
            mLeadingSilence = Convert.ToDouble ( mLeadingSilenceBox.Text ) ;

            
            mPhraseList =  mPhraseAsset.ApplyPhraseDetection(mThreshold, mGap, mLeadingSilence)  ;
            Assets.AudioMediaAsset FirstAsset = mPhraseList[ 4 ] as Assets.AudioMediaAsset;

            MessageBox.Show("Phrase count" + mPhraseList.Count.ToString());
            //MessageBox.Show(FirstAsset.LengthInMilliseconds.ToString());
            //Audio.AudioPlayer.Instance.Play ( FirstAsset );
        }
    }
}