using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PipelineInterface.DataTypes
    {
    class IntDataType
        {
                private int m_Val;
        private ScriptParameter m_Parameter;
        private int m_Max;
        private int m_Min;


        public IntDataType ( ScriptParameter p, XmlNode DataTypeNode )
            {
            m_Parameter = p;

            XmlNode FirstChild = DataTypeNode.FirstChild;
            if (FirstChild.Attributes.Count > 0)
                {
                string min = FirstChild.Attributes.GetNamedItem ( "min" ).Value;
                string max = FirstChild.Attributes.GetNamedItem ( "max" ).Value;

                m_Min = int.Parse ( min );
                m_Max = int.Parse ( max );
                }
            else
                {
                m_Max = 231;
                m_Min = -231;
                }
                        }

        public int Max { get { return m_Max; } }

        public int Min { get { return m_Min; } }


        public int Value
            {
            get { return m_Val; }
            set
                {
                if (value >= m_Min && value <= m_Max)
                    {
                    m_Val = value;
                    m_Parameter.ParameterValue = m_Val.ToString ();
                    }
                else
                    throw new System.Exception ( Localizer.Message ( "Pipeline_OutOfRange" ) );
                }
            }

        }
    }
