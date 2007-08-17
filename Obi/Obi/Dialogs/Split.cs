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
using urakawa.media.data;
using urakawa.media.data.audio;

namespace Obi.Dialogs
{
    public partial class Split : Form
    {
        private Audio.AudioPlayer mPlayer;           // the audio player
        private PhraseNode mNode;                    // the phrase to split
        private ManagedAudioMedia mSourceAudio;      // source audio
        private ManagedAudioMedia mResultAudio;      // new audio after the split
        private double mSplitTime;                   // time at which the split should occur
        private Audio.AudioPlayerState mSplitState;  // audio player state when creating the dialog
        private double mDialogLoadTime ;
        private List<double> mStepSizeList = new List<double>();
        private int mSelectedStepSize;

        public Split(PhraseNode node, double time, Audio.AudioPlayer player)
        {
            InitializeComponent();
            mNode = node;
            mDialogLoadTime = time;
            mPlayer = player;
            mSplitState = Audio.AudioPlayerState.Playing;
                        mSourceAudio = node.Audio;
            mResultAudio = null;
            
            mPlayer.StateChanged += new Events.Audio.Player.StateChangedHandler(AudioPlayer_StateChanged);
            mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            mPlayer.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
            tmUpdateTimePosition.Enabled = true;
            InitialiseStepSizeList();
            mSelectedStepSize = 3;
            MessageBox.Show(time.ToString());
        }

        /// <summary>
        /// New audio after the split.
        /// </summary>
        public ManagedAudioMedia ResultAudio { get { return mResultAudio; } }

        /// <summary>
        /// Get the time at which the split happens.
        /// </summary>
        public double SplitTime { get { return mSplitTime; } }


