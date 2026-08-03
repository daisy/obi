import whisperx.alignment as alignment

print()
print("WhisperX Default Alignment Models")
print("=" * 60)
print()

# Dictionary used internally by WhisperX
default_models = alignment.DEFAULT_ALIGN_MODELS_HF

print()
print("TORCH MODELS")
print("=" * 40)

try:

    for code, model in alignment.DEFAULT_ALIGN_MODELS_TORCH.items():

        print(f"{code} -> {model}")

except Exception as ex:

    print(ex)

supported = []

for code in sorted(default_models.keys()):

    model_name = default_models[code]

    supported.append((code, model_name))

    print(f"{code:5} -> {model_name}")

print()
print("=" * 60)
print("C# LIST")
print("=" * 60)
print()

print('new WhisperLanguageItem')
print('{')
print('    DisplayName = "Auto Detect (Recommended)",')
print('    LanguageCode = "auto"')
print('},')
print()

# Whisper language names
language_names = {
    "af": "Afrikaans",
    "ar": "Arabic",
    "bg": "Bulgarian",
    "ca": "Catalan",
    "cs": "Czech",
    "da": "Danish",
    "de": "German",
    "el": "Greek",
    "en": "English",
    "es": "Spanish",
    "et": "Estonian",
    "eu": "Basque",
    "fa": "Persian",
    "fi": "Finnish",
    "fr": "French",
    "gl": "Galician",
    "he": "Hebrew",
    "hi": "Hindi",
    "hr": "Croatian",
    "hu": "Hungarian",
    "hy": "Armenian",
    "id": "Indonesian",
    "is": "Icelandic",
    "it": "Italian",
    "ja": "Japanese",
    "ko": "Korean",
    "lt": "Lithuanian",
    "lv": "Latvian",
    "mk": "Macedonian",
    "ml": "Malayalam",
    "mr": "Marathi",
    "ms": "Malay",
    "nl": "Dutch",
    "nn": "Norwegian Nynorsk",
    "no": "Norwegian",
    "pl": "Polish",
    "pt": "Portuguese",
    "ro": "Romanian",
    "ru": "Russian",
    "sk": "Slovak",
    "sl": "Slovenian",
    "sr": "Serbian",
    "sv": "Swedish",
    "ta": "Tamil",
    "te": "Telugu",
    "th": "Thai",
    "tr": "Turkish",
    "uk": "Ukrainian",
    "ur": "Urdu",
    "vi": "Vietnamese",
    "zh": "Chinese"
}

for code in sorted(default_models.keys()):

    display_name = language_names.get(code, code)

    print("new WhisperLanguageItem")
    print("{")
    print(f'    DisplayName = "{display_name}",')
    print(f'    LanguageCode = "{code}"')
    print("},")