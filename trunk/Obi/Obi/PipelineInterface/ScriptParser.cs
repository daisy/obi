using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace PipelineInterface
{
    // a common class for manipulating pipeline script files
    // Only this class interacts with physical script files
    class ScriptParser
    {

        private XmlDataDocument m_ScriptDocument;
        private string m_ScriptFilePath;
        private XmlNodeList m_ParametersList;

        public ScriptParser(string ScriptPath)
        {
            m_ScriptFilePath = ScriptPath;
            XmlTextReader reader = new XmlTextReader(m_ScriptFilePath);
            reader.XmlResolver = null;

            m_ScriptDocument = new XmlDataDocument();
            m_ScriptDocument.XmlResolver = null;
            m_ScriptDocument.Load(reader);
            reader.Close();
            m_ParametersList = m_ScriptDocument.GetElementsByTagName("parameter");

        }

        /// <summary>
        /// summary
        /// returns parameter value for supplied parameter name
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetParameterValue(string nodeName)
        {
            foreach (XmlNode n in m_ParametersList)
            {
                for (int AttrIndex = 0; AttrIndex < n.Attributes.Count; AttrIndex++)
                {
                    if (n.Attributes[AttrIndex].Value == nodeName)
                        return n.Attributes.GetNamedItem("value").Value;
                }               
            }
            return "";
        }

        /// <summary>
        ///  Sets parameter value for supplied parameter name
                /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="val"></param>
        public void SetParameterValue(string nodeName, string val)
        {
                                    foreach (XmlNode n in m_ParametersList)
            {
                for (int AttrIndex = 0; AttrIndex < n.Attributes.Count; AttrIndex++)
                                        {
                                            if (n.Attributes[AttrIndex].Value == nodeName)
                {
                    SetParameterValue(n, val);
                    break;
                }
                                        } // end attribute loop
            }// end foreach loop

        }

        public void SetParameterValue(XmlNode node, string val)
        {
            node.Attributes.GetNamedItem("value").Value = val.Trim();
            node.Attributes.GetNamedItem("required").Value = "false";

            //System.Windows.Forms.MessageBox.Show(node.Attributes.GetNamedItem("value").Value);
                    }


        /// <summary>
        ///  commits changes to script file
                /// </summary>
        public void CommitScriptChanges()
        {
            // write to script file
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(m_ScriptFilePath, null);
                writer.Formatting = Formatting.Indented;
                m_ScriptDocument.Save(writer);
                writer.Close();
                writer = null;
                m_ScriptDocument = null;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
             
        }

    }
}
