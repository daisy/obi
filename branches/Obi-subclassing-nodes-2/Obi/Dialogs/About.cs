using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Form for giving info about Obi.
    /// </summary>
    public partial class About : Form
    {
        /// <summary>
        /// Create a new About form.
        /// </summary>
        public About()
        {
            InitializeComponent();
            // setup the link data for following the link
            linkLabel1.Links[0].LinkData = linkLabel1.Text;
        }

        /// <summary>
        /// Follow a link (opens a browser externally.)
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }

    }
}