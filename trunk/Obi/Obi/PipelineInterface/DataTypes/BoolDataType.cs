using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.PipelineInterface.DataTypes
{
    public class BoolDataType
    {
        private bool m_Val ;

        public BoolDataType ( ScriptParameter p)
        {
            
        }

        public bool Value
        {
            get {return m_Val;}
            set { m_Val = value ; }
        }

    }
}
