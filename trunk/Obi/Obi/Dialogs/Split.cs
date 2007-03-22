using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

using urakawa.core;
using urakawa.media;
namespace Obi.Dialogs
{
    public partial class Split : Form
    {
        private PhraseNode mNode;                     // the phrase to split
        private Assets.AudioMediaAsset mSourceAsset;  // the source asset
        private Assets.AudioMediaAsset mResultAsset;  // the new asset created by the split
        private double mSplitTime;                    // time at which the split should occur
        private Audio.AudioPlayerState mSplitState;   // audio player state when creating the dialog
        private double mDialogLoadTime ;
        private List<double> mStepSizeList = new List<double>();
        private int mSelectedStepSize;

        public Split(PhraseNode node, double time, Audio.AudioPlayerState state)
        {
            InitializeComponent();
            mNode = node;
            mDialogLoadTime = time;
            mSplitState = state;
            mSourceAsset = node.Asset;
            mResultAsset = null;
            
            Audio.AudioPlayer.Instance.StateChanged += new Events.Audio.Player.StateChangedHandler(AudioPlayer_StateChanged);
            Audio.AudioPlayer.Instance.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            Audio.AudioPlayer.Instance.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
            tmUpdateTimePosition.Enabled = true;
            InitialiseStepSizeList();
            mSelectedStepSize = 3;
        }

        private void Split_Load(object sender, EventArgs e)
        {
            txtDisplayTime.Text = "00:00:00";
            txtSplitTime.Text = "0";

            // start playing as soon as dialog is invoked
            Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
            ob_VuMeter.LowerThreshold = 50;
            ob_VuMeter.UpperThreshold = 300;
            ob_VuMeter.SampleTimeLength = 1000;

            if (mSplitState == Audio.AudioPlayerState.Stopped)
            {
                Audio.AudioPlayer.Instance.Play(mSourceAsset);
                btnPreview.Enabled = false;
                btnSplit.Enabled = false;
                mPlayButton.Visible = false;
                mPauseButton.Visible = true;
            }
            else if (mSplitState == Audio.AudioPlayerState.Playing)
            {
                Audio.AudioPlayer.Instance.Play(mSourceAsset, mDialogLoadTime);
                btnPreview.Enabled = false;
                btnSplit.Enabled = false;
                mPlayButton.Visible = false;
                mPauseButton.Visible = true;
            }
            else if (mSplitState == Audio.AudioPlayerState.Paused)
            {
                mSplitTime = mDialogLoadTime;

                tmUpdateTimePosition.Enabled = false;
                mPlayButton.Visible = true;
                mPauseButton.Visible = false;
                btnPreview.Enabled = true;
                UpdateSplitTime();
            }
            mPauseButton.Focus();

            txtStepSize.Text = (mStepSizeList[mSelectedStepSize]).ToString();
        }

        /// <summary>
        /// When split was completed, get the result (= new) asset.
        /// </summary>
        public Assets.AudioMediaAsset ResultAsset
        {
            get { return mResultAsset; }
        }

        /// <summary>
        /// Get the time at which the split happens.
        /// </summary>
        public double SplitTime
        {
            get { return mSplitTime; }
        }

        //member variables
        int m_Step=10000;
        double m_FineStep = 500;
        bool PreviewEnabled = false;

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // assigns split text box value to m_dSplitTime if valid
            CheckSplitTime();

            // if btn preview is enabled then stop audio if playing so as to play it again from split time
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                Audio.AudioPlayer.Instance.Stop();

            if (mSourceAsset.AudioLengthInBytes > mSplitTime &&
                Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped)
            {
                // Vumeter values are kept high so that there is no warning for peak over load and it is not visible also
                Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
                ob_VuMeter.LowerThreshold = 50;
                ob_VuMeter.UpperThreshold = 300;
                ob_VuMeter.SampleTimeLength = 1000;
                //Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;

                // check if sufficient time is left after split time to use GetChunk if not use Audio lengthin ms as second parameter
                if (mSourceAsset.LengthInMilliseconds - mSplitTime > 4000 )
                Audio.AudioPlayer.Instance.Play(mSourceAsset.GetChunk(mSplitTime, mSplitTime + 4000));
                else
                Audio.AudioPlayer.Instance.Play(mSourceAsset.GetChunk(mSplitTime, mSourceAsset.LengthInMilliseconds-100 ));
                PreviewEnabled = true;
                btnPreview.Text = "&Back";
                tmUpdateTimePosition.Enabled = true;
                mPauseButton.Text = "&Pause";
            }
        }

        //long lCountPreviewMinuts=4;
        //long tmCount ;
        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {

            // if preview mode is enabled then display formatted current play time
            // by adding it to split time as preview asset will have its start point at split time
            if (PreviewEnabled == true)
                txtDisplayTime.Text = ChangeTimeToDisplay(Audio.AudioPlayer.Instance.CurrentTimePosition + mSplitTime);

                // else display normal play time 
            else
            {
                double temptime;
                temptime = Audio.AudioPlayer.Instance.CurrentTimePosition;
                txtDisplayTime.Text = ChangeTimeToDisplay( temptime );
                //AudioTrackBar.Value = Convert.ToInt32( temptime / 100) ; 
 
            }

        }

