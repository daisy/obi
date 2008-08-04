using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Obi.PipelineInterface.DataTypes
{
        class PathDataType
    {
                private string m_Path;
            private ScriptParameter m_Parameter;

        public enum InputOrOutput { input, output } ;
        public InputOrOutput m_InputOrOutput;

        public enum FileOrDirectory { File, Directory }
        public FileOrDirectory m_FileOrDirectory;

            public PathDataType( ScriptParameter p, XmlNode DataTypeNode)
        {
            m_Parameter = p;
            m_Path = p.ParameterValue ;
            XmlNode ChildNode = DataTypeNode.FirstChild;
            m_FileOrDirectory = ChildNode.Name == "file" ? FileOrDirectory.File : FileOrDirectory.Directory;
            m_InputOrOutput = ChildNode.Attributes.GetNamedItem("type").Value == "input" ? InputOrOutput.input : InputOrOutput.output;
                    }

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
                else throw new System.Exception ("Path do not exists") ;
                    }
}

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
                    throw new System.Exception("cannot create path or type input");
                                    }
                            }

    }
}
