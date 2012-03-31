using System;
using System.Collections ;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace PipelineInterface.DataTypes
{
    public class EnumDataType
    {
        private List<string>  m_ValueList ;
        private List<string> m_NiceNameList;
        private int m_SelectedIndex;
        public ScriptParameter m_Parameter ;

        public EnumDataType (ScriptParameter p ,  XmlNode node )
        {
            m_Parameter = p;
            m_ValueList = new List<string>();
            m_NiceNameList = new List<string>();
            m_SelectedIndex = -1;
            PopulateListFromNode(node);

            // select list node as default value if default  exists
            if (p.ParameterValue != null && p.ParameterValue !="" 
                && m_ValueList.Contains(p.ParameterValue ))
                m_SelectedIndex = m_ValueList.BinarySearch(p.ParameterValue) ;
        }

        private void PopulateListFromNode(XmlNode DatatypeNode )
        {
            XmlNode EnumNode = DatatypeNode.FirstChild;
                        XmlNodeList nodeList = EnumNode.SelectNodes("item");
            
            foreach (XmlNode n in nodeList)
            {
                                                                                                                                                                    m_ValueList.Add(n.Attributes.GetNamedItem("value").Value);
                m_NiceNameList.Add (n.Attributes.GetNamedItem("nicename").Value );
                            }
        }


        public List<string> GetValues { get { return m_ValueList; } }
        public List<string> GetNiceNames { get { return m_NiceNameList; } }

        /// <summary>
        ///  Gets and sets the index of value selected.
                /// </summary>
        public int SelectedIndex
        {
            get { return m_SelectedIndex;  }
            set 
            {
                if (value >= 0 && value < m_ValueList.Count )
                                    SetSelectedIndexAndUpdateScript( value  ) ;
                                    else throw new System.Exception (Localizer.Message("IndexNotInRange")) ;
            }
        }


        public string SelectedItemValue
        {
            get { return m_ValueList[m_SelectedIndex]; }
            set
            {
                if (value != null && m_ValueList.Contains(value))
                    SetSelectedIndexAndUpdateScript( m_ValueList.BinarySearch(value) );
                else throw new System.Exception(Localizer.Message("NotAbleToSelectItem"));
            }
        }


        public string SelectedItemNiceName
        {
            get { return m_NiceNameList[m_SelectedIndex]; }
            set
            {
                if (value != null && m_NiceNameList.Contains(value))
                    SetSelectedIndexAndUpdateScript( m_NiceNameList.BinarySearch(value) ) ;
                else throw new System.Exception(Localizer.Message("NotAbleToSelectItem"));
            }
        }


        private bool SetSelectedIndexAndUpdateScript ( int Index)
        {
            if (Index > 0 && Index < m_ValueList.Count)
            {
                m_SelectedIndex = Index;
                                m_Parameter.ParameterValue = SelectedItemValue;
                return true;
            }
            else
                return false;
                    }
        



    }
}
