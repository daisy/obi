using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace PipelineInterface
{
    // a common class for manipulating pipeline script files
    // Only this class interacts with physical script files
    class ScriptParser
    {
        private string mScriptFilePath;                // path to the script file
        private List<ScriptParameter> mParameterList;  // list of script parameters
        private string m_Name; // name of script
        private string mNiceName;                      // nice name for the script itself

        public ScriptParser(string ScriptPath)
        {
            mParameterList = new List<ScriptParameter>();
            mScriptFilePath = ScriptPath;
            XmlTextReader reader = new XmlTextReader(mScriptFilePath);
            reader.XmlResolver = null;
            XmlDataDocument doc = new XmlDataDocument();
            doc.XmlResolver = null;
            doc.Load(reader);
            reader.Close();
            XmlNode taskScriptNode = doc.DocumentElement;
            if (taskScriptNode.Attributes.GetNamedItem ( "name" ) != null)
                {
                m_Name = taskScriptNode.Attributes.GetNamedItem ( "name" ).Value; 
                }
            mNiceName = GetScriptNiceName(doc, mScriptFilePath);
            PopulateParameterList(doc);
        }

        /// <summary>
        /// Returns name of script. it should be unique
        /// </summary>
        public string Name { get { return m_Name; } }

        /// <summary>
        /// Get the nice name of the script.
        /// </summary>
        public string NiceName { get { return mNiceName; } }

        /// <summary>
        /// summary
        /// List of parameters available in script
        /// </summary>
        public List<ScriptParameter> ParameterList
        {
            get { return mParameterList; }
        }


        // Find the script nice name; or, by default, the name from the file name.
        private string GetScriptNiceName(XmlDataDocument doc, string path)
        {
            XmlNodeList tasks = doc.GetElementsByTagName("taskScript");
            if (tasks.Count > 0)
            {
                foreach (XmlNode child in tasks[0].ChildNodes)
                {
                    if (child.LocalName == "nicename") return child.InnerText;
                }
            }
            return Path.GetFileNameWithoutExtension(path);
        }

        //  populate parameter list
        private void PopulateParameterList(XmlDataDocument doc)
        {
            XmlNodeList CompleteNodeList = doc.GetElementsByTagName("parameter");
            ScriptParameter p = null;
            foreach (XmlNode n in CompleteNodeList)
            {
                if (n.Attributes.Count > 0)
                {
                    p = new ScriptParameter(n);
                    mParameterList.Add(p);
                }
            }
        }



        /// <summary>
        ///  executes script
        /// </summary>
        public void ExecuteScript()
        {
            foreach (ScriptParameter p in ParameterList)
            {
                if (p.IsParameterRequired
                    && (p.ParameterValue == null || p.ParameterValue == ""))
                {
                    throw new System.Exception(Localizer.Message("Pipeline_InvalidScriptsParameters"));
                }
            }

            string Param = "";
            foreach (ScriptParameter p in ParameterList)
            {
                if (p.IsParameterRequired)
                {
                    Param = Param + " \"" + p.Name + "=" + p.ParameterValue + "\"";
                }
            }
            // invoke the script
            string PipelineFilePath = Path.Combine(Directory.GetParent(mScriptFilePath).Parent.FullName, "pipeline-lite.bat");
            Process PipelineProcess = new Process();
            PipelineProcess.StartInfo.CreateNoWindow = true;
            PipelineProcess.StartInfo.ErrorDialog = true;
            PipelineProcess.StartInfo.UseShellExecute = false;

            PipelineProcess.StartInfo.FileName = PipelineFilePath;
            PipelineProcess.StartInfo.Arguments = " -x -s \"" + mScriptFilePath + "\" -p" + Param;
            PipelineProcess.StartInfo.WorkingDirectory = Directory.GetParent(Directory.GetParent(mScriptFilePath).FullName).FullName;

            try
            {
                PipelineProcess.Start();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return;
            }
            PipelineProcess.WaitForExit();
            //System.Windows.Forms.MessageBox.Show("Task completed");
        }
    }
}
