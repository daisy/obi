import os
import sys
import json
import time

# ------------------------------------------
# Parse command line
# ------------------------------------------

batch_mode = (
    len(sys.argv) > 1
    and sys.argv[1] == "--batch"
)

detect_language_mode = (
    len(sys.argv) > 1
    and sys.argv[1] == "--detect-language"
)


if detect_language_mode:

    if len(sys.argv) != 8:
        print(
            "Usage: "
            "python run_whisperx.py "
            "--detect-language "
            "input_audio output_language.txt "
            "model models_folder hf_cache nltk_data"
        )

        sys.exit(1)


    detection_audio = sys.argv[2]

    detection_output = sys.argv[3]

    MODEL_NAME = sys.argv[4]

    MODELS_DIR = sys.argv[5]

    HF_CACHE = sys.argv[6]

    NLTK_DATA_DIR = sys.argv[7]


elif batch_mode:

    if len(sys.argv) != 8:
        print("Usage...")

        sys.exit(1)


    batch_file = sys.argv[2]

    MODEL_NAME = sys.argv[3]

    BOOK_LANGUAGE = sys.argv[4]

    MODELS_DIR = sys.argv[5]

    HF_CACHE = sys.argv[6]

    NLTK_DATA_DIR = sys.argv[7]


else:

    if len(sys.argv) != 8:
        print("Usage...")

        sys.exit(1)


    input_audio = sys.argv[1]

    output_json = sys.argv[2]

    MODEL_NAME = sys.argv[3]

    BOOK_LANGUAGE = sys.argv[4]

    MODELS_DIR = sys.argv[5]

    HF_CACHE = sys.argv[6]

    NLTK_DATA_DIR = sys.argv[7]

# ------------------------------------------
# Create model folder
# ------------------------------------------

os.makedirs(MODELS_DIR, exist_ok=True)
os.makedirs(HF_CACHE, exist_ok=True)
os.makedirs(NLTK_DATA_DIR, exist_ok=True)
# ------------------------------------------
# Environment
# ------------------------------------------

os.environ["HF_HOME"] = HF_CACHE
os.environ["HF_HUB_CACHE"] = HF_CACHE
os.environ["TORCH_HOME"] = MODELS_DIR
os.environ["XDG_CACHE_HOME"] = HF_CACHE
os.environ["NLTK_DATA"] = NLTK_DATA_DIR

os.environ["HF_HUB_DISABLE_SYMLINKS_WARNING"] = "1"

import nltk

if NLTK_DATA_DIR not in nltk.data.path:
    nltk.data.path.insert(0, NLTK_DATA_DIR)

try:
    nltk.data.find("tokenizers/punkt_tab")
except LookupError:
    print("Downloading NLTK punkt_tab...")
    nltk.download("punkt_tab", download_dir=NLTK_DATA_DIR)

# MUST be after environment variables
import whisperx

# ---------------------------------------------------
# SETTINGS
# ---------------------------------------------------

device = "cpu"

LANGUAGE_NAMES = {
    "en": "English",
    "fr": "French",
    "de": "German",
    "es": "Spanish",
    "it": "Italian",
    "ar": "Arabic",
    "ca": "Catalan",
    "cs": "Czech",
    "da": "Danish",
    "el": "Greek",
    "eu": "Basque",
    "fa": "Persian",
    "fi": "Finnish",
    "gl": "Galician",
    "he": "Hebrew",
    "hi": "Hindi",
    "hr": "Croatian",
    "hu": "Hungarian",
    "id": "Indonesian",
    "ja": "Japanese",
    "ka": "Georgian",
    "ko": "Korean",
    "lv": "Latvian",
    "ml": "Malayalam",
    "nl": "Dutch",
    "nn": "Norwegian Nynorsk",
    "no": "Norwegian",
    "pl": "Polish",
    "pt": "Portuguese",
    "ro": "Romanian",
    "ru": "Russian",
    "sk": "Slovak",
    "sl": "Slovenian",
    "sv": "Swedish",
    "te": "Telugu",
    "tl": "Filipino",
    "tr": "Turkish",
    "uk": "Ukrainian",
    "ur": "Urdu",
    "vi": "Vietnamese",
    "zh": "Chinese"
}

# ---------------------------------------------------
# LOAD MODEL
# ---------------------------------------------------

if detect_language_mode:

    print(
        "WhisperX language detection mode")

    print(
        f"Model: {MODEL_NAME}")

    model = whisperx.load_model(
        MODEL_NAME,
        device,
        compute_type="float32")
    print(
    "WhisperX model type:",
    type(model))

    print(
        "WhisperX model attributes:"
    )

    print(
        [
            name
            for name in dir(model)
            if "model" in name.lower()
            or "language" in name.lower()
            or "detect" in name.lower()
        ]
    )  
    import inspect

    print(
        "detect_language signature:",
        inspect.signature(
            model.detect_language
        )
    )

