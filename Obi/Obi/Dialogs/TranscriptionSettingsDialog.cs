using Obi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class TranscriptionSettingsDialog : Form
    {
        private WhisperModel m_Model;
        private string m_BookLanguage = "en";
        private TranscriptionEngine m_TranscriptionEngine =
            TranscriptionEngine.Parakeet;

        private bool m_UpdatingLanguageEngineLists;

        public TranscriptionSettings SelectedTranscriptionSettings
        {
            get
            {
                return new TranscriptionSettings
                {
                    Engine =
                        m_TranscriptionEngine,

                    Language =
                        m_BookLanguage,

                    WhisperModel =
                        m_Model
                };
            }
        }

        public TranscriptionSettingsDialog()
        {
            InitializeComponent();

            // --------------------------------------------------
            // ENGINE
            // --------------------------------------------------

            m_TranscriptionEngineCb.DisplayMember =
                nameof(TranscriptionEngineItem.DisplayName);

            m_TranscriptionEngineCb.ValueMember =
                nameof(TranscriptionEngineItem.Engine);

            m_TranscriptionEngineCb.SelectedIndexChanged +=
                m_TranscriptionEngineCb_SelectedIndexChanged;


            // --------------------------------------------------
            // WHISPER MODEL
            // --------------------------------------------------

            m_ModelCb.DataSource =
                new List<WhisperModelItem>
                {
                    new()
                    {
                        Model =
                            WhisperModel.Large,

                        DisplayName =
                            "Large (Best Accuracy)"
                    },

                    new()
                    {
                        Model =
                            WhisperModel.Medium,

                        DisplayName =
                            "Medium (Balanced)"
                    },

                    new()
                    {
                        Model =
                            WhisperModel.Small,

                        DisplayName =
                            "Small (Fastest)"
                    }
                };

            m_ModelCb.DisplayMember =
                "DisplayName";

            // Medium is the default.
            m_ModelCb.SelectedIndex = 1;

            m_Model =
                ((WhisperModelItem)m_ModelCb.SelectedItem).Model;


            // --------------------------------------------------
            // LANGUAGE
            // --------------------------------------------------

            m_BookLanguageCb.DisplayMember =
                nameof(WhisperLanguageItem.DisplayName);

            m_BookLanguageCb.ValueMember =
                nameof(WhisperLanguageItem.LanguageCode);

            m_BookLanguageCb.SelectedIndexChanged +=
                m_BookLanguageCb_SelectedIndexChanged;


            // --------------------------------------------------
            // INITIAL SELECTIONS
            //
            // Same defaults as ImportAudioUsingWhisper:
            //
            // Engine   = Parakeet
            // Language = English
            // Model    = Medium
            // --------------------------------------------------

            InitializeLanguageAndEngineSelections();

            UpdateWhisperModelAvailability();
        }


        // ==========================================================
        // INITIALIZE LANGUAGE / ENGINE SELECTIONS
        // ==========================================================

        private void InitializeLanguageAndEngineSelections()
        {
            m_UpdatingLanguageEngineLists = true;

            try
            {
                // --------------------------------------------------
                // Engine list
                // --------------------------------------------------

                m_TranscriptionEngineCb.DataSource =
                    CreateEngineItems();

                m_TranscriptionEngineCb.SelectedValue =
                    TranscriptionEngine.Parakeet;


                // --------------------------------------------------
                // Language list
                //
                // Parakeet is initially selected, therefore only
                // Parakeet-supported languages are shown.
                // --------------------------------------------------

                m_BookLanguageCb.DataSource =
                    CreateLanguageItems(
                        includeAllLanguages: false);

                m_BookLanguageCb.SelectedValue =
                    "en";


                // --------------------------------------------------
                // Internal values
                // --------------------------------------------------

                m_TranscriptionEngine =
                    TranscriptionEngine.Parakeet;

                m_BookLanguage =
                    "en";
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // CREATE ENGINE LIST
        // ==========================================================

        private static List<TranscriptionEngineItem>
            CreateEngineItems()
        {
            return new List<TranscriptionEngineItem>
            {
                new()
                {
                    Engine =
                        TranscriptionEngine.Parakeet,

                    DisplayName =
                        "Parakeet"
                },

                new()
                {
                    Engine =
                        TranscriptionEngine.Whisper,

                    DisplayName =
                        "Whisper"
                }
            };
        }


        // ==========================================================
        // CREATE LANGUAGE LIST
        // ==========================================================

        private static List<WhisperLanguageItem>
            CreateLanguageItems(
                bool includeAllLanguages)
        {
            if (includeAllLanguages)
            {
                return WhisperLanguages.Languages
                    .ToList();
            }

            return ParakeetLanguages.Languages
                .ToList();
        }


        // ==========================================================
        // BOOK LANGUAGE CHANGED
        // ==========================================================

        private void m_BookLanguageCb_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (m_UpdatingLanguageEngineLists)
                return;

            if (m_BookLanguageCb.SelectedItem
                is not WhisperLanguageItem selectedLanguage)
            {
                return;
            }

            string language =
                string.IsNullOrWhiteSpace(
                    selectedLanguage.LanguageCode)
                    ? "en"
                    : selectedLanguage.LanguageCode
                        .Trim()
                        .ToLowerInvariant();

            m_BookLanguage =
                language;
        }


        // ==========================================================
        // TRANSCRIPTION ENGINE CHANGED
        // ==========================================================

        private void m_TranscriptionEngineCb_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (m_UpdatingLanguageEngineLists)
                return;

            if (m_TranscriptionEngineCb.SelectedItem
                is not TranscriptionEngineItem selectedEngine)
            {
                return;
            }

            TranscriptionEngine engine =
                selectedEngine.Engine;

            m_TranscriptionEngine =
                engine;

            UpdateWhisperModelAvailability();


            // ------------------------------------------------------
            // PARAKEET
            //
            // Only Parakeet-supported languages are available.
            // Auto Detect is not available.
            // ------------------------------------------------------

            if (engine ==
                TranscriptionEngine.Parakeet)
            {
                string language =
                    string.IsNullOrWhiteSpace(
                        m_BookLanguage)
                        ? "en"
                        : m_BookLanguage
                            .Trim()
                            .ToLowerInvariant();

                bool supported =
                    ParakeetLanguages.SupportedCodes.Contains(
                        language);

                if (!supported)
                {
                    SetLanguageSelection("en");
                }

                RefreshLanguageList(
                    parakeetOnly: true);

                return;
            }


            // ------------------------------------------------------
            // WHISPER
            //
            // Auto Detect + all Whisper-supported languages.
            // ------------------------------------------------------

            RefreshLanguageList(
                parakeetOnly: false);
        }


        // ==========================================================
        // UPDATE WHISPER MODEL AVAILABILITY
        // ==========================================================

        private void UpdateWhisperModelAvailability()
        {
            if (m_TranscriptionEngineCb.SelectedItem
                is not TranscriptionEngineItem selectedEngine)
            {
                return;
            }

            m_ModelCb.Enabled =
                selectedEngine.Engine !=
                TranscriptionEngine.Parakeet;
        }


        // ==========================================================
        // REFRESH LANGUAGE LIST
        // ==========================================================

        private void RefreshLanguageList(
            bool parakeetOnly)
        {
            string selectedLanguage =
                string.IsNullOrWhiteSpace(
                    m_BookLanguage)
                    ? "en"
                    : m_BookLanguage
                        .Trim()
                        .ToLowerInvariant();

            m_UpdatingLanguageEngineLists = true;

            try
            {
                List<WhisperLanguageItem> languages =
                    CreateLanguageItems(
                        includeAllLanguages:
                            !parakeetOnly);

                m_BookLanguageCb.DataSource =
                    languages;

                bool selectionExists =
                    languages.Any(
                        language =>
                            language.LanguageCode.Equals(
                                selectedLanguage,
                                StringComparison.OrdinalIgnoreCase));

                if (selectionExists)
                {
                    m_BookLanguageCb.SelectedValue =
                        selectedLanguage;
                }
                else
                {
                    string fallbackLanguage =
                        parakeetOnly
                            ? "en"
                            : "auto";

                    m_BookLanguage =
                        fallbackLanguage;

                    m_BookLanguageCb.SelectedValue =
                        fallbackLanguage;
                }
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // SET LANGUAGE SELECTION
        // ==========================================================

        private void SetLanguageSelection(
            string language)
        {
            language =
                string.IsNullOrWhiteSpace(language)
                    ? "en"
                    : language
                        .Trim()
                        .ToLowerInvariant();

            m_UpdatingLanguageEngineLists = true;

            try
            {
                m_BookLanguage =
                    language;

                m_BookLanguageCb.SelectedValue =
                    language;
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // OK
        // ==========================================================

        private void m_btnOK_Click(
            object? sender,
            EventArgs e)
        {
            // Read the final selections directly from the controls
            // before closing the dialog.

            if (m_TranscriptionEngineCb.SelectedItem
                is TranscriptionEngineItem selectedEngine)
            {
                m_TranscriptionEngine =
                    selectedEngine.Engine;
            }

            if (m_BookLanguageCb.SelectedItem
                is WhisperLanguageItem selectedLanguage)
            {
                m_BookLanguage =
                    string.IsNullOrWhiteSpace(
                        selectedLanguage.LanguageCode)
                        ? "en"
                        : selectedLanguage.LanguageCode
                            .Trim()
                            .ToLowerInvariant();
            }

            if (m_ModelCb.SelectedItem
                is WhisperModelItem selectedModel)
            {
                m_Model =
                    selectedModel.Model;
            }

            DialogResult =
                DialogResult.OK;

            Close();
        }


        // ==========================================================
        // CANCEL
        // ==========================================================

        private void m_btnCancel_Click(
            object? sender,
            EventArgs e)
        {
            DialogResult =
                DialogResult.Cancel;

            Close();
        }
    }
}