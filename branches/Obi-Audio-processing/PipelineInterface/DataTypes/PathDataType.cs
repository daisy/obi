using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace PipelineInterface.DataTypes
{
        class PathDataType
    {
                private string m_Path;
            private ScriptParameter m_Parameter;

        public enum InputOrOutput { input, output } ;
        private InputOrOutput m_InputOrOutput;

        public enum FileOrDirectory { File, Directory }
        private FileOrDirectory m_FileOrDirectory;

            public PathDataType( ScriptParameter p, XmlNode DataTypeNode)
        {
            m_Parameter = p;
            m_Path = p.ParameterValue ;
            XmlNode ChildNode = DataTypeNode.FirstChild;
            m_FileOrDirectory = ChildNode.Name == "file" ? FileOrDirectory.File : FileOrDirectory.Directory;
            m_InputOrOutput = ChildNode.Attributes.GetNamedItem("type").Value == "input" ? InputOrOutput.input : InputOrOutput.output;
                    }

            /// <summary>
            ///  The path is of a file or a directory
                        /// </summary>
            public FileOrDirectory IsFileOrDirectory { get { return m_FileOrDirectory; } }

            /// <summary>
            /// Is it a path for input file/directory or for output file/directory
                        /// </summary>
            public InputOrOutput isInputOrOutput { get { return  m_InputOrOutput ; } } 

        public string  Value
                {
            get { return m_Path; }
            set
            {
                if (Exists (value) )
                {
                m_Path = value;
            UpdateScript(m_Path);
                }
                else throw new System.Exception (Localizer.Message("No_Path")) ;
                    }
}
            /// <summary>
            ///  Does the path exists
                        /// </summary>
            /// <param name="PathValue"></param>
            /// <returns></returns>
            public bool Exists(string PathValue)
            {
                                    if (m_FileOrDirectory == FileOrDirectory.File)
                    {
                        if (File.Exists(PathValue))
                            return true ;
                                            }
                    else if (m_FileOrDirectory == FileOrDirectory.Directory)
                    {
                        if (Directory.Exists(PathValue))
                                                    return true ;
                    }
                    return false;
            }

            private bool UpdateScript(string Val)
            {
                if (Val != null)
                {
                                        m_Parameter.ParameterValue = Val;
                    return true;
                }
                else
                    return false;
            }

            public bool Create(string pathValue)
            {
                if (m_InputOrOutput == InputOrOutput.output)
                {
                    if (m_FileOrDirectory == FileOrDirectory.File)
                    {
                        File.CreateText(pathValue);
                        return true;
                    }
                    else if (m_FileOrDirectory == FileOrDirectory.Directory)
                    {
                        Directory.CreateDirectory(pathValue);
                        return true;
                    }
                    return false;
                }
                else
                {
                    throw new System.Exception(Localizer.Message("CannotCreate_InputPath"));
                                    }
                            }

    }
}
