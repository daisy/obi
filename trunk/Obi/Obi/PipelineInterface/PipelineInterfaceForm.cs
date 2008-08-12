using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.PipelineInterface
{
    public partial class PipelineInterfaceForm : Form
    {
        public PipelineInterfaceForm()
        {
            InitializeComponent();
        }

        private string m_InputPath;
        private string m_ProjectDirectory;
        private ScriptParser m_Parser  ;
        private int m_YCordinate;

        public PipelineInterfaceForm (string scriptPath, string inputPath, string ProjectDirectory)
                        : this()
        {
                                    if (!File.Exists(scriptPath)) throw new Exception(string.Format(Localizer.Message("no_script"), scriptPath));
            m_Parser = new ScriptParser(scriptPath);

            m_InputPath = inputPath;
            m_ProjectDirectory = ProjectDirectory;
            m_YCordinate = 0;

            FileInfo f = new FileInfo(scriptPath) ;
            this.Text = f.Name.Replace(f.Extension, "");
    }


        private void PipelineInterfaceForm_Load(object sender, EventArgs e)
        {
            CreateControlsDynamically();
        }

// creates controls dynamically from datatype of script parameter
        public void CreateControlsDynamically()
        {
            int Tab_Index = 0;
    
            foreach (ScriptParameter p in m_Parser.ParameterList)
            {
                if (p.ParameterDataType is DataTypes.PathDataType)
                {
                    CreatePathBrowserControl(p, Tab_Index, m_YCordinate);
                }
                else if (p.ParameterDataType is DataTypes.EnumDataType)
                {
                    CreateEnumControl(p,Tab_Index,m_YCordinate);
                }

                    Tab_Index++;
                    

                }
                // update ok button and cancel button
                m_btnOk.Location = new Point(this.Size.Width - (m_btnOk.Size.Width + m_btnOk.Size.Width + 10), m_YCordinate);
                m_btnCancel.Location = new Point(this.Size.Width - (m_btnOk.Size.Width), m_YCordinate);
                m_btnOk.TabIndex = Tab_Index;
                m_btnCancel.TabIndex = Tab_Index + 1;

                this.Size = new Size(this.Size.Width, this.Size.Height + m_btnCancel.Size.Height + 20);
                                                    }

        private void CreatePathBrowserControl(ScriptParameter p,  int Tab_Index, int m_YCordinate)
        {
            ParameterControls.PathBrowserControl PC = new Obi.PipelineInterface.ParameterControls.PathBrowserControl(p, m_InputPath , m_ProjectDirectory );
            PC.Location = new Point(0,m_YCordinate );
            PC.Visible = true;
            PC.TabIndex = Tab_Index;
            Controls.Add(PC);


            UpdateFormSize(PC);    
        }

        private void CreateEnumControl(ScriptParameter p , int Tab_Index , int m_YCordinate)
        {
            ParameterControls.EnumControl PC = new Obi.PipelineInterface.ParameterControls.EnumControl(p);
            PC.Location = new Point(0, m_YCordinate);
            PC.Visible = true;
            PC.TabIndex = Tab_Index;
            Controls.Add(PC);

            UpdateFormSize(PC);
        }

        private void UpdateFormSize(Control PC)
        {
                        m_YCordinate = m_YCordinate + PC.Size.Height;

            if (PC.Size.Width > this.Size.Width || this.Size.Height < m_YCordinate)
                this.Size = new Size(PC.Size.Width, m_YCordinate);
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is ParameterControls.BaseUserControl)
                    ((ParameterControls.BaseUserControl)Controls[i]).UpdateScriptParameterValue();
                              
            }
            //foreach (ScriptParameter p in m_Parser.ParameterList)
                //MessageBox.Show(p.ParameterValue);


            m_Parser.ExecuteScript();
                        Close();
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}