using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml ;

namespace PipelineInterface.DataTypes
    {
    class StringDataType
        {
        private string m_Val;
        private ScriptParameter m_Parameter;
        private string m_RegularExpression;

        public StringDataType ( ScriptParameter p, XmlNode DataTypeNode )
            {
            m_Parameter = p;

            m_Val = p.ParameterValue;

            XmlNode FirstChild = DataTypeNode.FirstChild;
            if (FirstChild.Attributes.Count > 0)
                m_RegularExpression = FirstChild.Attributes.GetNamedItem ( "regex" ).Value;
            else
                m_RegularExpression = "";
            }

        public string Value
            {
            get { return m_Val; }
            set
                {
                if (value != "" &&
                    ((m_RegularExpression != "" && Regex.IsMatch ( value, m_RegularExpression, RegexOptions.None ))
                    || m_RegularExpression == ""))
                    {
                    m_Parameter.ParameterValue = value;
                    }
                else throw new System.Exception ( Localizer.Message ( "Pipeline_InvalidString" ) );
                }
            }

        public string RegularExpression
            {
            get { return m_RegularExpression; }
            }


        }
    }
