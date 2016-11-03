using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PipelineInterface.ParameterControls
    {
    public partial class EnumControl : BaseUserControl
        {
        private ScriptParameter m_Parameter;
        private DataTypes.EnumDataType m_EnumData;

        public EnumControl ()
            {
            InitializeComponent ();
            }

        public EnumControl ( ScriptParameter p,string ObiFont )
            : this ()
            {
            m_Parameter = p;
            m_EnumData = (DataTypes.EnumDataType)p.ParameterDataType;

            mNiceNameLabel.Text = GetLocalizedString (p.NiceName);
            mComboBox.AccessibleName =GetLocalizedString( p.NiceName);
            base.DescriptionLabel =GetLocalizedString(  p.Description);

            DataTypes.EnumDataType EnumData = (DataTypes.EnumDataType)p.ParameterDataType;
            foreach (string s in EnumData.GetNiceNames)
                {
                mComboBox.Items.Add ( s );
                }
            
            base.Size = this.Size;
            if (m_EnumData.SelectedIndex >= 0 && m_EnumData.SelectedIndex < m_EnumData.GetValues.Count)
                mComboBox.SelectedIndex = m_EnumData.SelectedIndex;
            if (ObiFont != this.Font.Name)
            {
                this.Font = new Font(ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
            }


        private void ComboboxControl_Load ( object sender, EventArgs e )
            {

            }

        public override void UpdateScriptParameterValue ()
            {
            try
                {
                m_EnumData.SelectedIndex = mComboBox.SelectedIndex;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void mNiceNameLabel_LocationChanged ( object sender, EventArgs e )
            {
            // this is work around to fix miss alignment of these controls. will have to look in flow layout to provide long term fix
            if (mNiceNameLabel.Location.X > 20)
                {
                mNiceNameLabel.Location = new Point ( 6, mNiceNameLabel.Location.Y );
                mComboBox.Location = new Point ( 20 + mNiceNameLabel.Size.Width, mComboBox.Location.Y );
                }
            }


        }
    }
