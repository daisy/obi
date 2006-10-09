using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NCXEdit.NCX;

namespace NCXEdit
{
    public partial class TitleAuthorDialog : Form
    {
        public TitleAuthorDialog()
        {
            InitializeComponent();
        }

        /*
        public Title GetTitle()
        {
            Text text = new Text(titleText.Text);
            Audio audio = titleAudio.Text == "" ? null : new Audio(new Uri(titleAudio.Text));
        }
        */

        [System.Runtime.InteropServices.DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

        [System.Flags]
        public enum PlaySoundFlags : int
        {
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }

        private void titleChooseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files (*.wav)|*.wav";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                titleAudio.Text = dialog.FileName;
            }
        }

        private void titlePlayButton_Click(object sender, EventArgs e)
        {
            if (titleAudio.Text != null)
            {
                PlaySound(titleAudio.Text, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
            }
        }
    }
}