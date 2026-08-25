using System.Collections.Generic;

namespace Obi.Models
{
    public static class ParakeetLanguages
    {
        public static readonly HashSet<string> SupportedCodes =
            new HashSet<string>(
                new[]
                {
                    "bg", // Bulgarian
                    "hr", // Croatian
                    "cs", // Czech
                    "da", // Danish
                    "nl", // Dutch
                    "en", // English
                    "et", // Estonian
                    "fi", // Finnish
                    "fr", // French
                    "de", // German
                    "el", // Greek
                    "hu", // Hungarian
                    "it", // Italian
                    "lv", // Latvian
                    "lt", // Lithuanian
                    "mt", // Maltese
                    "pl", // Polish
                    "pt", // Portuguese
                    "ro", // Romanian
                    "sk", // Slovak
                    "sl", // Slovenian
                    "es", // Spanish
                    "sv", // Swedish
                    "ru", // Russian
                    "uk"  // Ukrainian
                });
    }
}