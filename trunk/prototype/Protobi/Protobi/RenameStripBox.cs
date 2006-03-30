using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    /// <summary>
    /// Text box to input a new label for a strip.
    /// </summary>
    public partial class RenameStripBox : Form
    {
        public string Label { get { return textBox.Text; } }  // get the label once the user has clicked OK

        /// <summary>
        /// Create the box with an initial value for the text field.
        /// </summary>
        /// <param name="label">The original label of the strip to rename.</param>
        public RenameStripBox(string label)
        {
            InitializeComponent();
            textBox.Text = label;
            textBox.Focus();
        }
    }
}