        private void Split_Load(object sender, EventArgs e)
        {
            txtDisplayTime.Text = ObiForm.FormatTime_hh_mm_ss(0.0);
            txtSplitTime.Text = "0";

            // start playing as soon as dialog is invoked

            if (mSplitState == Audio.AudioPlayerState.Stopped)
            {
                                 mPlayer.Play(mSourceAudio.getMediaData ()  );
                btnPreview.Enabled = false;
                btnSplit.Enabled = false;
                mPlayButton.Visible = false;
                mPauseButton.Visible = true;
            }
            else if (mSplitState == Audio.AudioPlayerState.Playing)
            {
                // TODO where's play?
                 mPlayer.Play(mSourceAudio.getMediaData ()  , mDialogLoadTime);
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

        //member variables
        int m_Step=10000;
        double m_FineStep = 500;
        bool PreviewEnabled = false;

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // assigns split text box value to m_dSplitTime if valid
            CheckSplitTime();

            // if btn preview is enabled then stop audio if playing so as to play it again from split time
            if (mPlayer.State == Audio.AudioPlayerState.Playing) mPlayer.Stop();
            if (mSourceAudio.getMediaData().getPCMLength() > mSplitTime &&
                mPlayer.State == Audio.AudioPlayerState.Stopped)
            {
                // check if sufficient time is left after split time to use GetChunk if not use Audio lengthin ms as second parameter
                // TODO check time and see about play
                if (mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat() - mSplitTime > 3000)
                    mPlayer.Play(mSourceAudio.getMediaData(), mSplitTime, mSplitTime + 3000);
                else
                    mPlayer.Play(mSourceAudio.getMediaData(), mSplitTime, mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat()); 
                PreviewEnabled = true;
                btnPreview.Text = "&Back";
                tmUpdateTimePosition.Enabled = true;
                mPauseButton.Text = "&Pause";
            }
        }
        

        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {

            // if preview mode is enabled then display formatted current play time
            // by adding it to split time as preview asset will have its start point at split time
            if (PreviewEnabled == true)
                txtDisplayTime.Text = ChangeTimeToDisplay(mPlayer.CurrentTimePosition + mSplitTime);

                // else display normal play time 
            else
            {
                double temptime;
                temptime = mPlayer.CurrentTimePosition;
                txtDisplayTime.Text = ChangeTimeToDisplay( temptime );
                
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
            if (mPlayer.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = mPlayer.CurrentTimePosition;

                // navigation will work only if the result of navigation is with in bounds of asset
                if (dCurrentPlayPosition - m_Step > 0)
                mPlayer.CurrentTimePosition = dCurrentPlayPosition - m_Step;
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
            double length = mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat();
            if (mPlayer.State == Audio.AudioPlayerState.Playing)
            {
                
                double dCurrentPlayPosition = mPlayer.CurrentTimePosition;
                if (dCurrentPlayPosition + m_Step < length - m_FineStep )
                    mPlayer.CurrentTimePosition = dCurrentPlayPosition + m_Step;
            }
            else
            {
                mSplitTime = mSplitTime + m_Step;
                if (mSplitTime > length - m_FineStep) mSplitTime = length;
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
            if (mPlayer.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = mPlayer.CurrentTimePosition;
                if (dCurrentPlayPosition - m_FineStep > 0)
                    mPlayer.CurrentTimePosition = dCurrentPlayPosition - (m_FineStep * 2);
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
            double length = mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat();
            m_FineStep = (Convert.ToDouble(txtStepSize.Text) * 1000);
            CheckSplitTime();
            if (mPlayer.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = mPlayer.CurrentTimePosition;
                if (dCurrentPlayPosition + m_FineStep < length - m_FineStep )
                    mPlayer.CurrentTimePosition = dCurrentPlayPosition + m_FineStep;
            }
            else
            {
                mSplitTime = mSplitTime + m_FineStep;
                if (mSplitTime > length - m_FineStep) mSplitTime = length;
                UpdateSplitTime();
                txtDisplayTime.Text = mSplitTime.ToString(); txtDisplayTime.Text = ChangeTimeToDisplay(mSplitTime);
            }

        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            // update m_dSplitTime from split text box
            // result of the split must be in mResultAsset
            // if split time is not on bounds of asset then stop asset if playing and split it
            if ( CheckSplitTime ()  == true )
            {
                if (mPlayer.State == Audio.AudioPlayerState.Playing) mPlayer.Stop();
                mResultAudio =  mSourceAudio.split(new urakawa.media.timing.Time( mSplitTime ));
                //mResultAudio = Audio.DataManager.SplitAndManage(mSourceAudio, mSplitTime);
                Close();
            }
            else
            {
                MessageBox.Show("Split command canceled");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (mPlayer.State == Audio.AudioPlayerState.Playing )
            mPlayer.Stop();

            //mPlayer.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

 
        private void mPauseButton_Click(object sender, EventArgs e)
        {
            if(mPlayer.State == Audio.AudioPlayerState.Playing)
            {
                // Assigns the m_dSplitTime according to playing mode i.e. preview or play
                if (PreviewEnabled)
                {
                    mSplitTime = mSplitTime + mPlayer.CurrentTimePosition;
                }
                else
                {
                    mSplitTime = mPlayer.CurrentTimePosition;
                }
                mPlayer.Stop();
                tmUpdateTimePosition.Enabled = false;
                mPlayButton.TabIndex = 1;
                mPauseButton.TabIndex = 0;
                mPlayButton.Visible = true;
                mPauseButton.Visible = false;
                btnPreview.Text = "Pre&view" ;
                btnPreview.Enabled = true;
                btnSplit.Enabled = true;
                UpdateSplitTime();
            }
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            if (mPlayer.State == Audio.AudioPlayerState.Stopped &&
                mSplitTime != mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat())
            {
                CheckSplitTime();
                 mPlayer.Play(mSourceAudio.getMediaData ()  , mSplitTime);
                tmUpdateTimePosition.Enabled = true;
                mPauseButton.TabIndex = 1;
                mPlayButton.TabIndex = 0;
                mPauseButton.Visible = true;
                mPlayButton.Visible = false;
                
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

        }

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
            btnPreview.Text = "Pre&view";
            mPauseButton.Visible = false;
            mPlayButton.Visible = true;
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
            if (mPlayer.State == Audio.AudioPlayerState.Playing)
                mPlayer.Stop();
            //mPlayer.VuMeterObject.CloseVuMeterForm();
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
        bool CheckSplitTime ()
        {
            // flag to indicate if split time is changed
            bool IsChanged = true ;

            // split Text box shows time in sec so it is converted in ms
            double dCheckTime = 0 ;
            try
            {
                dCheckTime = Convert.ToDouble(txtSplitTime.Text) * 1000;
            }
            catch
            {
                txtSplitTime.Text = ( mSplitTime / 1000 ).ToString () ;
                IsChanged = false;
                MessageBox.Show("Invalid Splittime format, continueing from previous valid time: " + txtSplitTime.Text + "sec");
            }
            if (dCheckTime < 0 || dCheckTime > mSourceAudio.getDuration().getTimeDeltaAsMillisecondFloat())
            {
                txtSplitTime.Text = ( mSplitTime / 1000 ).ToString () ;
                IsChanged = false;
                MessageBox.Show("Error! Split time is out of bounds of asset, Continueing from previous Splittime: " + txtSplitTime.Text + "sec");
            }
            else
            {
                mSplitTime = dCheckTime;
            }
            return IsChanged;
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

        private void txtSplitTime_Leave(object sender, EventArgs e)
        {
            CheckSplitTime();
        }
    }// end of class
}
