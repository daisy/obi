using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PipelineInterface
{
    public class ScriptParameter
    {
        

            private string m_Name;
            private string m_Value;
        private string m_NiceName;
            private bool m_Required;
            private string m_Discription;
        private object m_DataType ;
        
            public ScriptParameter(XmlNode node)
            {
                GetParameterAttributeInfo(node);
                GetParameterProperties(node);
            }


        /// <summary>
        /// extracts  parameter attribute info from script xml file
                /// </summary>
        /// <param name="node"></param>
        private void GetParameterAttributeInfo(XmlNode node)
        {
                // Get attribute  information of ScriptParameter
                for (int AttrIndex = 0; AttrIndex < node.Attributes.Count; AttrIndex++)
                {
                    switch (node.Attributes[AttrIndex].Name)
                    {
                        case "name":
                            m_Name = node.Attributes.GetNamedItem("name").Value;
                            break;

                        case "value":
                            m_Value = node.Attributes.GetNamedItem("value").Value;
                            break;

                        case "required":
                            m_Required=  node.Attributes.GetNamedItem("required").Value == "true" ? true : false;
                            break;
                    }
                }
        }
            
/// <summary>
///  extracts parameter properties from script xml file
/// </summary>
/// <param name="node"></param>
        private void GetParameterProperties( XmlNode node )
        {
                    // Get properties of parameter from its child nodes
                    XmlNode ChildNode = node.FirstChild;

                    while (ChildNode != null)
                    {
                        switch (ChildNode.Name)
                        {
                            case "nicename" :
                                m_NiceName = ChildNode.InnerText;
                                                                break;

                            case "description" :
                                m_Discription = ChildNode.InnerText;
                                                                break; 

                            case "datatype" :
                                GetDatatype(ChildNode);
                                break;
                        }
                                
                        ChildNode = ChildNode = ChildNode.NextSibling;
                                            }
                                       
                }


        /// <summary>
        /// choose  datatype matching  parameter datatype in script and set its properties from script xml file
                        /// </summary>
        /// <param name="DatatypeNode"></param>
        private void GetDatatype(XmlNode DatatypeNode)
        {
            switch (DatatypeNode.FirstChild.Name)
            {
                case "file":
                    m_DataType = new DataTypes.PathDataType( this , DatatypeNode);
                                        break;

                                    case "directory":
                                        m_DataType = new DataTypes.PathDataType(this , DatatypeNode);
                                        break;

                case "enum":
                    m_DataType = new  DataTypes.EnumDataType( this , DatatypeNode);
                    break;

                case "boolean":
                    m_DataType = new DataTypes.BoolDataType(this, DatatypeNode);
                    break;

                    case "integer":
                    m_DataType = new DataTypes.IntDataType ( this, DatatypeNode );
                    break;

                    case "string":
                    m_DataType = new DataTypes.StringDataType( this, DatatypeNode );
                    break;    
            }
        }


            public string Name { get { return m_Name; } }
        public string NiceName { get { return m_NiceName; } }
            public string Description { get { return m_Discription; } }
            public bool IsParameterRequired { get { return m_Required; } }
        public object ParameterDataType { get { return m_DataType ; }  }

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

