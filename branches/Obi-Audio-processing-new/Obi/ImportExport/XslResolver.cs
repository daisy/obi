using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Obi.ImportExport
{
    /// <summary>
    /// This class helps resolve references inside the assembly
    /// We need it because the XSLT files are embedded resources and they reference each other.
    /// The logic for this code was copied from http://msmvps.com/blogs/jayharlow/archive/2005/01/24/33766.aspx
    /// </summary>
    public class XslResolver : System.Xml.XmlResolver
    {
        private Type mType;

        public XslResolver(Type type)
        {
            mType = type;
        }

        public override System.Net.ICredentials Credentials
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (!absoluteUri.IsFile) throw new Exception("Not a file URI");
            if (File.Exists(absoluteUri.LocalPath))
            {
                return new FileStream(absoluteUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                string name = Path.GetFileName(absoluteUri.LocalPath);
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
                Stream stream = null;
                //Try the specific culture
                stream = GetManifestResourceStream(culture, name);
                //Try the neutral culture
                if (stream == null && !culture.IsNeutralCulture) stream = GetManifestResourceStream(culture.Parent, name);
                //Try the default culture
                if (stream == null) stream = mType.Assembly.GetManifestResourceStream("XukToZed." + name);
                if (stream == null) throw new FileNotFoundException("File not found: " + name);
                return stream;
            }
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri == null) baseUri = new Uri(mType.Assembly.Location);
            return base.ResolveUri(baseUri, relativeUri);
        }

        private Stream GetManifestResourceStream(System.Globalization.CultureInfo culture, string name)
        {
            try
            {
                System.Reflection.Assembly satellite = mType.Assembly.GetSatelliteAssembly(culture);
                return satellite.GetManifestResourceStream(mType, name);
            }
            catch
            {
                return null;
            }

        }
    }
}
