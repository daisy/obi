using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace Obi.PipelineInterface
{
    // a common class for manipulating pipeline script files
    // Only this class interacts with physical script files
    class ScriptParser
    {

        private XmlDataDocument m_ScriptDocument;
        private string m_ScriptFilePath;
        private List<ScriptParameter> m_ParameterList;

        public ScriptParser(string ScriptPath)
        {
            m_ParameterList= new List<ScriptParameter>() ;
            m_ScriptFilePath = ScriptPath;
            XmlTextReader reader = new XmlTextReader(m_ScriptFilePath);
            reader.XmlResolver = null;

            m_ScriptDocument = new XmlDataDocument();
            m_ScriptDocument.XmlResolver = null;
            m_ScriptDocument.Load(reader);
            reader.Close();
            
            //populate parameters list
            PopulateParameterList();
                    }
        /// <summary>
        /// <summary
        ///  populate parameter list
        /// </summary>
        private void PopulateParameterList()
        {
    XmlNodeList CompleteNodeList =  m_ScriptDocument.GetElementsByTagName("parameter") ;
            ScriptParameter p  = null ;
            foreach (XmlNode n in CompleteNodeList)
            {
                if (n.Attributes.Count > 0)
                {
                    p = new ScriptParameter(n);
                    m_ParameterList.Add(p);
                }
}

        }


        /// <summary>
        /// summary
        /// List of parameters available in script
        /// </summary>
        public List<ScriptParameter> ParameterList
        {
            get { return m_ParameterList; } 
        }

        /// <summary>
        ///  executes script
                /// </summary>
        public void ExecuteScript  ()
        {
            /*
            if (!File.Exists(m_InputFile.ParameterValue)
                            || !Directory.Exists(m_OutputDirectory.ParameterValue) || Directory.GetFiles(m_OutputDirectory.ParameterValue).Length > 0
                            || (m_BitRate.ParameterValue != "32" && m_BitRate.ParameterValue != "48" && m_BitRate.ParameterValue != "64" && m_BitRate.ParameterValue != "128"))
            {
                throw new System.Exception( Localizer.Message ("Invalid_ScriptParameters")) ;
                return;
            }
             */

            string Param = "";
            foreach (ScriptParameter p in  ParameterList)
            {
                if (p.IsParameterRequired)
                {
                    //Param = Param + " --\"" + p.ParameterName + "=" + p.ParameterValue + "\"";
                    Param = Param + " \"" + p.Name + "=" + p.ParameterValue + "\"";
                }
            }
                        // invoke the script
            string PipelineFilePath = Path.Combine( Directory.GetParent(m_ScriptFilePath).Parent.FullName, "pipeline-lite.bat" );
                        Process PipelineProcess = new Process();
            PipelineProcess.StartInfo.CreateNoWindow = true;
            PipelineProcess.StartInfo.ErrorDialog = true;
            PipelineProcess.StartInfo.UseShellExecute = false;
                        
                        PipelineProcess.StartInfo.FileName = PipelineFilePath;
            PipelineProcess.StartInfo.Arguments =" -x -q -s \"" + m_ScriptFilePath + "\" -p" + Param;
            PipelineProcess.StartInfo.WorkingDirectory = Directory.GetParent(Directory.GetParent(m_ScriptFilePath).FullName).FullName ;
            
                        try
            {
                PipelineProcess.Start();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            PipelineProcess.WaitForExit();
                        System.Windows.Forms.MessageBox.Show("Task completed");            
        }

    }
}
