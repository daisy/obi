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
        private string mInputPath;
        private string mProjectDirectory;
        private ScriptParser mParser;

        public PipelineInterfaceForm() { InitializeComponent(); }

        public PipelineInterfaceForm(string scriptPath, string inputPath, string ProjectDirectory)
            : this()
        {
            if (!File.Exists(scriptPath)) throw new Exception(string.Format(Localizer.Message("no_script"), scriptPath));
            mParser = new ScriptParser(scriptPath);
            if (File.Exists ( inputPath ))
                mInputPath = inputPath;
            else
                {
                mInputPath = "";
                MessageBox.Show (Localizer.Message("NoPrimaryExportDirectory"));
                                                }

            mProjectDirectory = ProjectDirectory;
            FileInfo f = new FileInfo(scriptPath);
            this.Text = f.Name.Replace(f.Extension, "");
        }


        private void PipelineInterfaceForm_Load(object sender, EventArgs e)
        {
            int tabIndex = 0;
            int w = 0;
            int h = 0;
            foreach (ScriptParameter p in mParser.ParameterList)
            {
                Control c =
                    p.ParameterDataType is DataTypes.BoolDataType ? (Control)new ParameterControls.BoolControl(p) :
                    p.ParameterDataType is DataTypes.IntDataType ? (Control)new ParameterControls.IntControl( p ) :
                    p.ParameterDataType is DataTypes.StringDataType ? (Control)new ParameterControls.StringControl( p ) :
                    p.ParameterDataType is DataTypes.EnumDataType ? (Control)new ParameterControls.EnumControl(p) :
                        (Control)new ParameterControls.PathBrowserControl(p, mInputPath, mProjectDirectory);
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