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
    nltk.data.find("tokenizers/punkt_tab/english/")
except LookupError as exc:
    raise RuntimeError(
        "The local NLTK punkt_tab resource is missing. "
        f"Expected it under: {NLTK_DATA_DIR}\\tokenizers\\punkt_tab. "
        "Repair or reinstall Obi so its bundled NLTK data is copied."
    ) from exc

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

                # Phrase segmentation settings. These are duration targets,
                # not character limits: word alignment remains untouched.
                TARGET_PHRASE_SECONDS = 12.0
                MIN_PHRASE_SECONDS = 7.0
                SOFT_MAX_PHRASE_SECONDS = 15.0
                HARD_MAX_PHRASE_SECONDS = 18.0
                PAUSE_BREAK_SECONDS = 0.9

                SENTENCE_ENDINGS = (
                    ".", "!", "?", "。", "！", "？", "…"
                )

                # Periods in common English titles/abbreviations must not
                # terminate a phrase. In particular, splitting after "Mr."
                # separates the title from the person's name.
                ENGLISH_NON_TERMINAL_ABBREVIATIONS = {
                    "mr", "mrs", "ms", "miss", "dr", "prof",
                    "rev", "fr", "sr", "jr", "st", "mt",
                    "gen", "col", "maj", "capt", "cmdr",
                    "lt", "sgt", "hon", "pres", "gov",
                    "messrs", "mme", "mlle"
                }

                CLAUSE_ENDINGS = (
                    ",", ";", ":", "—", "–"
                )

                # English-only grammatical hints. Other languages still use
                # punctuation, pauses and duration limits below.
                is_english = (
                    (BOOK_LANGUAGE or "").lower().split("-")[0] == "en"
                )

                ENGLISH_CONTINUATION_STARTERS = {
                    "and", "but", "or", "so", "yet", "because",
                    "although", "though", "while", "whereas",
                    "which", "who", "whom", "whose", "that",
                    "when", "where", "if", "unless", "until",
                    "since", "as", "after", "before"
                }

                ENGLISH_BAD_END_WORDS = {
                    "a", "an", "the", "and", "or", "but", "nor",
                    "of", "to", "for", "from", "with", "by",
                    "in", "on", "at", "into", "onto", "upon",
                    "as", "than", "that", "which", "who",
                    "whose", "because", "although", "while"
                }

                def clean_token(token):
                    return (token or "").strip()

                def token_lower(token):
                    return clean_token(token).lower().strip(
                        " \t\r\n.,;:!?\"'“”‘’()[]{}"
                    )

                def strip_closing_punctuation(token):
                    return clean_token(token).rstrip("\\\"'”’)]}")

                def ends_sentence(token):
                    cleaned = strip_closing_punctuation(token)

                    # Do not treat a full stop in an English title as the
                    # end of a sentence/phrase. Keep the title attached to
                    # the following name (e.g. "Mr. Philotson").
                    if is_english and cleaned.endswith("."):
                        abbreviation = token_lower(cleaned)
                        if abbreviation in ENGLISH_NON_TERMINAL_ABBREVIATIONS:
                            return False

                    return cleaned.endswith(SENTENCE_ENDINGS)

                def ends_clause(token):
                    return strip_closing_punctuation(token).endswith(
                        CLAUSE_ENDINGS
                    )

                def phrase_duration(words):
                    if not words:
                        return 0.0
                    return max(
                        0.0,
                        float(words[-1]["end"]) - float(words[0]["start"])
                    )

                def flush_phrase(words_to_flush=None):
                    phrase_source = (
                        current_phrase if words_to_flush is None
                        else words_to_flush
                    )

                    if not phrase_source:
                        return

                    first_word = phrase_source[0]
                    last_word = phrase_source[-1]

                    # Preserve the existing DAISY timing safety padding.
                    start_padding = 0.20
                    end_padding = 0.25

                    start_time = max(
                        0,
                        first_word["start"] - start_padding
                    )
                    end_time = last_word["end"] + end_padding

                    phrase_text = " ".join(
                        w["word"] for w in phrase_source
                    )

                    confidence_values = [
                        w.get("score", 1.0) for w in phrase_source
                    ]
                    phrase_confidence = (
                        sum(confidence_values) / len(confidence_values)
                    )

                    phrase_words = []
                    for w in phrase_source:
                        phrase_words.append({
                            "word": w["word"],
                            "start": round(w["start"], 3),
                            "end": round(w["end"], 3),
                            "confidence": round(w.get("score", 1.0), 3)
                        })

                    phrases.append({
                        "phraseId": f"p{len(phrases) + 1}",
                        "start": round(start_time, 3),
                        "end": round(end_time, 3),
                        "text": phrase_text,
                        "confidence": round(phrase_confidence, 3),
                        "words": phrase_words
                    })

                    if words_to_flush is None:
                        current_phrase.clear()

                def choose_long_phrase_boundary(words):
                    """
                    Select a natural word boundary once a phrase becomes
                    long. Prefer clause punctuation and grammatical clause
                    transitions; only force a split at the hard duration cap.
                    Returns the index of the final word in the left phrase.
                    """
                    if len(words) < 2:
                        return None

                    candidates = []
                    for boundary_index in range(len(words) - 1):
                        left = words[boundary_index]
                        right = words[boundary_index + 1]

                        elapsed = (
                            float(left["end"]) -
                            float(words[0]["start"])
                        )

                        if elapsed < MIN_PHRASE_SECONDS:
                            continue

                        if elapsed > SOFT_MAX_PHRASE_SECONDS:
                            continue

                        left_token = clean_token(left["word"])
                        right_token = token_lower(right["word"])

                        score = 0.0

                        if ends_clause(left_token):
                            score += 5.0

                        if is_english and right_token in ENGLISH_CONTINUATION_STARTERS:
                            score += 3.0

                        pause = (
                            float(right["start"]) -
                            float(left["end"])
                        )
                        if pause >= 0.35:
                            score += min(pause, 1.0) * 2.0

                        if is_english and token_lower(left_token) in ENGLISH_BAD_END_WORDS:
                            score -= 5.0

                        # Prefer boundaries near the target duration, but
                        # grammatical quality is more important than length.
                        score -= abs(elapsed - TARGET_PHRASE_SECONDS) * 0.12

                        if score > 0:
                            candidates.append((score, boundary_index))

                    if candidates:
                        candidates.sort(key=lambda item: item[0], reverse=True)
                        return candidates[0][1]

                    # Do not force a split before the hard limit. At the
                    # limit, choose the nearest safe word boundary.
                    if phrase_duration(words) < HARD_MAX_PHRASE_SECONDS:
                        return None

                    eligible = []
                    for boundary_index in range(len(words) - 1):
                        left = words[boundary_index]
                        elapsed = (
                            float(left["end"]) -
                            float(words[0]["start"])
                        )
                        if elapsed >= MIN_PHRASE_SECONDS:
                            penalty = abs(elapsed - TARGET_PHRASE_SECONDS)
                            if is_english and token_lower(left["word"]) in ENGLISH_BAD_END_WORDS:
                                penalty += 100.0
                            if ends_clause(left["word"]):
                                penalty -= 3.0
                            eligible.append((penalty, boundary_index))

                    if eligible:
                        eligible.sort(key=lambda item: item[0])
                        return eligible[0][1]

                    # Last-resort safety split for exceptionally long
                    # continuous speech with no detectable clause boundary.
                    for boundary_index, word_item in enumerate(words[:-1]):
                        if (
                            float(word_item["end"]) -
                            float(words[0]["start"])
                        ) >= HARD_MAX_PHRASE_SECONDS:
                            return boundary_index

                    return len(words) - 2

                # Discard malformed alignment records before segmentation so
                # pauses are always measured between adjacent valid words.
                valid_word_segments = [
                    word for word in word_segments
                    if (
                        "start" in word and "end" in word
                        and word.get("word")
                        and float(word["end"]) >= float(word["start"])
                    )
                ]

                for i, word in enumerate(valid_word_segments):
                    current_phrase.append(word)

                    # A complete sentence is always a preferred phrase end.
                    should_break = ends_sentence(word["word"])

                    # A genuine silence remains a reliable boundary in every
                    # supported language.
                    if i < len(valid_word_segments) - 1:
                        next_word = valid_word_segments[i + 1]
                        pause = (
                            float(next_word["start"]) -
                            float(word["end"])
                        )
                        if pause > PAUSE_BREAK_SECONDS:
                            should_break = True

                    if should_break:
                        flush_phrase()
                        continue

                    # Split long phrases at a clause-aware word boundary.
                    # This replaces the old unconditional 80-character cut.
                    if (
                        phrase_duration(current_phrase)
                        >= TARGET_PHRASE_SECONDS
                    ):
                        boundary_index = choose_long_phrase_boundary(
                            current_phrase
                        )

                        if boundary_index is not None:
                            left_phrase = current_phrase[:boundary_index + 1]
                            remaining_phrase = current_phrase[boundary_index + 1:]

                            flush_phrase(left_phrase)
                            current_phrase = remaining_phrase

                flush_phrase()

                def merge_short_phrases(phrase_list):
                    """
                    Merge tiny fragments only when doing so will not erase a
                    completed sentence boundary. Re-number IDs after merging.
                    """
                    if len(phrase_list) <= 1:
                        return phrase_list

                    merged = []
                    for phrase in phrase_list:
                        word_count = len(phrase["text"].split())
                        duration = phrase["end"] - phrase["start"]
                        too_short = word_count <= 2 or duration <= 1.0

                        if merged and too_short:
                            previous = merged[-1]

                            # Never append a new phrase to a previous complete
                            # sentence; that sentence boundary is meaningful.
                            previous_words = previous["text"].rstrip()
                            if not previous_words.endswith(SENTENCE_ENDINGS):
                                previous["text"] += " " + phrase["text"]
                                previous["end"] = phrase["end"]
                                previous["words"].extend(phrase["words"])
                                previous["confidence"] = round(
                                    (
                                        previous["confidence"] *
                                        (len(previous["words"]) - len(phrase["words"])) +
                                        phrase["confidence"] * len(phrase["words"])
                                    ) / max(1, len(previous["words"])),
                                    3
                                )
                                continue

                        merged.append(phrase)

                    for index, phrase in enumerate(merged, start=1):
                        phrase["phraseId"] = f"p{index}"

                    return merged

                phrases = merge_short_phrases(phrases)

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