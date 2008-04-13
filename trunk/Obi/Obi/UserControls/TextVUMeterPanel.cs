using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.UserControls
{
    public partial class TextVUMeterPanel : UserControl
    {
        // member variables
        private Audio.VuMeter m_VuMeter;    // Instance of VuMeter 
                private String m_StrLeftOverloadIndicator;
        private String m_StrRightOverloadIndicator;
        private bool m_BeepEnabled = false;
        private bool m_ShowMaxMinValues;
        private double m_MaxLeftDB;
        private double m_MaxRightDB;

        public TextVUMeterPanel()
        {
            InitializeComponent();
                        mResetButton.Enabled = false;
            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";
            m_ShowMaxMinValues = false ;

        }

        public Audio.VuMeter VuMeter
        {
            get
            {
                return m_VuMeter;
            }
            set
            {
                m_VuMeter = value;

                if (m_VuMeter  != null)
                {
                    m_VuMeter.PeakOverload += new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent );
                    //m_VuMeter.UpdateForms += new Events.Audio.VuMeter.UpdateFormsHandler(CatchUpdateForms);
                    m_VuMeter.ResetEvent += new Events.Audio.VuMeter.ResetHandler(VuMeter_ResetEvent);
                    m_MaxLeftDB = -100.00;
                    m_MaxRightDB = -100.00;
                    mResetButton.Enabled = m_ShowMaxMinValues;
                                    }
            }
        }

        public bool ShowMaxMinValues        
        {
            get { return m_ShowMaxMinValues; }
            set
            {
                if (value != null)
                    m_ShowMaxMinValues = value;

                if (m_ShowMaxMinValues)
                    mResetButton.Enabled = true;
            }
        }


        private void tmUpdateText_Tick(object sender, EventArgs e)
        {
            if (m_VuMeter != null)
            {
                double LeftDb = 0;
                double RightDb = 0;
                if (VuMeter.PeakDbValue != null)
                {
                                        if (VuMeter.PeakDbValue.Length > 0)
                        LeftDb = VuMeter.PeakDbValue[0];

                    if (VuMeter.PeakDbValue.Length > 1)
                        RightDb = VuMeter.PeakDbValue[1];
                    
                    if (LeftDb > 0 )
                        LeftDb = 0.0;

                    if (RightDb >  0 )
                        RightDb = 0.0;
                                        
                }
                if (!m_ShowMaxMinValues)
                {
                    mLeftBox.Text = m_StrLeftOverloadIndicator + LeftDb.ToString();
                    mRightBox.Text = m_StrRightOverloadIndicator + RightDb.ToString();
                }
                else
                SetExtremeValues(LeftDb, RightDb);
            }  
            if (m_BeepEnabled)
            {
                PlayBeep();
                // only this function can set beep enable to false 
                m_BeepEnabled = false;
            }
        }

        // load the beep file and plays it once in case of overload
        private void PlayBeep()
        {
            FileInfo BeepFile = new FileInfo("Beep.wav");
            if (BeepFile.Exists)
            {
                System.Media.SoundPlayer PlayBeep = new System.Media.SoundPlayer("Beep.wav");
                PlayBeep.Play();
            }
        }
        
        
        private void SetExtremeValues( double LeftDB , double RightDB) 
        {
            // set textbox for left channel
            if (LeftDB > m_MaxLeftDB)
                m_MaxLeftDB = LeftDB;

                            string strMaxLeftDB = m_MaxLeftDB.ToString();  
            if ( strMaxLeftDB.Length > 5 )
                strMaxLeftDB = strMaxLeftDB.Substring(0, 5);


            mLeftBox.Text = m_StrLeftOverloadIndicator + strMaxLeftDB;
            
            // set text for right channel
            if (RightDB > m_MaxRightDB)
                m_MaxRightDB = RightDB;

            string strMaxRightDB = m_MaxRightDB.ToString();
            if (strMaxRightDB.Length > 5)
                strMaxRightDB = strMaxRightDB.Substring(0, 5);


            mRightBox.Text = m_StrRightOverloadIndicator+ strMaxRightDB ;
            
        }

        void CatchPeakOverloadEvent(object sender, EventArgs e)
        {
            Obi.Events.Audio.VuMeter.PeakOverloadEventArgs EventOb = e as Obi.Events.Audio.VuMeter.PeakOverloadEventArgs;
            if (EventOb.Channel == 1)
                m_StrLeftOverloadIndicator = "OL ";
            else
                m_StrRightOverloadIndicator = "OL ";

            Overload();
            Audio.VuMeter ob_VuMeter = sender as Audio.VuMeter;


            // beep enabled false means this is first peak overload after text timer tick, so play beep
            if (m_BeepEnabled == false)
            {
                PlayBeep();
                m_BeepEnabled = true;
            }
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
            
        }

        private void mResetButton_Click(object sender, EventArgs e)
        {
            if ( !m_ShowMaxMinValues )
            mResetButton.Enabled = false ;

            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";

            if (m_VuMeter != null  )
            {
                m_VuMeter.Reset();
            }

            if (m_ShowMaxMinValues)
            {
                mLeftBox.Text = "";
                mRightBox.Text = "";
                m_MaxLeftDB = -100.00;
                m_MaxRightDB = -100.00;
            }
        }

        private delegate  void   SetTextBoxCallBack  () ;

        private void VuMeter_ResetEvent( object sender  , EventArgs e )
        {
            m_MaxLeftDB = -100.00 ;
            
            SetResetText();
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


    }// end of class
}
