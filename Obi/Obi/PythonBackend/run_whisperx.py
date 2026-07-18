import os
import sys
import json

# ------------------------------------------
# Parse command line
# ------------------------------------------

batch_mode = len(sys.argv) > 1 and sys.argv[1] == "--batch"

if batch_mode:

    if len(sys.argv) != 7:
        print("Usage...")
        sys.exit(1)

    batch_file = sys.argv[2]
    MODEL_NAME = sys.argv[3]
    MODELS_DIR = sys.argv[4]
    HF_CACHE = sys.argv[5]
    NLTK_DATA_DIR = sys.argv[6]

else:

    if len(sys.argv) != 7:
        print("Usage...")
        sys.exit(1)

    input_audio = sys.argv[1]
    output_json = sys.argv[2]
    MODEL_NAME = sys.argv[3]
    MODELS_DIR = sys.argv[4]
    HF_CACHE = sys.argv[5]
    NLTK_DATA_DIR = sys.argv[6]

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


# ---------------------------------------------------
# LOAD MODEL
# ---------------------------------------------------

print("Loading WhisperX model...")

print(f"Model: {MODEL_NAME}")

model = whisperx.load_model(
    MODEL_NAME,
    device,
    compute_type="float32")

print("Whisper model loaded")

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

                audio = whisperx.load_audio(
                    input_audio)

                print("Audio loaded")

                # ---------------------------------------------------
                # TRANSCRIBE
                # ---------------------------------------------------
                print("Transcribing audio...")

                if language is None:

                    result = model.transcribe(
                        audio,
                        batch_size=2)

                else:

                    result = model.transcribe(
                        audio,
                        batch_size=2,
                        language=language)

                print("Transcription completed")
                
                detected_language = result["language"]

                # ---------------------------------------------------
                # LOAD ALIGNMENT MODEL
                # ---------------------------------------------------
                language = detected_language

                if language not in alignment_cache:

                    print(
                        f"Loading alignment model ({language})...")

                    model_a, metadata = whisperx.load_align_model(
                        language_code=language,
                        device=device)

                    alignment_cache[language] = {
                        "model": model_a,
                        "metadata": metadata
                    }

                    print(
                        "Alignment model loaded")
                else:
                    print(
                        f"Using cached alignment model ({language})")

                cached = alignment_cache[language]

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
        
        
    detected_book_language = None

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

            print(
                f"Book language: {detected_book_language}")

else:

        transcribe_file(
            model,
            alignment_cache,
            input_audio,
            output_json,
            None)