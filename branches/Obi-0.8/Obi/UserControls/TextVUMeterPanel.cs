using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class TextVUMeterPanel : UserControl
    {
        private bool IsPlay ;
        private Playlist mPlaylist = null;
        private RecordingSession mRecordingSession = null;
        public bool Enable = false;

        public TextVUMeterPanel( )
        {
            InitializeComponent();
            mResetButton.Enabled = false;

        }
        
    public Playlist PlayListObj
    {
        get
        {
            return mPlaylist;
        }
        set
        {
            if ( Enable )
            SetPlayer(value);

        }
    }

        public RecordingSession RecordingSessionObj
        {
            get
            {
                return mRecordingSession;
            }
            set
            {
                if (Enable)
                SetRecorder(value);

            }
        }


        private void SetPlayer( Playlist PlayListArg )
        {
            
            mPlaylist = PlayListArg ;
            mRecordingSession = null;
            IsPlay = true;

            // Associate events
            mPlaylist.Audioplayer.VuMeter.PeakOverload += new Obi.Events.Audio.VuMeter.PeakOverloadHandler( CatchPeakOverloadEvent  );
            mPlaylist.Audioplayer.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler( CatchStateChangedEvent );


            Enable = false;
        }


        private void SetRecorder(RecordingSession RecordingSessionArg)
        {
            mRecordingSession = RecordingSessionArg ;
            mPlaylist = null;
            IsPlay = false;

            // Associate events
            mRecordingSession.AudioRecorderObj.VuMeterObject.PeakOverload += new Obi.Events.Audio.VuMeter.PeakOverloadHandler ( CatchPeakOverloadEvent ) ; 
            mRecordingSession.AudioRecorderObj.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler ( CatchStateChangedEvent );

            Enable = false;
        }   


        private void tmUpdateText_Tick(object sender, EventArgs e)
        {
            if (IsPlay == true && mPlaylist != null )
            {
                if (mPlaylist.Audioplayer.State == Audio.AudioPlayerState.Playing)
                {
                    mLeftBox.Text = mPlaylist.Audioplayer.VuMeter.m_MeanValueLeft.ToString();
                    mRightBox.Text = mPlaylist.Audioplayer.VuMeter.m_MeanValueRight.ToString();
                }
            }
            else if (IsPlay == false && mRecordingSession != null )
            {
                if ( mRecordingSession.AudioRecorderObj.State == Audio.AudioRecorderState.Recording || mRecordingSession.AudioRecorderObj.State == Obi.Audio.AudioRecorderState.Listening )
                {
                    mLeftBox.Text = mRecordingSession.AudioRecorderObj.VuMeterObject.m_MeanValueLeft.ToString();
                    mRightBox.Text = mRecordingSession.AudioRecorderObj.VuMeterObject.m_MeanValueRight.ToString();
                }
            }

        }

        private void CatchPeakOverloadEvent(object sender, EventArgs e)
        {
            Overload();
            Audio.VuMeter ob_VuMeter = sender as Audio.VuMeter;
        }

        private delegate void OverloadCallBack () ;

        private void Overload () 
        {
            if (InvokeRequired)
            {
                Invoke(new OverloadCallBack ( Overload ) )  ;
            }
            else
            {
                mResetButton.Enabled = true;
            }

        }


        private void CatchStateChangedEvent(object sender, EventArgs e)
        {
        }

            delegate     void StateChangeCallBack () ;

        private void StateChangeFunction ()
        {
            if (InvokeRequired)
            {
                Invoke(new StateChangeCallBack ( StateChangeFunction ));
            }
            else
            {
                
            }
        }

        private void StateChangeOperations()
        {
            if (IsPlay == true)
            {

            }
            else if (IsPlay == false)
            {

            }
        }

        private void mResetButton_Click(object sender, EventArgs e)
        {
            mResetButton.Enabled = false;
            if (IsPlay)
            {
                mPlaylist.Audioplayer.VuMeter.Reset();
            }
            else if (IsPlay == false)
            {
                mRecordingSession.AudioRecorderObj.VuMeterObject.Reset();
            }
        }

    }
}
