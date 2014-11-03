using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Form for giving info about Obi.
    /// </summary>
    public partial class About : Form
    {
        private Settings mSettings;
        private bool m_flagFontChange = false;
        /// <summary>
        /// Create a new About form.
        /// </summary>
        public About(Settings settings)
        {
            InitializeComponent();
            mSettings = settings;
            mWebBrowser.Url = new System.Uri(System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("about_file_name")));
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new System.Drawing.Font(settings.ObiFont, this.Font.Size, System.Drawing.FontStyle.Regular);//@fontconfig  
                m_flagFontChange = true;
            }
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
            if (m_flagFontChange)
            {
                mWebBrowser.Document.ExecCommand("SelectAll", false, "null");
                mWebBrowser.Document.ExecCommand("FontName", false, mSettings.ObiFont);
                mWebBrowser.Document.ExecCommand("Unselect", false, "null");
            }
            mWebBrowser.Document.GetElementById("info-version").InnerText = System.String.Format("{0} v{1}",
                Application.ProductName, Application.ProductVersion);
            mWebBrowser.Document.GetElementById("real-version").InnerText =
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            mWebBrowser.Document.GetElementById("website").InnerText = Localizer.Message("obi_url");
            mWebBrowser.Document.GetElementById("website").SetAttribute("href", Localizer.Message("obi_url"));
        }

        private void mbtnClose_Click ( object sender, System.EventArgs e )
            {
            Close ();
            }

        private void About_Load ( object sender, System.EventArgs e )
            {

            }
    }
}