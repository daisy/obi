using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.PipelineInterface.ParameterControls
    {
    public partial class IntControl : BaseUserControl
        {
        private ScriptParameter m_Parameter;
        private DataTypes.IntDataType m_IntData;

        public IntControl ()
            {
            InitializeComponent ();
            }

        public IntControl ( ScriptParameter p )
            : this ()
            {
            m_Parameter = p;

            m_IntData =(DataTypes.IntDataType) p.ParameterDataType;
            label1.Text = p.NiceName;
            textBox1.AccessibleName = p.Description;
            if ( p.ParameterValue != null )  textBox1.Text = p.ParameterValue;

            base.DescriptionLabel = p.Description;
            

            int wdiff = label1.Width;
                        wdiff -= label1.Width;
            if (wdiff < 0)
                {
                Point location = label1.Location;
                Width -= wdiff;
                label1.Location = location;
                }
            else
                {
                label1.Location = new Point ( label1.Location.X - wdiff, label1.Location.Y );
                }
            if (mLabel.Width + mLabel.Margin.Horizontal > Width) Width = mLabel.Width + mLabel.Margin.Horizontal;
            base.Size = this.Size;
            }

        private void textBox1_TextChanged ( object sender, EventArgs e )
            {
                        if (textBox1.Text != "" && textBox1.Text != "-")
                {
                int val = Convert.ToInt32 ( textBox1.Text );

                if (val > m_IntData.Max || val < m_IntData.Min)
                    {
                    textBox1.Text = m_IntData.Min.ToString ();
                    MessageBox.Show ( string.Format ( Localizer.Message ( "Pipeline_Error_NotExpectedRange" ), m_IntData.Min.ToString (), m_IntData.Max.ToString () ) );
                    }
                }
                                     }

        public override void UpdateScriptParameterValue ()
            {
            try
                {
                m_IntData.Value = int.Parse ( textBox1.Text );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        }
    }
