using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;
using System.Web.Services.Description;

namespace Obi.Dialogs
{
    public partial class AddAzureVoice : Form
    {
        private SpeechSynthesizer? m_AzureSpeechSynthesizer;
        private readonly SpeechConfig m_AzureSpeechConfig;
        private GenerateSpeech m_GenerateSpeech;
        private SynthesisVoicesResult resultasync;
        private Dictionary<string, string> m_languageCodesDictionary;
        private List<string> m_DialectList;

        public AddAzureVoice(SpeechSynthesizer speechSynthesizer, SpeechConfig speechConfig, GenerateSpeech generateSpeech)
        {
            InitializeComponent();
            m_AzureSpeechSynthesizer = speechSynthesizer;
            m_AzureSpeechConfig = speechConfig;
            m_GenerateSpeech = generateSpeech;
            m_GenderCb.SelectedIndex= 0;    

            LoadAzureVoices();
        }

        private void m_AddVoiceBtn_Click(object sender, EventArgs e)
        {
            if (m_AddAzureVoiceListBox.SelectedItem == null)
            {
                MessageBox.Show("Kindly select voice to add", Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string voice = m_AddAzureVoiceListBox.SelectedItem.ToString();
            m_GenerateSpeech.AddAzureVoices(voice);
            this.Close();

        }

        private void UpdateAzureVoices()
        {
            m_AddAzureVoiceListBox.Items.Clear();
            if (resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                foreach (var tempVoice in resultasync.Voices)
                {
                    if (m_GenderCb.SelectedItem.ToString() == "All" || (m_GenderCb.SelectedItem.ToString() == "Male" && SynthesisVoiceGender.Male == tempVoice.Gender)
                         || (m_GenderCb.SelectedItem.ToString() == "Female" && SynthesisVoiceGender.Female == tempVoice.Gender))
                    {
                        if (m_LanguageCb.SelectedItem != null && m_DialectCb.SelectedItem != null && m_VoiceCb.SelectedItem != null)
                        {
                            var languageCode = m_languageCodesDictionary.FirstOrDefault(x => x.Value == m_LanguageCb.SelectedItem.ToString()).Key;
                            var dialectCode = m_languageCodesDictionary.FirstOrDefault(x => x.Value == m_DialectCb.SelectedItem.ToString()).Key;
                            string extractLanguageCode = tempVoice.Locale.Split('-')[0];    
                            if (m_LanguageCb.SelectedItem.ToString() == "All" || extractLanguageCode.Equals(languageCode))
                                if (m_DialectCb.SelectedItem.ToString() == "All" || tempVoice.Locale.Equals(dialectCode))
                                    if (m_VoiceCb.SelectedItem.ToString() == "All" || tempVoice.LocalName == m_VoiceCb.SelectedItem.ToString())
                                        m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName + "  " + tempVoice.LocalName);
                        }
                    }
                }
                 
            }

        }

        private void updateDialectCb()
        {
            m_DialectCb.Items.Clear();
            m_DialectCb.Items.Add("All");
            if (m_DialectList != null)
                foreach (var dialect in m_DialectList)
                {
                    if (!m_DialectCb.Items.Contains(dialect) && (m_LanguageCb.SelectedItem.ToString() == "All" ||  dialect.Contains(m_LanguageCb.SelectedItem.ToString())))
                    {
                        m_DialectCb.Items.Add(dialect.ToString());
                    }
                }
            m_DialectCb.SelectedIndex = 0;

        }
        private void updateVoiceCb()
        {
            m_VoiceCb.Items.Clear();
            m_VoiceCb.Items.Add("All");

            var languageCode = m_languageCodesDictionary.FirstOrDefault(x => x.Value == m_LanguageCb.SelectedItem.ToString()).Key;
            var dialectCode = m_languageCodesDictionary.FirstOrDefault(x => x.Value == m_DialectCb.SelectedItem.ToString()).Key;
            foreach (var voice in resultasync.Voices)
            {
                if ((m_LanguageCb.SelectedIndex == 0 && m_DialectCb.SelectedIndex == 0) || (m_DialectCb.SelectedIndex != 0 && voice.Locale == dialectCode)
                    || (m_LanguageCb.SelectedIndex != 0 && m_DialectCb.SelectedIndex == 0 && voice.Locale.Contains(languageCode)))
                {
                    if (m_GenderCb.SelectedIndex == 0 || m_GenderCb.SelectedItem.ToString() == voice.Gender.ToString())
                        m_VoiceCb.Items.Add(voice.LocalName);
                }
            }
            //if (m_DialectList != null)
            //    foreach (var dialect in m_DialectList)
            //    {
            //        if (!m_DialectCb.Items.Contains(dialect) && (m_LanguageCb.SelectedItem.ToString() == "All" ||  dialect.Contains(m_LanguageCb.SelectedItem.ToString())))
            //        {
            //            m_DialectCb.Items.Add(dialect.ToString());
            //        }
            //    }
            m_VoiceCb.SelectedIndex = 0;

        }

        private async void LoadAzureVoices()
        {
            if (m_AzureSpeechSynthesizer == null)
            {
                m_AzureSpeechSynthesizer = new(m_AzureSpeechConfig);
            }
            m_languageCodesDictionary = new Dictionary<string, string>
            {
                {"af","Afrikaans" },
                {"af-ZA","Afrikaans (South Africa)" },

                {"am","Amharic" },
                {"am-ET", "Amharic (Ethiopia)" },

                {"ar","Arabic" },
                {"ar-AE","Arabic (U.A.E.)" },
                {"ar-BH","Arabic (Bahrain)" },
                {"ar-DZ","Arabic (Algeria)" },
                {"ar-EG","Arabic (Egypt)" },
                {"ar-IQ","Arabic (Iraq)" },
                {"ar-JO","Arabic (Jordan)" },
                {"ar-KW","Arabic (Kuwait)" },
                {"ar-LB","Arabic (Lebanon)" },
                {"ar-LY","Arabic (Libya)" },
                {"ar-MA","Arabic (Morocco)" },
                {"ar-OM","Arabic (Oman)" },
                {"ar-QA","Arabic (Qatar)" },
                {"ar-SA","Arabic (Saudi Arabia)" },
                {"ar-SY","Arabic (Syria)" },
                {"ar-TN","Arabic (Tunisia)" },
                {"ar-YE","Arabic (Yemen)" },

                {"as","Assamese" },
                {"as-IN","Assamese (India)" },

                {"az","Azeri" },
                {"az-AZ","Azeri (Azerbaijan)" },

                {"bg","Bulgarian" },
                {"bg-BG","Bulgarian (Bulgaria)" },

                {"bn","Bangla" },
                {"bn-BD","Bangla (Bangladesh)" },
                {"bn-IN","Bangla (India)" },

                {"bs","Bosnian" },
                {"bs-BA","Bosnian (Bosnia and Herzegovina)" },

                {"ca","Catalan" },
                {"ca-ES","Catalan (Spain)" },

                {"cs","Czech" },
                {"cs-CZ","Czech (Czech Republic)" },

                {"cy","Welsh" },
                {"cy-GB","Welsh (United Kingdom)" },

                {"da","Danish" },
                {"da-DK","Danish (Denmark)" },

                {"de","German" },
                {"de-AT","German (Austria)" },
                {"de-CH","German (Switzerland)" },
                {"de-DE","German (Germany)" },

                {"el","Greek" },
                {"el-GR","Greek (Greece)" },

                { "en", "English" },
                { "en-AU", "English (Australia)" },
                { "en-CA", "English (Canada)" },
                { "en-GB", "English (United Kingdom)" },
                { "en-HK", "English (Hong Kong SAR)" },
                { "en-IE", "English (Ireland)" },
                { "en-IN", "English (India)" },
                { "en-KE", "English (Kenya)" },
                { "en-NG", "English (Nigeria)" },
                { "en-NZ", "English (New Zealand)" },
                { "en-PH", "English (Republic of the Philippines)" },
                { "en-SG", "English (Singapore)" },
                { "en-TZ", "English (Tanzania)" },
                { "en-US", "English (United States)" },
                { "en-ZA", "English (South Africa)" },

                { "es", "Spanish" },
                { "es-AR", "Spanish (Argentina)" },
                { "es-BO", "Spanish (Bolivia)" },
                { "es-CL", "Spanish (Chile)" },
                { "es-CO", "Spanish (Colombia)" },
                { "es-CR", "Spanish (Costa Rica)" },
                { "es-CU", "Spanish (Cuba)" },
                { "es-DO", "Spanish (Dominican Republic)" },
                { "es-EC", "Spanish (Ecuador)" },
                { "es-ES", "Spanish (Spain)" },
                { "es-GQ", "Spanish (Equatorial Guinea)" },
                { "es-GT", "Spanish (Guatemala)" },
                { "es-HN", "Spanish (Honduras)" },
                { "es-MX", "Spanish (Mexico)" },
                { "es-NI", "Spanish (Nicaragua)" },
                { "es-PA", "Spanish (Panama)" },
                { "es-PE", "Spanish (Peru)" },
                { "es-PR", "Spanish (Puerto Rico)" },
                { "es-PY", "Spanish (Paraguay)" },
                { "es-SV", "Spanish (El Salvador)" },
                { "es-US", "Spanish (United States)" },
                { "es-UY", "Spanish (Uruguay)" },
                { "es-VE", "Spanish (Venezuela)" },

                { "et", "Estonian" },
                { "et-EE", "Estonian (Estonia)" },

                { "eu", "Basque" },
                { "eu-ES", "Basque (Spain)" },

                { "fa", "Farsi" },
                { "fa-IR", "Farsi (Iran)" },

                { "fi", "Finnish" },
                { "fi-FI", "Finnish (Finland)" },

                { "fil", "Filipino" },
                { "fil-ph", "Filipino (Philippines)" },

                { "fr", "French" },
                { "fr-BE", "French (Belgium)" },
                { "fr-CA", "French (Canada)" },
                { "fr-CH", "French (Switzerland)" },
                { "fr-FR", "French (France)" },

                { "ga", "Irish" },
                { "ga-IE", "Irish (Ireland)" },

                { "gl", "Galician" },
                { "gl-ES", "Galician (Spain)" },

                { "gu", "Gujarati" },
                { "gu-IN", "Gujarati (India)" },

                { "he", "Hebrew" },
                { "he-IL", "Hebrew (Israel)" },

                { "hi", "Hindi" },
                { "hi-IN", "Hindi (India)" },

                { "hr", "Croatian" },
                { "hr-HR", "Croatian (Croatia)" },

                { "hu", "Hungarian" },
                { "hu-HU", "Hungarian (Hungary)" },

                { "hy", "Armenian" },
                { "hy-AM", "Armenian (Armenia)" },

                { "id", "Indonesian" },
                { "id-ID", "Indonesian (Indonesia)" },

                { "is", "Icelandic" },
                { "is-IS", "Icelandic (Iceland)" },

                { "it", "Italian" },
                { "it-IT", "Italian (Italy)" },

                { "iu", "Inuktitut" },
                { "iu-CANS-CA", "Inuktitut (Syllabics Canada)" },
                { "iu-LATN-CA", "Inuktitut (Latin  Canada)" },

                { "ja", "Japanese" },
                { "ja-JP", "Japanese (Japan)" },
                
                { "jv", "Javanese" },
                { "jv-ID", "Javanese (Indonesia)" },

                { "ka", "Georgian" },
                { "ka-GE", "Georgian (Georgia)" },

                { "kk", "Kazakh" },
                { "kk-KZ", "Kazakh (Kazakhstan)" },

                { "km", "Cambodian" },
                { "km-KH", "Cambodian (Cambodia)" },

                { "kn", "Kannada" },
                { "kn-IN", "Kannada (India)" },

                { "ko", "Korean" },
                { "ko-KR", "Korean (Korea)" },

                { "lo", "Laothian" },
                { "lo-LA", "Lao (Lao PDR)" },

                { "lt", "Lithuanian" },
                { "lt-LT", "Lithuanian (Lithuania)" },

                { "lv", "Latvian" },
                { "lv-LV", "Latvian (Latvia)" },

                { "mk", "Macedonian" },
                { "mk-MK", "Macedonian" },

                { "ml", "Malayalam" },
                { "ml-IN", "Malayalam (India)" },

                { "mn", "Mongolian" },
                { "mn-MN", "Mongolian (Mongolia)" },

                { "mr", "Marathi" },
                { "mr-IN", "Marathi (India)" },

                { "ms", "Malay" },
                { "ms-MY", "Malay (Malaysia)" },

                { "mt", "Maltese" },
                { "mt-MT", "Maltese (Malta)" },

                { "my", "Burmese" },
                { "my-MM", "Burmese (Myanmar)" },

                { "nb", "Norwegian" },
                { "nb-NO", "Norwegian Bokm†l (Norway)" },

                { "ne", "Nepali" },
                { "ne-NP", "Nepali (Nepal)" },

                { "nl", "Dutch" },
                { "nl-BE", "Dutch (Belgium)" },
                { "nl-NL", "Dutch (Netherlands)" },

                { "or", "Oriya" },
                { "or-IN", "Odia (India)" },

                { "pa", "Punjabi" },
                { "pa-IN", "Punjabi (India)" },

                { "pl", "Polish" },
                { "pl-PL", "Polish (Poland)" },

                { "ps", "Pashto" },
                { "ps-AF", "Pashto (Afghanistan)" },

                { "pt", "Portuguese" },
                { "pt-BR", "Portuguese (Brazil)" },
                { "pt-PT", "Portuguese (Portugal)" },

                { "ro", "Romanian" },
                { "ro-RO", "Romanian (Romania)" },
                { "ru", "Russian" },
                { "ru-RU", "Russian (Russia)" },

                { "si", "Singhalese" },
                { "si-LK", "Sinhala (Sri Lanka)" },

                { "sl", "Slovenian" },
                { "sl-SI", "Slovenian (Slovenia)" },

                { "so", "Somali" },
                { "so-SO", "Somali (Somalia)" },

                { "sq", "Albanian" },
                { "sq-AL", "Albanian (Albania)" },

                { "sr", "Serbian" },
                { "sr-Latn-RS", "Serbian (Latin Serbia)" },
                { "sr-RS", "Serbian (Serbia)" },

                { "su", "Sudanese" },
                { "su-ID", "Sundanese (Indonesia)" },

                { "sv", "Swedish" },
                { "sv-SE", "Swedish (Sweden)" },

                { "sw", "Swahili" },
                { "sw-KE", "Swahili (Kenya)" },
                { "sw-TZ", "Swahili (Tanzania)" },

                { "ta", "Tamil" },
                { "ta-IN", "Tamil (India)" },
                { "ta-LK", "Tamil (Sri Lanka)" },
                { "ta-MY", "Tamil (Malaysia)" },
                { "ta-SG", "Tamil (Singapore)" },

                { "te", "Tegulu" },
                { "te-IN", "Tegulu (India)" },

                { "th", "Thai" },
                { "th-TH", "Thai (Thailand)" },

                { "tr", "Turkish" },
                { "tr-TR", "Turkish (Turkey)" },

                { "uk", "Ukrainian" },
                { "uk-UA", "Ukrainian (Ukraine)" },

                { "ur", "Urdu" },
                { "ur-IN", "Urdu (India)" },
                { "ur-PK", "Urdu (Pakistan)" },

                { "uz", "Uzbek" },
                { "uz-UZ", "Uzbek (Uzbekistan)" },

                { "vi", "Vietnamese" },
                { "vi-VN", "Vietnamese (Vietnam)" },

                { "wuu", "Wu Chinese" },
                { "wuu-CN", "Wu Chinese (China)" },

                { "yue", "Cantonese" },
                { "yue-CN", "Cantonese (China)" },

                { "zh", "Chinese" },
                { "zh-CN", "Chinese (PRC)" },
                { "zh-HK", "Chinese (Hong Kong)" },
                { "zh-TW", "Chinese (Taiwan)" },

                { "zu", "Zulu" },
                { "zu-ZA", "Zulu (South Africa)" }

            };
            m_DialectList = new List<string>();
            //using var resultasync = await m_AzureSpeechSynthesizer.GetVoicesAsync();
            resultasync = await m_AzureSpeechSynthesizer.GetVoicesAsync();
            if (resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                m_AddAzureVoiceListBox.Items.Clear();

                m_LanguageCb.Items.Add("All");
                m_DialectCb.Items.Add("All");
                m_VoiceCb.Items.Add("All");

                m_LanguageCb.SelectedIndex = 0;
                m_DialectCb.SelectedIndex = 0;
                m_VoiceCb.SelectedIndex = 0;

                foreach (var tempVoice in resultasync.Voices)
                {

                    m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName);

                    if(m_DialectCb != null && m_DialectCb.Items != null  && m_languageCodesDictionary.ContainsKey(tempVoice.Locale.ToString()))
                    {
                        if(!m_DialectCb.Items.Contains(m_languageCodesDictionary[tempVoice.Locale.ToString()]))
                        m_DialectCb.Items.Add(m_languageCodesDictionary[tempVoice.Locale.ToString()]);
                        m_DialectList.Add(m_languageCodesDictionary[tempVoice.Locale.ToString()]);
                    }
                    var buket = tempVoice.Locale.ToString().Split("-");
                    if (!m_LanguageCb.Items.Contains(buket[0].ToString()) && m_languageCodesDictionary.ContainsKey(buket[0].ToString()))
                    {
                        if(!m_LanguageCb.Items.Contains(m_languageCodesDictionary[buket[0].ToString()]))
                        m_LanguageCb.Items.Add(m_languageCodesDictionary[buket[0].ToString()]);
                    }
                     
                    m_VoiceCb.Items.Add(tempVoice.LocalName);

                    //if(tempVoice.Locale.Contains("en"))
                    //{
                    //    if(tempVoice.Locale.Contains("en-US"))
                    //    m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName + " " + tempVoice.LocalName + " US English");
                    //    //m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName + " " + tempVoice.LocalName + " English");
                    //}
                  //  m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName + tempVoice.LocalName);
                }
            }







        }

        private void m_GenderCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            //LoadAzureVoices();
            if (resultasync != null && resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                UpdateAzureVoices();
                if (m_LanguageCb.SelectedItem != null && m_DialectCb.SelectedItem != null && m_VoiceCb.SelectedItem != null)
                    updateVoiceCb();
            }
        }

        private void m_LanguageCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_DialectCb.SelectedItem != null && resultasync != null && resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                UpdateAzureVoices();
                if (m_LanguageCb.SelectedItem != null)
                    updateDialectCb();
            }
        }

        private void m_DialectCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultasync != null && resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                UpdateAzureVoices();
                if (m_LanguageCb.SelectedItem != null && m_DialectCb.SelectedItem != null)
                    updateVoiceCb();
            }
        }

        private void m_VoiceCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultasync != null && resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                UpdateAzureVoices();
                //if (m_LanguageCb.SelectedItem != null && m_DialectCb.SelectedItem != null)
                //    updateVoiceCb();
            }
        }
    }
}
