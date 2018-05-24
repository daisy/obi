using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PipelineInterface
    {
    public class PipelineInfo
        {
        private Dictionary<string, FileInfo> m_ScriptsInfo = new Dictionary<string, FileInfo> ();
        private Dictionary<string, string> m_TaskScriptNameToNiceNameMap = new Dictionary<string,string> ();

        public PipelineInfo ( string ScriptsDirectory )
            {
            PopulateScriptsDictionary ( ScriptsDirectory );
            
            }

        private void PopulateScriptsDictionary ( string DirPath )
            {
            string[] ScriptsFilePaths = Directory.GetFiles ( DirPath, "*.taskScript", SearchOption.TopDirectoryOnly );
            FileInfo ScriptFileInfo = null;
            foreach (string s in ScriptsFilePaths)
                {
                ScriptFileInfo = new FileInfo ( s );
                ScriptParser parser = new ScriptParser ( ScriptFileInfo.FullName );
                if (!m_ScriptsInfo.ContainsKey ( parser.Name ))
                    {
                    m_ScriptsInfo.Add ( parser.Name, ScriptFileInfo );
                    m_TaskScriptNameToNiceNameMap.Add ( parser.Name, 
                        !string    .IsNullOrEmpty(Localizer.Message(parser.Name))? Localizer.Message(parser.Name):parser.NiceName);
                    
                    }
                else
                    {
                    System.Windows.Forms.MessageBox.Show (string.Format ( Localizer.Message ( "Pipeline_DuplicateScript" ),parser.NiceName) , 
                        Localizer.Message ( "Caption_Warning" ),
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning );
                    }
                }
            }

        public Dictionary<string, FileInfo> ScriptsInfo
            {
            get { return m_ScriptsInfo; }
            }

        public Dictionary<string, string> TaskScriptNameToNiceNameMap
            {
            get
                {
                return m_TaskScriptNameToNiceNameMap;
                }
            }

        }
    }
