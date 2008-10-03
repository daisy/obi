using System;
using System.Collections.Generic;
using System.Text;
using System.Xml ;

namespace Obi.PipelineInterface.DataTypes
    {
    class StringDataType
        {
        private string m_Val;
        private ScriptParameter m_Parameter;

        public StringDataType ( ScriptParameter p, XmlNode DataTypeNode )
            {
            m_Parameter = p;

            m_Val = p.ParameterValue;
            }

        public string Value
            {
            get { return m_Val; }
            set
                {
                m_Parameter.ParameterValue = value;
                }
            }


        }
    }
