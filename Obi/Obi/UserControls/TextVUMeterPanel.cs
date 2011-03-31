using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AudioLib;

namespace Obi.UserControls
{
    public partial class TextVUMeterPanel : UserControl
    {
        // member variables
        private AudioLib.VuMeter m_VuMeter;    // Instance of VuMeter 
                private String m_StrLeftOverloadIndicator;
        private String m_StrRightOverloadIndicator;
        private string m_strLeftLowLevelIndicator;
        private string m_strRightLowLevelIndicator;
        private bool m_BeepEnabled = false ;
        private bool m_OverLoadBeepEnabled = false;
        private bool mShowMaxMinValues;
        private double m_MaxLeftDB;
        private double m_MaxRightDB;
        private double m_MinLeftDB;
        private double m_MinRightDB;
        private int m_AfterGoodCount;
        

        public TextVUMeterPanel()
        {
            InitializeComponent();
                        mResetButton.Enabled = false;
            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";
            m_strLeftLowLevelIndicator = "";
            m_strRightLowLevelIndicator = "";
            mShowMaxMinValues = false;
            m_BeepEnabled = false;
        }

        public AudioLib.VuMeter VuMeter
        {
            get
            {
                return m_VuMeter;
            }
            set
            {
                if (m_VuMeter != null)
                {
                    m_VuMeter.PeakMeterOverloaded -= new AudioLib.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
                    m_VuMeter.PeakMeterUpdated -= new AudioLib.VuMeter.PeakMeterUpdateHandler(CatchPeakMeterUpdateEvent);

                    m_VuMeter.ResetEvent -= new AudioLib.VuMeter.ResetHandler(VuMeter_ResetEvent);

                    m_VuMeter.LevelTooLowEvent -= new AudioLib.VuMeter.LevelTooLowHandler(CatchLevelTooLowEvent);
                    m_VuMeter.LevelGoodEvent -= new AudioLib.VuMeter.LevelGoodHandler(PlayLevelGoodSound);

                }
                m_VuMeter = value;

                if (m_VuMeter  != null)
                {
                    m_VuMeter.EnableAudioLevelAlerts = true;
                    m_VuMeter.PeakMeterOverloaded += new AudioLib.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
                    m_VuMeter.PeakMeterUpdated += new AudioLib.VuMeter.PeakMeterUpdateHandler(CatchPeakMeterUpdateEvent);

                    m_VuMeter.ResetEvent += new AudioLib.VuMeter.ResetHandler(VuMeter_ResetEvent);

m_VuMeter.LevelTooLowEvent += new AudioLib.VuMeter.LevelTooLowHandler(CatchLevelTooLowEvent);
m_VuMeter.LevelGoodEvent += new AudioLib.VuMeter.LevelGoodHandler ( PlayLevelGoodSound );

                    m_MaxLeftDB = -100.00;
                    m_MaxRightDB = -100.00;
                    mResetButton.Enabled = mShowMaxMinValues;
                    m_AfterGoodCount = 0;
                                                        }
            }
        }

        public void CatchPeakMeterUpdateEvent(object sender, AudioLib.VuMeter.PeakMeterUpdateEventArgs e)
        {
            //double channelValueLeft = 0;
            //double channelValueRight = 0;

            //if (e.PeakDb != null && e.PeakDb.Length > 0)
            //{
            //    channelValueLeft = e.PeakDb[0];

            //    if (e.PeakDb.Length > 1)
            //    {
            //        channelValueRight = e.PeakDb[1];
            //    }
            //    else
            //    {
            //        channelValueRight = channelValueLeft;
            //    }

            //    if (channelValueLeft == Double.PositiveInfinity
            //        && e.PeakDb.Length > 1
            //        && channelValueRight == Double.PositiveInfinity)
            //    {
            //        VuMeter_ResetEvent();
            //        return;
            //    }
            //}
        }

        public bool BeepEnable
        {
            get { return m_BeepEnabled; }
            set { m_BeepEnabled = value; }
        }

        public bool ShowMaxMinValues        
        {
            get { return mShowMaxMinValues; }
            set
            {
                mShowMaxMinValues = value;
                if (value) mResetButton.Enabled = true;
            }
        }


        private void tmUpdateText_Tick(object sender, EventArgs e)
        {
            
                        if (m_VuMeter != null)
            {
                            if (
//TODO: implement this !!!
!m_VuMeter.IsLevelTooLow||
                                mShowMaxMinValues)
                        UpdateRunningValues();
                    else 
                        UpdateRunningLowValues();

                    if (m_StrLeftOverloadIndicator == Localizer.Message ("TextVuMeter_OverloadIndicator" )
                        || m_StrRightOverloadIndicator == Localizer.Message ( "TextVuMeter_OverloadIndicator" ))
                        {
                        mResetButton.Enabled = true;
                        }
                                                               }
            
                                                           m_AfterGoodCount++;
        }

