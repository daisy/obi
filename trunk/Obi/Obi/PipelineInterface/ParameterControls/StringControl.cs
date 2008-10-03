using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Obi.PipelineInterface.ParameterControls
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

            label1.Text = p.NiceName;
            textBox1.AccessibleName = p.Description ;
            if (p.ParameterValue != null) textBox1.Text = p.ParameterValue;

            base.DescriptionLabel = p.Description;
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
