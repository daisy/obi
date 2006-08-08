using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using urakawa.core;
using urakawa.media;
namespace Obi.Dialogs
{
    public partial class Split : Form
    {
        private CoreNode mNode;  // the node to split
        private Assets.AudioMediaAsset mResultAsset;  // the new asset created by the split

        public Assets.AudioMediaAsset ResultAsset
        {
            get
            {
                return mResultAsset;
            }
        }

        public double SplitTime
        {
            get
            {
                return m_dSplitTime;
            }
        }

        public Split(CoreNode node, double splitTime)
        {
            InitializeComponent();
            mNode = node;
            m_dSplitTime = splitTime;
            ob_AudioAsset = Project.GetAudioMediaAsset(node);
            Audio.AudioPlayer.Instance.StateChanged += new Events.Audio.Player.StateChangedHandler(AudioPlayer_StateChanged);
            Audio.AudioPlayer.Instance.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            Audio.AudioPlayer.Instance.EndOfAudioBuffer += new Events.Audio.Player.EndOfAudioBufferHandler(AudioPlayer_EndOfAudioBuffer);
            Audio.AudioPlayer.Instance.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);

            // enable timer for displaying formatted time in HH:mm:ss
            tmUpdateTimePosition.Enabled = true;
        }
        
        //member variables
        Assets.AudioMediaAsset ob_AudioAsset;
        double m_dSplitTime;
        int m_Step=10000;
        int m_FineStep = 500;
        bool PreviewEnabled = false;

        

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // assigns split text box value to m_dSplitTime if valid
            CheckSplitTime();

            // if btn preview is enabled then stop audio if playing so as to play it again from split time
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                Audio.AudioPlayer.Instance.Stop();