        private void UpdateRunningValues()
        {
            double LeftDb = 0;
            double RightDb = 0;
            
            if (VuMeter.LastPeakDb != null)
            {//1
                
                if (VuMeter.LastPeakDb.Length > 0 && VuMeter.LastPeakDb[0] != Double.PositiveInfinity)
                    LeftDb = VuMeter.LastPeakDb[0];
                
                if (VuMeter.LastPeakDb.Length > 1 && VuMeter.LastPeakDb[1] != Double.PositiveInfinity)
                    RightDb = VuMeter.LastPeakDb[1];

                if (LeftDb > 0)
                    LeftDb = 0.0;

                if (RightDb > 0)
                    RightDb = 0.0;

            }//-1
            if (!mShowMaxMinValues)
            {//1
                mLeftBox.Text = m_StrLeftOverloadIndicator + LeftDb.ToString();
                if (m_VuMeter.LastPeakDb != null && m_VuMeter.LastPeakDb.Length > 1)
                {
                    mRightBox.Text = m_StrRightOverloadIndicator + RightDb.ToString();
                }
                else mRightBox.Text  = "--";
            }//-1
            else   // show extreme high and expreme low
                SetExtremeValues(LeftDb, RightDb );

            if (m_OverLoadBeepEnabled)
            {
                PlayBeep();
                // only this function can set OverLoadbeep enable to false 
                m_OverLoadBeepEnabled = false;
            }
        }

        private void UpdateRunningLowValues()
        {
            //if (m_VuMeter != null && m_VuMeter.IsLevelTooLow)
            {
                if (m_VuMeter.LastPeakDb != null && m_VuMeter.LastPeakDb[0] != Double.PositiveInfinity)
                {
                    mLeftBox.Text = m_strLeftLowLevelIndicator + m_VuMeter.LastPeakDb[0].ToString();
                }
                else mLeftBox.Text = "--";

                if (m_VuMeter.LastPeakDb != null && m_VuMeter.LastPeakDb.Length > 1 && m_VuMeter.LastPeakDb[1] != Double.PositiveInfinity)
                {
                    mRightBox.Text = m_strRightLowLevelIndicator + m_VuMeter.LastPeakDb[1].ToString();
                }
                else mRightBox.Text = "--";
            }
        }


        // load the beep file and plays it once in case of overload
        private void PlayBeep()
        {
        FileInfo BeepFile = new FileInfo ( Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "hi.wav" ) );
        if (BeepFile.Exists && m_BeepEnabled && m_AfterGoodCount >= -3)
            {
                System.Media.SoundPlayer PlayBeep = new System.Media.SoundPlayer(BeepFile.FullName);
                PlayBeep.Play();
            }
        }
        
        
        private void SetExtremeValues( double MaxLeftDB , double MaxRightDB ) 
        {
            // set textbox for left channel
            if (MaxLeftDB > m_MaxLeftDB)
                m_MaxLeftDB = MaxLeftDB;
            
                            string strMaxLeftDB = m_MaxLeftDB.ToString();  
            if ( strMaxLeftDB.Length > 5 )
                strMaxLeftDB = strMaxLeftDB.Substring(0, 5);


            string strMinLeftDB = m_MinLeftDB.ToString();
                        if ( strMinLeftDB.Length >5 )
                strMinLeftDB = strMinLeftDB.Substring(0, 5);

            mLeftBox.Text = m_StrLeftOverloadIndicator + strMaxLeftDB+ "/" + m_strLeftLowLevelIndicator + strMinLeftDB;
            
            // set text for right channel
            if (m_VuMeter.LastPeakDb != null && m_VuMeter.LastPeakDb.Length > 1)
            {
                if (MaxRightDB > m_MaxRightDB)
                    m_MaxRightDB = MaxRightDB;

                string strMaxRightDB = m_MaxRightDB.ToString();
                if (strMaxRightDB.Length > 5)
                    strMaxRightDB = strMaxRightDB.Substring(0, 5);

                strMinLeftDB = m_MinRightDB.ToString();

                if (strMinLeftDB.Length > 5)
                    strMinLeftDB = strMinLeftDB.Substring(0, 5);

                mRightBox.Text = m_StrRightOverloadIndicator + strMaxRightDB + "/" + m_strLeftLowLevelIndicator + strMinLeftDB ;
            }
            else
                mRightBox.Text = "--" ;
        }

