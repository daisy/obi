using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PipelineInterface.ParameterControls
{
    public partial class BoolControl : BaseUserControl
    {
        ScriptParameter m_Parameter;
        DataTypes.BoolDataType m_boolDataType;

        public BoolControl()
        {
            InitializeComponent();
                    }

        public BoolControl(ScriptParameter p)
            : this()
        {
            base.DescriptionLabel = GetLocalizedString( p.Description);
            checkBox1.Text =GetLocalizedString( p.NiceName);
            m_Parameter = p;
            m_boolDataType = (DataTypes.BoolDataType)p.ParameterDataType;
            checkBox1.Checked = m_boolDataType.Value;
        }

        public override void UpdateScriptParameterValue()
        {
                        try
            {
                m_boolDataType.Value = checkBox1.Checked;    
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
            }
}