else:

    if BOOK_LANGUAGE == "auto":

        print(
            "Book Language: Auto Detect")

    else:

        display_name = LANGUAGE_NAMES.get(
            BOOK_LANGUAGE,
            BOOK_LANGUAGE)

        print(
            f"Book Language: "
            f"{display_name} (User Selected)")


    print(
        "Loading WhisperX model...")

    print(
        f"Model: {MODEL_NAME}")

    model = whisperx.load_model(
        MODEL_NAME,
        device,
        compute_type="float32",
        language=
            BOOK_LANGUAGE
            if BOOK_LANGUAGE != "auto"
            else None)


print("Whisper model loaded")

# ---------------------------------------------------
# LANGUAGE DETECTION ONLY
# ---------------------------------------------------

if detect_language_mode:

    print(
        "WhisperX language detection mode"
    )

    print(
        f"Audio: {detection_audio}"
    )

    print(
        "Loading audio for language detection..."
    )

    audio = whisperx.load_audio(
        detection_audio
    )

    # ---------------------------------------------------
    # Only use the first 30 seconds.
    # ---------------------------------------------------

    sample_rate = 16000

    max_samples = (
        30 * sample_rate
    )

    audio_30s = audio[
        :max_samples
    ]

    print(
        "Detecting language from first 30 seconds..."
    )

    detection_start = time.perf_counter()

    detected_language = model.detect_language(
        audio_30s
    )

    detection_end = time.perf_counter()

    print(
        f"Language detection processing time: "
        f"{detection_end - detection_start:.2f} seconds"
    )

    print(
        f"Detected language: "
        f"{detected_language}"
    )
    with open(
        detection_output,
        "w",
        encoding="utf-8"
    ) as f:

        f.write(
            detected_language
        )

    print(
        "Language detection completed"
    )

    sys.exit(0)

# ---------------------------------------------------
# ALIGNMENT MODEL CACHE
# ---------------------------------------------------

alignment_cache = {}

                    
# ---------------------------------------------------
# TRANSCRIBE SINGLE FILE
# ---------------------------------------------------

