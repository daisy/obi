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
            mWebBrowser.Url = new System.Uri(System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("about_file_name")));
        }

        // Catch links going outside to open in a different browser
        private void mWebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme != "file")
            {
                System.Diagnostics.Process.Start(e.Url.ToString());
                e.Cancel = true;
            }
        }

        private void mWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            mWebBrowser.Document.GetElementById("info-version").InnerText = System.String.Format("{0} v{1}",
                Application.ProductName, Application.ProductVersion);
            mWebBrowser.Document.GetElementById("real-version").InnerText =
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            mWebBrowser.Document.GetElementById("website").InnerText = Localizer.Message("obi_url");
            mWebBrowser.Document.GetElementById("website").SetAttribute("href", Localizer.Message("obi_url"));
        }
    }
}