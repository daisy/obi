using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// The help browser.
    /// </summary>
    public partial class Help : Form
    {
        /// <summary>
        /// Get the web browser object of the dialog (to set the document.)
        /// </summary>
        public WebBrowser WebBrowser
        {
            get { return mWebBrowser; }
        }

        /// <summary>
        /// Create the help dialog.
        /// </summary>
        public Help()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Catch links going outside of the help file so that they are opened in a real browser instead.
        /// </summary>
        private void mWebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // at the moment file links point inside the help file, other links should be handled externally.
            if (e.Url.Scheme != "file")
            {
                System.Diagnostics.Process.Start(e.Url.ToString());
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Set the title of the frame to that of the help page once it's loaded.
        /// </summary>
        private void mWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Text = mWebBrowser.DocumentTitle;
        }
    }
}