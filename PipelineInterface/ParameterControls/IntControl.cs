using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PipelineInterface.ParameterControls
{
    public partial class IntControl : BaseUserControl
    {
        private ScriptParameter m_Parameter;
        private DataTypes.IntDataType m_IntData;

        public IntControl()
        {
            InitializeComponent();
        }

        public IntControl(ScriptParameter p,string ObiFont )
            : this()
        {
            m_Parameter = p;
            m_IntData = (DataTypes.IntDataType)p.ParameterDataType;

            mNiceNameLabel.Text = GetLocalizedString( p.NiceName);

            mIntBox.AccessibleName =GetLocalizedString( p.Description);
            if (p.ParameterValue != null) mIntBox.Text = p.ParameterValue;
            base.DescriptionLabel =GetLocalizedString ( p.Description);

            int x_IntBox = mNiceNameLabel.Location.X + mNiceNameLabel.Width + mNiceNameLabel.Margin.Right + mIntBox.Margin.Left;
            mIntBox.Location = new Point(x_IntBox, mIntBox.Location.Y);
            if (ObiFont != this.Font.Name)
            {
                this.Font = new Font(ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }

        private void mIntBox_TextChanged(object sender, EventArgs e)
        {
            if (mIntBox.Text != "" && mIntBox.Text != "-")
            {
            int val = m_IntData.Value ;
            try
                {
                val = Convert.ToInt32 ( mIntBox.Text );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                mIntBox.Text = m_IntData.Value.ToString () ;
                                }

                if (val > m_IntData.Max || val < m_IntData.Min)
                {
                    mIntBox.Text = m_IntData.Min.ToString();
                    MessageBox.Show(string.Format(Localizer.Message("Pipeline_Error_NotExpectedRange"), m_IntData.Min.ToString(), m_IntData.Max.ToString()));
                }
            }
        }

        public override void UpdateScriptParameterValue()
        {
            try
            {
                m_IntData.Value = int.Parse(mIntBox.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
