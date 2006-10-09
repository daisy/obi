using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Protobi
{
    class Localizer
    {
        private static readonly Localizer instance = new Localizer();
        private ResourceManager resmngr;

        public static Localizer Instance { get { return instance; } }

        private Localizer()
        {
            resmngr = new ResourceManager("Protobi.messages", GetType().Assembly);
        }

        /// <summary>
        /// Get a localized string.
        /// </summary>
        /// <param name="key">Key in the resources file.</param>
        /// <returns>The localized string for the given key.</returns>
        public static string GetString(string key)
        {
            return instance.resmngr.GetString(key);
        }
    }
}