            if (ob_AudioAsset.AudioLengthInBytes > m_dSplitTime &&
                Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped)
            {
                // Vumeter values are kept high so that there is no warning for peak over load and it is not visible also
                Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
                ob_VuMeter.LowerThreshold = 50;
                ob_VuMeter.UpperThreshold = 300;
                ob_VuMeter.SampleTimeLength = 1000;
                Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;

                // check if sufficient time is left after split time to use GetChunk if not use Audio lengthin ms as second parameter
                if (ob_AudioAsset.LengthInMilliseconds - m_dSplitTime > 4000 )
                Audio.AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, m_dSplitTime + 4000));
                else
                Audio.AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, ob_AudioAsset.LengthInMilliseconds-100 ));
                PreviewEnabled = true;
                btnPreview.Text = "&Back";
                tmUpdateTimePosition.Enabled = true;
                btnPause.Text = "&Pause";
            }
        }

        //long lCountPreviewMinuts=4;
        //long tmCount ;
        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {

            // if preview mode is enabled then display formatted current play time
            // by adding it to split time as preview asset will have its start point at split time
            if (PreviewEnabled == true)
                txtDisplayTime.Text = ChangeTimeToDisplay(Audio.AudioPlayer.Instance.CurrentTimePosition + m_dSplitTime);

                // else display normal play time 
            else
                txtDisplayTime.Text = ChangeTimeToDisplay(Audio.AudioPlayer.Instance.CurrentTimePosition);


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
                m_dSplitTime = m_dSplitTime - m_Step;
                if (m_dSplitTime < 0)
                    m_dSplitTime = 0;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
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
                if (dCurrentPlayPosition + m_Step < ob_AudioAsset.LengthInMilliseconds  - m_FineStep )
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_Step;
            }
            else
            {
                m_dSplitTime = m_dSplitTime + m_Step;
                if (m_dSplitTime > ob_AudioAsset.LengthInMilliseconds - m_FineStep)
                    m_dSplitTime = ob_AudioAsset.LengthInMilliseconds;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
        }

        private void btnFineRewind_Click(object sender, EventArgs e)
        {
            FineRewind();
        }

        void FineRewind ()
        {
            CheckSplitTime();
            btnPause.Text = "&Play";
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition - m_FineStep > 0)
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition - (m_FineStep * 2);
            }
            else
            {
                m_dSplitTime = m_dSplitTime - m_FineStep;
                if (m_dSplitTime < 0)
                    m_dSplitTime = 0;
                UpdateSplitTime();
                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
        }

        private void btnFineForward_Click(object sender, EventArgs e)
        {
            FineForward() ;
        }

        void FineForward ()
        {
            CheckSplitTime();
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition + m_FineStep < ob_AudioAsset.LengthInMilliseconds - m_FineStep )
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_FineStep;
            }
            else
            {
                m_dSplitTime = m_dSplitTime + m_FineStep;
                if (m_dSplitTime > ob_AudioAsset.LengthInMilliseconds- m_FineStep)
                    m_dSplitTime = ob_AudioAsset.LengthInMilliseconds;
                UpdateSplitTime();
                txtDisplayTime.Text = m_dSplitTime.ToString(); txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }

        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            // update m_dSplitTime from split text box
            CheckSplitTime();
            // result of the split must be in mResultAsset
            // if split time is not on bounds of asset then stop asset if playing and split it
            if (m_dSplitTime > 0 && m_dSplitTime < ob_AudioAsset.LengthInMilliseconds)
            {
                if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                    Audio.AudioPlayer.Instance.Stop();

                mResultAsset = ob_AudioAsset.Split(m_dSplitTime) as Assets.AudioMediaAsset;
                ob_AudioAsset.Manager.AddAsset(mResultAsset);
                Close();
            }
            else
            MessageBox.Show("Enter correct value to split");

            
                
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing )
            Audio.AudioPlayer.Instance.Stop();

            Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

        private void Split_Load(object sender, EventArgs e)
        {
            
            txtDisplayAsset.Text = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannel)).getText();
            txtDisplayTime.Text = "00:00:00";
            txtSplitTime.Text = "0";

            // start playing as soon as dialog is invoked
            Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
            ob_VuMeter.LowerThreshold = 50;
            ob_VuMeter.UpperThreshold = 300;
            ob_VuMeter.SampleTimeLength = 1000;
            Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
            Audio.AudioPlayer.Instance.Play(ob_AudioAsset);
            btnPreview.Enabled = false;
            btnSplit.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {

            btnSplit.Enabled = true;
            if(Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {

                // Assigns the m_dSplitTime according to playing mode i.e. preview or play
                if (PreviewEnabled == false)
                m_dSplitTime = Audio.AudioPlayer.Instance.CurrentTimePosition;
                else
                m_dSplitTime = m_dSplitTime  + Audio.AudioPlayer.Instance.CurrentTimePosition;

                Audio.AudioPlayer.Instance.Stop();
            tmUpdateTimePosition.Enabled = false;
                btnPause.Text = "&Play";
                btnPreview.Enabled= true;
                btnPreview.Text = "Pre&view";
                PreviewEnabled = false;
                UpdateSplitTime();
            }
            else if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped && m_dSplitTime != ob_AudioAsset.LengthInMilliseconds)
            {
                CheckSplitTime();
                //MessageBox.Show(m_dSplitTime.ToString());
                //AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, ob_AudioAsset.LengthInMilliseconds));
                Audio.AudioPlayer.Instance.Play( ob_AudioAsset , m_dSplitTime);
                
// enable the timer for displaying formated play time
                tmUpdateTimePosition.Enabled = true;
                btnPause.Text = "&Pause";
                btnPreview.Enabled = false;
            }

            txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
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
            btnPause.Text = "&Play";
            btnPreview.Text = "Pre&view";
            btnPause.Enabled = true;
            btnPreview.Enabled = true;
               txtDisplayTime.Text = ChangeTimeToDisplay (m_dSplitTime);
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

        private void AudioPlayer_EndOfAudioBuffer(object sender, Events.Audio.Player.EndOfAudioBufferEventArgs e)
        {
        }

        private void AudioPlayer_UpdateVuMeter(object sender, Events.Audio.Player.UpdateVuMeterEventArgs e)
        {
        }

        private void Split_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                Audio.AudioPlayer.Instance.Stop();
            Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
        }
        
        // change time in double to formatted time i.e. hh:mm:ss format
        string ChangeTimeToDisplay(double dTime)
        {
            double dMiliSeconds = dTime;
            int Seconds = Convert.ToInt32 (dMiliSeconds / 1000);
            string sSeconds = Seconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes = Minutes.ToString("00");
            int Hours = Minutes / 60;
            string sHours = Hours.ToString("00");
            return sHours + ":" + sMinutes + ":" + sSeconds;
        
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {

                MessageBox.Show("Key handled");

        }


        
        void UpdateSplitTime ( )
        {
            double dDisplaySplitTime = m_dSplitTime / 1000;
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
            if (dCheckTime < 0 || dCheckTime > ob_AudioAsset.LengthInMilliseconds)
            {
                MessageBox.Show("Error! Split time is out of bounds of asset");
            }
            else
            {
                m_dSplitTime = dCheckTime;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            
            switch (keyData)
            {
                case Keys.Control | Keys.Up:
                    FastRewind();
                    break;
                
                case Keys.Control | Keys.Down:
                    FastForward();
                    break;
                case Keys.Control | Keys.Left:
                    FineRewind();
                    break;
                case Keys.Control | Keys.Right:
                    FineForward();
                    break;
                        
            }
            
            return base.ProcessDialogKey(keyData);

        }
       


        


    }// end of class
}