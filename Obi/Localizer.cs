using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Application-wide localizer for messages.
    /// </summary>
    class Localizer
    {
        private ResourceManager mResmngr;                              // the resource manager for the messages

        private static readonly Localizer INSTANCE = new Localizer();  // singleton to have a static Message() method

        /// <summary>
        /// Create an new localizer
        /// </summary>
        private Localizer()
        {
            mResmngr = new ResourceManager("Obi.messages", GetType().Assembly);
        }

        /// <summary>
        /// Get a localized string.
        /// </summary>
        /// <param name="key">Key in the resources file.</param>
        /// <returns>The localized string for the given key.</returns>
        public static string Message(string key)
        {
            return INSTANCE.mResmngr.GetString(key);
        }
    }
}