        private void btnFastRewind_Click(object sender, EventArgs e)
        {
            FastRewind () ;
        }
        

        void FastRewind ()
        {
            // updates m_dSplitTime from split text box
            CheckSplitTime();

            // if state is playing navigation will act only on play and it will not change split time
            // else if state is stopped, it will change split time in m_dSplitTime and split text box.
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;

                // navigation will work only if the result of navigation is with in bounds of asset
                if (dCurrentPlayPosition - m_Step > 0)
                Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition - m_Step;
            }
            else
            {
                //btnPause.Text = "&Play";

                //if state is stoped, m_dSplitTime is changed and if result of navigation is out of bound of asset
                // then split time is taken upto the respective bound
                mSplitTime = mSplitTime - m_Step;
                if (mSplitTime < 0)
                    mSplitTime = 0;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            FastForward();
        }

        void FastForward ()
        {
            CheckSplitTime();
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition + m_Step < mSourceAsset.LengthInMilliseconds  - m_FineStep )
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_Step;
            }
            else
            {
                mSplitTime = mSplitTime + m_Step;
                if (mSplitTime > mSourceAsset.LengthInMilliseconds - m_FineStep)
                    mSplitTime = mSourceAsset.LengthInMilliseconds;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }
        }

        private void btnFineRewind_Click(object sender, EventArgs e)
        {
            FineRewind();
        }

        void FineRewind ()
        {
            m_FineStep = (  Convert.ToDouble(txtStepSize.Text ) * 1000 ) ;
            CheckSplitTime();
            mPauseButton.Text = "&Play";
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition - m_FineStep > 0)
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition - (m_FineStep * 2);
            }
            else
            {
                mSplitTime = mSplitTime - m_FineStep;
                if (mSplitTime < 0)
                    mSplitTime = 0;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }
        }

        private void btnFineForward_Click(object sender, EventArgs e)
        {
            FineForward() ;
        }

        void FineForward ()
        {
            
            m_FineStep = (Convert.ToDouble(txtStepSize.Text) * 1000);
            CheckSplitTime();
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition + m_FineStep < mSourceAsset.LengthInMilliseconds - m_FineStep )
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_FineStep;
            }
            else
            {
                mSplitTime = mSplitTime + m_FineStep;
                if (mSplitTime > mSourceAsset.LengthInMilliseconds- m_FineStep)
                    mSplitTime = mSourceAsset.LengthInMilliseconds;
                UpdateSplitTime();
                txtDisplayTime.Text = mSplitTime.ToString(); txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }

        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            // update m_dSplitTime from split text box
            CheckSplitTime();
            // result of the split must be in mResultAsset
            // if split time is not on bounds of asset then stop asset if playing and split it
            if (mSplitTime > 0 && mSplitTime < mSourceAsset.LengthInMilliseconds)
            {
                if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing) Audio.AudioPlayer.Instance.Stop();
                mResultAsset = mSourceAsset.Manager.SplitAudioMediaAsset(mSourceAsset, mSplitTime);
                // mResultAsset = mSourceAsset.Split(mSplitTime);
                // mSourceAsset.Manager.AddAsset(mResultAsset);
                Close();
            }
            else
            {
                MessageBox.Show("Enter correct value to split");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing )
            Audio.AudioPlayer.Instance.Stop();

            //Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

 
        private void mPauseButton_Click(object sender, EventArgs e)
        {
            if(Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                // Assigns the m_dSplitTime according to playing mode i.e. preview or play
                if (PreviewEnabled)
                {
                    mSplitTime = mSplitTime + Audio.AudioPlayer.Instance.CurrentTimePosition;
                }
                else
                {
                    mSplitTime = Audio.AudioPlayer.Instance.CurrentTimePosition;
                }
                Audio.AudioPlayer.Instance.Stop();
                tmUpdateTimePosition.Enabled = false;
                mPlayButton.Visible = true;
                mPauseButton.Visible = false;
                btnSplit.Enabled = true;
                UpdateSplitTime();
            }
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped && mSplitTime != mSourceAsset.LengthInMilliseconds)
            {
                CheckSplitTime();
                Audio.AudioPlayer.Instance.Play(mSourceAsset, mSplitTime);
                tmUpdateTimePosition.Enabled = true;
                mPlayButton.Visible = false;
                mPauseButton.Visible = true;
                btnSplit.Enabled = true;
                btnPreview.Enabled = false;
                txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }
        }

        
            

            



        private void AudioPlayer_StateChanged(object sender, Events.Audio.Player.StateChangedEventArgs e)
        {
        }

        private void AudioPlayer_EndOfAudioAsset(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
            tmUpdateTimePosition.Enabled = false;
            PreviewEnabled = false;
            // for safe threading following function is called through delegate using invoke required
            CallEndAssetOperations();

            // following one line added for serial playing experiment
            //CanPlay = true;
        }

        // following one line added for serial playing experiment
        //bool CanPlay = false; 


            void CallEndAssetOperations()
        {
            if (InvokeRequired)
            {
                Invoke(new CloseCallback( EndAssetOperations));
            }
                else
                EndAssetOperations() ;
            
        }

        void EndAssetOperations()
        {
            mPauseButton.Text = "&Play";
            btnPreview.Text = "Pre&view";
            mPauseButton.Enabled = true;
            btnPreview.Enabled = true;
               txtDisplayTime.Text = ChangeTimeToDisplay (mSplitTime);
        }
         //Convoluted way to close necessary for debugging (JQ)
        private delegate void CloseCallback();

        public new void Close()
        {
            if (InvokeRequired)
            {
                Invoke(new CloseCallback(Close));
            }
            else
            {
                base.Close();
            }
        }

        private void AudioPlayer_UpdateVuMeter(object sender, Events.Audio.Player.UpdateVuMeterEventArgs e)
        {
        }

        private void Split_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                Audio.AudioPlayer.Instance.Stop();
            //Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
        }
        
        // change time in double to formatted time i.e. hh:mm:ss format
        string ChangeTimeToDisplay(double dTime)
        {
            double dMiliSeconds = dTime;
            int Seconds = Convert.ToInt32 (dMiliSeconds / 1000);
            int DisplaySeconds = Seconds;
            if (DisplaySeconds > 59)
                DisplaySeconds = DisplaySeconds - (60 * (DisplaySeconds / 60));

            string sSeconds = DisplaySeconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            int DisplayMinutes = Minutes;
            if (DisplayMinutes > 59)
                DisplayMinutes = DisplayMinutes - ( 60 * ( DisplayMinutes / 60 )) ;

            string sMinutes = DisplayMinutes.ToString("00");
            int Hours = Minutes / 60;
            int DisplayHours = Hours;
            if (DisplayHours > 23)
                DisplayHours = DisplayHours  - 24;

            string sHours = DisplayHours.ToString("00");
            return sHours + ":" + sMinutes + ":" + sSeconds;
        
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {

                MessageBox.Show("Key handled");

        }


        
        void UpdateSplitTime()
        {
            double dDisplaySplitTime = mSplitTime / 1000;
                txtSplitTime.Text = dDisplaySplitTime.ToString ();
        }

        private void tmCheckSplitTime_Tick(object sender, EventArgs e)
        {}

        // checks the validity of split time in split text box and assign it to 
        // m_dSplitTime if it is not 0 or end of asset else show a error message box
        void CheckSplitTime ()
        {
            // split Text box shows time in sec so it is converted in ms
            double dCheckTime = Convert.ToDouble (txtSplitTime.Text)* 1000;
            if (dCheckTime < 0 || dCheckTime > mSourceAsset.LengthInMilliseconds)
            {
                MessageBox.Show("Error! Split time is out of bounds of asset");
            }
            else
            {
                mSplitTime = dCheckTime;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            
            switch (keyData)
            {
                case Keys.Alt | Keys.Up:
                    IncrementStepSize();
                    break;
                
                case Keys.Alt | Keys.Down:
                    DecrementStepSize();
                    break;
                case Keys.Alt | Keys.Left:
                    FineRewind();
                    break;
                case Keys.Alt | Keys.Right:
                    FineForward();
                    break;
                        
                case Keys.Control | Keys.Alt | Keys.T :
                    txtDisplayTime.Focus();
                    break;
            }
            
            return base.ProcessDialogKey(keyData);

        }

        // experiment for serial  playing of assets start line
        private void tmMonitorEnd_Tick(object sender, EventArgs e)
        {
            //if (CanPlay == true)
            //{
                //Audio.AudioPlayer.Instance.Play(mSourceAsset);
                //CanPlay = false;
            //}
            
        }


        private void InitialiseStepSizeList ()
    {
        mStepSizeList.Add (.1) ;
        mStepSizeList.Add(.2);
        mStepSizeList.Add(.3);
        mStepSizeList.Add(.5);

        mStepSizeList.Add(1);
        mStepSizeList.Add(2);
        mStepSizeList.Add(3);
        mStepSizeList.Add(5);

        mStepSizeList.Add(10);
        mStepSizeList.Add(15);
    }

        private void btnStepSizeIncrement_Click(object sender, EventArgs e)
        {
            IncrementStepSize();
        }

        private void btnStepSizeDecrement_Click(object sender, EventArgs e)
        {
            DecrementStepSize();
        }

        private void IncrementStepSize()
        {
            if (mSelectedStepSize < mStepSizeList.Count - 1)
            {
                ++mSelectedStepSize;
                txtStepSize.Text = (mStepSizeList[mSelectedStepSize]).ToString();
            }
        }

        private void DecrementStepSize()
        {
            if (mSelectedStepSize > 0)
            {
                --mSelectedStepSize;
                txtStepSize.Text = (mStepSizeList[mSelectedStepSize]).ToString();
            }
        }

        private void txtStepSize_TextChanged(object sender, EventArgs e)
        {

        }
    }// end of class
}