def transcribe_file(
        model,
        alignment_cache,
        input_audio,
        output_json,
        language=None):
            
            


                # ---------------------------------------------------
                # LOAD AUDIO
                # ---------------------------------------------------

                print("Loading audio...")
                

                print("Audio file:", os.path.abspath(input_audio))

                exists = os.path.exists(input_audio)

                print("Exists:", exists)

                if exists:
                    print("Size:", os.path.getsize(input_audio))
                else:
                    raise Exception(f"Audio file not found: {input_audio}")
                
                audio = whisperx.load_audio(
                    input_audio)                    
  
                print("Audio type:", type(audio))
                print("Audio shape:", audio.shape)
                print("Audio dtype:", audio.dtype)
                print("Audio length:", len(audio))
                
                if len(audio) == 0:
                    raise Exception(
                        f"Failed to decode audio file:\n"
                        f"{input_audio}\n\n"
                        "The decoded waveform contains zero audio samples.\n"
                        "Possible causes:\n"
                        "- The audio file is empty or corrupted.\n"
                        "- FFmpeg could not decode the audio.\n"
                        "- The audio format is unsupported."
                    )
                    
                print("Audio loaded")

                # ---------------------------------------------------
                # TRANSCRIBE
                # ---------------------------------------------------
                print("Transcribing audio...")

                # ---------------------------------------
                # AUTO DETECT vs EXPLICIT LANGUAGE
                # ---------------------------------------

                if language is None or language == "auto":

                    print("Language: Auto Detect")

                    result = model.transcribe(
                        audio,
                        batch_size=2)

                else:

                    display_name = LANGUAGE_NAMES.get(language, language)
                    print(f"Language: {display_name}")

                    result = model.transcribe(
                        audio,
                        batch_size=2,
                        language=language)

                print("Transcription completed")
                
                detected_language = result["language"]

                # ---------------------------------------------------
                # LOAD ALIGNMENT MODEL
                # ---------------------------------------------------
                alignment_language = detected_language

                if alignment_language  not in alignment_cache:

                    print(
                        f"Loading alignment model ({alignment_language})...")

                    model_a, metadata = whisperx.load_align_model(
                    language_code=alignment_language,
                    device=device)

                    alignment_cache[alignment_language] = {
                        "model": model_a,
                        "metadata": metadata
                    }

                    print(
                        "Alignment model loaded")
                else:
                    print(
                        f"Using cached alignment model ({alignment_language})")

                cached = alignment_cache[alignment_language]

                model_a = cached["model"]

                metadata = cached["metadata"]

                # ---------------------------------------------------
                # ALIGN WORD TIMESTAMPS
                # ---------------------------------------------------

                print("Aligning timestamps...")

                result = whisperx.align(
                    result["segments"],
                    model_a,
                    metadata,
                    audio,
                    device)

                print("Alignment completed")

                # ---------------------------------------------------
                # BUILD PHRASES
                # ---------------------------------------------------

                word_segments = result.get(
                    "word_segments",
                    [])

                phrases = []

                current_phrase = []


                def flush_phrase():


                    if len(current_phrase) == 0:
                        return

                    first_word = current_phrase[0]

                    last_word = current_phrase[-1]

                    # -----------------------------------------
                    # DAISY timing safety padding
                    # -----------------------------------------

                    start_padding = 0.20
                    end_padding = 0.25

                    start_time = max(
                        0,
                        first_word["start"] - start_padding)

                    end_time = (
                        last_word["end"] + end_padding)

                    # -----------------------------------------
                    # Build phrase text
                    # -----------------------------------------

                    phrase_text = " ".join(
                        w["word"]
                        for w in current_phrase)

                    # -----------------------------------------
                    # Calculate phrase confidence
                    # -----------------------------------------

                    confidence_values = []

                    for w in current_phrase:

                        confidence_values.append(
                            w.get("score", 1.0))

                    phrase_confidence = (
                        sum(confidence_values) /
                        len(confidence_values))

                    # -----------------------------------------
                    # Build word timestamps
                    # -----------------------------------------

                    phrase_words = []

                    for w in current_phrase:

                        phrase_words.append({
                            "word":
                                w["word"],

                            "start":
                                round(
                                    w["start"],
                                    3),

                            "end":
                                round(
                                    w["end"],
                                    3),

                            "confidence":
                                round(
                                    w.get("score", 1.0),
                                    3)
                        })

                    # -----------------------------------------
                    # Add phrase
                    # -----------------------------------------

                    phrases.append({
                        "phraseId":
                            f"p{len(phrases) + 1}",

                        "start":
                            round(
                                start_time,
                                3),

                        "end":
                            round(
                                end_time,
                                3),

                        "text":
                            phrase_text,

                        "confidence":
                            round(
                                phrase_confidence,
                                3),

                        "words":
                            phrase_words
                    })

                    current_phrase.clear()


                def merge_short_phrases(phrases):

                    if len(phrases) <= 1:
                        return phrases

                    merged = []

                    merged.append(phrases[0])

                    for i in range(1, len(phrases)):

                        current = phrases[i]

                        word_count = len(
                            current["text"].split())

                        duration = (
                            current["end"] -
                            current["start"])

                        too_short = (
                            word_count <= 2 or
                            duration <= 1.0)

                        if too_short:

                            previous = merged[-1]

                            previous["text"] = (
                                previous["text"] +
                                " " +
                                current["text"])

                            previous["end"] = (
                                current["end"])

                            previous["words"].extend(
                                current["words"])

                        else:
                            merged.append(current)

                    return merged


                for i, word in enumerate(word_segments):

                    # Skip invalid timestamp entries
                    if "start" not in word or "end" not in word:
                        continue

                    current_phrase.append(word)

                    should_break = False

                    # Break on punctuation
                    if word["word"].endswith(
                        (".", "!", "?", "?")):

                        should_break = True

                    # Break on speech pause
                    if i < len(word_segments) - 1:

                        next_word = word_segments[i + 1]

                        if "start" in next_word:

                            pause = (
                                next_word["start"] -
                                word["end"])

                            if pause > 0.9:
                                should_break = True

                    # Break on phrase text length
                    phrase_text = " ".join(
                        w["word"]
                        for w in current_phrase)

                    if len(phrase_text) >= 80:
                        should_break = True

                    if should_break:
                        flush_phrase()

                flush_phrase()

                # Merge tiny phrases
                phrases = merge_short_phrases(
                    phrases)

                # ---------------------------------------------------
                # FINAL JSON
                # ---------------------------------------------------

                output = {
                    "phrases": phrases
                }

                # ---------------------------------------------------
                # SAVE JSON
                # ---------------------------------------------------

                print("Saving JSON...")

                with open(
                    output_json,
                    "w",
                    encoding="utf-8") as f:

                    json.dump(
                        output,
                        f,
                        ensure_ascii=False,
                        indent=2)

                print("Completed")
                return detected_language

if batch_mode:

    print(
        "Loading batch job...")

    with open(
        batch_file,
        "r",
        encoding="utf-8") as f:

        jobs = json.load(f)

    files = jobs["files"]

    print(
        f"{len(files)} files found.")
        
        
    detected_book_language = (
    None
    if BOOK_LANGUAGE == "auto"
    else BOOK_LANGUAGE
   )

    for index, job in enumerate(files, start=1):

        print(
            f"Processing file {index} of {len(files)}")

        print(
            job["input"])

        detected_language = transcribe_file(
            model,
            alignment_cache,
            job["input"],
            job["output"],
            detected_book_language)

        if detected_book_language is None:

            detected_book_language = detected_language

            display_name = LANGUAGE_NAMES.get(detected_book_language, detected_book_language)
            print(f"Book Language: {display_name} (Auto Detected)")

else:

        transcribe_file(
            model,
            alignment_cache,
            input_audio,
            output_json,
            BOOK_LANGUAGE)