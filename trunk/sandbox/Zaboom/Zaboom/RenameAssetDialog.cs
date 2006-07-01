using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class RenameAssetDialog : Form
    {
        public string AssetName
        {
            get
            {
                return mNameBox.Text;
            }
        }

        public RenameAssetDialog(string name)
        {
            InitializeComponent();
            mNameBox.Text = name;
        }
    }
}