        void CatchPeakOverloadEvent(object sender, VuMeter.PeakOverloadEventArgs EventOb)
        {
            //Obi.Events.Audio.VuMeter.PeakOverloadEventArgs EventOb = e as Obi.Events.Audio.VuMeter.PeakOverloadEventArgs;
            if (EventOb.Channel == 1)
                //m_StrLeftOverloadIndicator = "OL ";
                m_StrLeftOverloadIndicator = Localizer.Message ( "TextVuMeter_OverloadIndicator" );
            else
                //m_StrRightOverloadIndicator = "OL ";
                m_StrRightOverloadIndicator = Localizer.Message ( "TextVuMeter_OverloadIndicator" );

            //UpdateControls ();
            AudioLib.VuMeter ob_VuMeter = sender as AudioLib.VuMeter;


            // beep enabled false means this is first peak overload after text timer tick, so play beep
            if (m_OverLoadBeepEnabled== false)
            {
                PlayBeep();
                m_OverLoadBeepEnabled = true;
            }
        }

        private void CatchLevelTooLowEvent(object sender, AudioLib.VuMeter.LevelTooLowEventArgs e)
        {
        if (m_AfterGoodCount >= 0)
            {
            m_MinLeftDB = e.LowLevelValue;
            if (m_VuMeter.LastPeakDb != null && m_VuMeter.LastPeakDb.Length > 1)
            {
                m_MinRightDB = e.LowLevelValue;
            }


            //m_strLeftLowLevelIndicator = "Low:";
             //m_strRightLowLevelIndicator = "Low:";
             m_strLeftLowLevelIndicator = Localizer.Message ( "TextVuMeter_LowIndicator" );
             m_strRightLowLevelIndicator = Localizer.Message ( "TextVuMeter_LowIndicator" );
            PlayLevelTooLowBeep ();
            }
        }


        private delegate void UpdateControlsCallBack () ;

        private void UpdateControls () 
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateControlsCallBack( UpdateControls) )  ;
            }
            else
            {
                mResetButton.Enabled = true;
            }

        }


        private void CatchStateChangedEvent(object sender, EventArgs e){}
            delegate     void StateChangeCallBack () ;

        private void StateChangeFunction ()
        {
            if (InvokeRequired)
            {
                Invoke(new StateChangeCallBack(StateChangeFunction));
            }
            else
            { }
        }

        private void StateChangeOperations(){}

        private void mResetButton_Click(object sender, EventArgs e)
        {
            if ( !mShowMaxMinValues )
            mResetButton.Enabled = false ;

            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";
            m_strLeftLowLevelIndicator = "";
            m_strRightLowLevelIndicator = "";

            if (m_VuMeter != null  )
            {
                //m_VuMeter.Reset();
                VuMeter_ResetEvent();
            }

            if (mShowMaxMinValues)
            {
                mLeftBox.Text = "";
                mRightBox.Text = "";
                m_MaxLeftDB = -100.00;
                m_MaxRightDB = -100.00;
                m_MinLeftDB = 0;
                m_MinRightDB = 0;
            }
        m_AfterGoodCount = 0;
                }

        private delegate  void   SetTextBoxCallBack  () ;

        private void VuMeter_ResetEvent(object sender, EventArgs e) { VuMeter_ResetEvent(); }
        private void VuMeter_ResetEvent() // object sender  , EventArgs e )
        {
            m_MaxLeftDB = -100.00 ;
            m_MaxRightDB = -100;
            m_MinLeftDB = 0;
            m_MinRightDB = 0;
            
            SetResetText();
            m_AfterGoodCount = 0;
                    }

        private void SetResetText()
        {
            if (InvokeRequired)
            {
                Invoke(new  SetTextBoxCallBack  ( SetResetText ));
            }
            else
            {
                mLeftBox.Text = m_StrLeftOverloadIndicator;
                mRightBox.Text = m_StrRightOverloadIndicator;
            }
        }

        private void PlayLevelTooLowBeep()
        {
            string FilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "low.wav");
                                    if (File.Exists(FilePath) && m_BeepEnabled)
            {
                System.Media.SoundPlayer LowBeepPlayer  = new System.Media.SoundPlayer(FilePath);
                LowBeepPlayer.Play();
            }
        }

        private void PlayLevelGoodSound ( object sender , EventArgs e)
            {
            string FilePath = Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "good.wav" );
            if (File.Exists ( FilePath ) && m_BeepEnabled)
                {
                System.Media.SoundPlayer LevelGoodSoundpPlayer = new System.Media.SoundPlayer ( FilePath );
                LevelGoodSoundpPlayer.Play ();
                }

            m_AfterGoodCount = -5 ;
                        }


    }// end of class
}
