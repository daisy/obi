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
    public partial class OutputDeviceDialog : Form
    {
        public int Selected
        {
            get
            {
                return mDeviceBox.SelectedIndex;
            }
        }

        public OutputDeviceDialog(ArrayList devices, int selected)
        {
            InitializeComponent();
            foreach (object device in devices)
            {
                mDeviceBox.Items.Add(device);
            }
            mDeviceBox.SelectedIndex = selected;
        }
    }
}