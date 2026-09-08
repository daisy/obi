using System.Collections.Generic;

namespace Obi.Models
{
    public static class ParakeetLanguages
    {
        public static readonly List<WhisperLanguageItem> Languages =
        [
            new()
            {
                DisplayName = "English",
                LanguageCode = "en"
            },

            new()
            {
                DisplayName = "French",
                LanguageCode = "fr"
            },

            new()
            {
                DisplayName = "German",
                LanguageCode = "de"
            },

            new()
            {
                DisplayName = "Spanish",
                LanguageCode = "es"
            },

            new()
            {
                DisplayName = "Italian",
                LanguageCode = "it"
            },

            new()
            {
                DisplayName = "Portuguese",
                LanguageCode = "pt"
            },

            new()
            {
                DisplayName = "Dutch",
                LanguageCode = "nl"
            },

            new()
            {
                DisplayName = "Russian",
                LanguageCode = "ru"
            },

            new()
            {
                DisplayName = "Ukrainian",
                LanguageCode = "uk"
            },

            new()
            {
                DisplayName = "Polish",
                LanguageCode = "pl"
            },

            new()
            {
                DisplayName = "Swedish",
                LanguageCode = "sv"
            },

            new()
            {
                DisplayName = "Danish",
                LanguageCode = "da"
            },

            new()
            {
                DisplayName = "Finnish",
                LanguageCode = "fi"
            },

            new()
            {
                DisplayName = "Czech",
                LanguageCode = "cs"
            },

            new()
            {
                DisplayName = "Greek",
                LanguageCode = "el"
            },

            new()
            {
                DisplayName = "Hungarian",
                LanguageCode = "hu"
            },

            new()
            {
                DisplayName = "Romanian",
                LanguageCode = "ro"
            },

            new()
            {
                DisplayName = "Croatian",
                LanguageCode = "hr"
            },

            new()
            {
                DisplayName = "Slovak",
                LanguageCode = "sk"
            },

            new()
            {
                DisplayName = "Slovenian",
                LanguageCode = "sl"
            },

            new()
            {
                DisplayName = "Bulgarian",
                LanguageCode = "bg"
            },

            new()
            {
                DisplayName = "Estonian",
                LanguageCode = "et"
            },

            new()
            {
                DisplayName = "Latvian",
                LanguageCode = "lv"
            },

            new()
            {
                DisplayName = "Lithuanian",
                LanguageCode = "lt"
            },

            new()
            {
                DisplayName = "Maltese",
                LanguageCode = "mt"
            }
        ];

        public static readonly HashSet<string> SupportedCodes =
            new(
                Languages.ConvertAll(
                    language =>
                        language.LanguageCode));
    }
}