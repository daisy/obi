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
                throw new System.Exception("One or more parameters are invalid");
                return;
            }
             */ 
            
            string Param = "";
            foreach (ScriptParameter p in  ParameterList)
            {
                if (p.IsParameterRequired)
                {
                    //Param = Param + " --\"" + p.ParameterName + "=" + p.ParameterValue + "\"";
                    Param = Param + " \"" + p.ParameterName + "=" + p.ParameterValue + "\"";
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
            PipelineProcess.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "\\PipelineLight";
            
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

    class ScriptParameter
    {
        private string m_Name;
        private string m_Value;
        private bool m_Required;
        private string m_DiscriptiveName;

        public ScriptParameter(XmlNode node)
        {
            XmlNodeList ChildList = node.ChildNodes;
            foreach (XmlNode x in ChildList)
            {
                if (x.Name == "nicename" && x.ParentNode == node)
                    m_DiscriptiveName = x.InnerText;
            }

            for (int AttrIndex = 0; AttrIndex < node.Attributes.Count; AttrIndex++)
            {
                if (node.Attributes[AttrIndex].Name == "name")
                {
                    m_Name = node.Attributes.GetNamedItem("name").Value;
                }
                else if (node.Attributes[AttrIndex].Name == "value")
                {
                    m_Value = node.Attributes.GetNamedItem("value").Value;
                }
                else if (node.Attributes[AttrIndex].Name == "required")
                {
                    if (node.Attributes.GetNamedItem("required").Value == "true")
                        m_Required = true;
                    else
                        m_Required = false;

                }
            }
        }

        public string ParameterName { get { return m_Name; } }
        public string ParameterDescriptiveName { get { return m_DiscriptiveName; } }
        public bool IsParameterRequired { get { return m_Required; } }

        public string ParameterValue
        {
            get { return m_Value; }
            set
            {
                if (value != null && value != "")
                {
                    m_Value = value;
                    m_Required = true;
                }
                else if (m_Required && (m_Value == null || m_Value == ""))
                    m_Required = false;
            }
        }


    }
    

}
