
namespace Obi.Models
{
    public class WhisperLanguageItem
    {
        public string DisplayName { get; set; } = string.Empty;

        public string LanguageCode { get; set; } = string.Empty;

        public override string ToString()
        {
            return DisplayName;
        }
    }
}