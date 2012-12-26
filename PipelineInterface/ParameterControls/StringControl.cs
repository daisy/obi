using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PipelineInterface.ParameterControls
    {
    public partial class StringControl : BaseUserControl
        {
        private ScriptParameter m_Parameter;
        private DataTypes.StringDataType m_StringData;

        public StringControl ()
            {
            InitializeComponent ();
            }

        public StringControl ( ScriptParameter p )
            : this ()
            {
            m_Parameter = p;
            m_StringData = (DataTypes.StringDataType)p.ParameterDataType;

            label1.Text = GetLocalizedString( p.NiceName);
            textBox1.AccessibleName =GetLocalizedString( p.Description );
            if (p.ParameterValue != null) textBox1.Text = p.ParameterValue;

            base.DescriptionLabel =GetLocalizedString( p.Description);




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

        public override void UpdateScriptParameterValue ()
            {
            try
                {
                if (textBox1.Text != "")  m_StringData.Value = textBox1.Text;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void textBox1_TextChanged ( object sender, EventArgs e )
            {
            if (textBox1.Text != "" && m_StringData.RegularExpression != ""
            && Regex.IsMatch ( textBox1.Text, m_StringData.RegularExpression, RegexOptions.None ))
            {
            MessageBox.Show ( Localizer.Message ( "Pipeline_InvalidString" ) );
                }
            }

            

        }
    }
