using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.PipelineInterface.ParameterControls
{
    public partial class EnumControl : BaseUserControl
    {
        private ScriptParameter m_Parameter;
        private DataTypes.EnumDataType m_EnumData;

        public EnumControl()
        {
            InitializeComponent();
        }

        public EnumControl( ScriptParameter p)
            : this()
        {
            m_Parameter = p ;
            m_EnumData = (DataTypes.EnumDataType) p.ParameterDataType ;

            mNiceNameLabel.Text =  p.NiceName;
            mComboBox.AccessibleName = p.NiceName;
            base.DescriptionLabel = p.Description;

            DataTypes.EnumDataType EnumData = (DataTypes.EnumDataType)p.ParameterDataType;
            foreach (string s in EnumData.GetNiceNames)
            {
                mComboBox.Items.Add(s);
            }

            base.Size = this.Size;
            if ( m_EnumData.SelectedIndex >= 0 && m_EnumData.SelectedIndex < m_EnumData.GetValues.Count )
            mComboBox.SelectedIndex = m_EnumData.SelectedIndex;
        }


        private void ComboboxControl_Load(object sender, EventArgs e)
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
                MessageBox.Show(ex.ToString());
            }
        }


    }
}
