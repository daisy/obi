using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PipelineInterface.ParameterControls
{
    public partial class BaseUserControl : UserControl
    {
        public BaseUserControl() { InitializeComponent(); }

        public virtual String DescriptionLabel
        {
            get { return mLabel.Text; }
            set { mLabel.Text = value; }
        }

        public virtual void UpdateScriptParameterValue() { }

        /// <summary>
        ///Accepts nice name or description and returns localized string if available else returns parameter value
        /// </summary>
        /// <param name="niceName"></param>
        /// <returns></returns>
        protected string GetLocalizedString(string niceName)
        {
            string key = niceName.Replace(" ", "_");
            if (key.EndsWith(".")) key = key.Substring(0, key.Length - 1);
            string val = Localizer.Message(key);
            Console.WriteLine("Key=" + key + "        Value=" + niceName);
            return string.IsNullOrEmpty(val) ? niceName : val;
        }

    }
}
