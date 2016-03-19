using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PipelineInterface
{
    /// <summary>
    /// Base dialog for the pipeline interface.
    /// </summary>
    public partial class PipelineInterfaceForm : Form
    {
        private string mInputPath;
        private string mProjectDirectory;
        private ScriptParser mParser;
        private string m_ObiFont;


        /// <summary>
        /// Used by the designer.
        /// </summary>
        public PipelineInterfaceForm() { InitializeComponent(); }

        /// <summary>
        /// Create a dialog for the pipeline script given as input. The path to the exported project
        /// as well as the directory of the project are also given as initial parameters.
        /// </summary>
        public PipelineInterfaceForm(string scriptPath, string inputPath, string ProjectDirectory, string ObiFont) //@fontconfig
            : this()
        {
            if (!File.Exists(scriptPath)) throw new Exception(string.Format(Localizer.Message("no_script"), scriptPath));
            mParser = new ScriptParser(scriptPath);
            
            if (File.Exists(inputPath))
            {
                mInputPath = inputPath;
            }
            else
            {
                mInputPath = "";
                MessageBox.Show(Localizer.Message("no_primary_export_directory"));
            }
            mProjectDirectory = ProjectDirectory;
            Text = mParser.NiceName;
            m_ObiFont = ObiFont;
            if (ObiFont != this.Font.Name)
            {
                this.Font = new Font(ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }



        private void PipelineInterfaceForm_Load(object sender, EventArgs e)
        {
            int tabIndex = 0;
            int w = 0;
            int h = 0;
            foreach (ScriptParameter p in mParser.ParameterList)
            {
                Control c =
                    p.ParameterDataType is DataTypes.BoolDataType ? (Control)new ParameterControls.BoolControl(p, m_ObiFont) :
                    p.ParameterDataType is DataTypes.IntDataType ? (Control)new ParameterControls.IntControl(p, m_ObiFont) :
                    p.ParameterDataType is DataTypes.StringDataType ? (Control)new ParameterControls.StringControl(p, m_ObiFont) :
                    p.ParameterDataType is DataTypes.EnumDataType ? (Control)new ParameterControls.EnumControl(p, m_ObiFont) :
                        (Control)new ParameterControls.PathBrowserControl(p, mInputPath, mProjectDirectory, m_ObiFont);
                mLayoutPanel.Controls.Add(c);
                mLayoutPanel.SetFlowBreak(c, true);
                c.TabIndex = tabIndex++;
                if (w < c.Width + c.Margin.Horizontal) w = c.Width + c.Margin.Horizontal;
                h += c.Height + c.Margin.Vertical;
            }
            int wdiff = w - mLayoutPanel.Width;
            int hdiff = h - mLayoutPanel.Height;
            mLayoutPanel.Size = new Size(w, h);
            foreach (Control c in mLayoutPanel.Controls) c.Width = mLayoutPanel.Width - c.Margin.Horizontal;
            Size = new Size(Width + wdiff, Height + hdiff);
        }

        public void RunScript()
        {
            for (int i = 0; i < mLayoutPanel.Controls.Count; i++)
            {
                if (mLayoutPanel.Controls[i] is ParameterControls.BaseUserControl)
                {
                    ((ParameterControls.BaseUserControl)mLayoutPanel.Controls[i]).UpdateScriptParameterValue();
                }
            }
            mParser.ExecuteScript();
            Close();
        }
    }
}