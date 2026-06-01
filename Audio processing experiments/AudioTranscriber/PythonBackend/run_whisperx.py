import os
import sys
import json

# ---------------------------------------------------
# PROJECT PATHS
# ---------------------------------------------------

BASE_DIR = os.path.dirname(
    os.path.dirname(
        os.path.abspath(__file__)))

MODELS_DIR = os.path.abspath(
    os.path.join(
        BASE_DIR,
        "Models"))

os.makedirs(
    MODELS_DIR,
    exist_ok=True)

# ---------------------------------------------------
# FORCE LOCAL MODEL STORAGE
# MUST BE BEFORE whisperx import
# ---------------------------------------------------

os.environ["HF_HOME"] = MODELS_DIR
os.environ["HF_HUB_DISABLE_SYMLINKS_WARNING"] = "1"
os.environ["TORCH_HOME"] = MODELS_DIR
os.environ["XDG_CACHE_HOME"] = MODELS_DIR

# ---------------------------------------------------
# IMPORTS
# ---------------------------------------------------

import whisperx

# ---------------------------------------------------
# VALIDATE ARGUMENTS
# ---------------------------------------------------

if len(sys.argv) < 3:

    print(
        "Usage: python run_whisperx.py input.wav output.json")

    sys.exit(1)

input_audio = sys.argv[1]
output_json = sys.argv[2]

# ---------------------------------------------------
# SETTINGS
# ---------------------------------------------------

device = "cpu"

MODEL_NAME = "large-v3"

# ---------------------------------------------------
# LOAD MODEL
# ---------------------------------------------------

print("Loading WhisperX model...")

model = whisperx.load_model(
    MODEL_NAME,
    device,
    compute_type="float32")

print("Whisper model loaded")

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

result = model.transcribe(
    audio,
    batch_size=2)

print("Transcription completed")

# ---------------------------------------------------
# LOAD ALIGNMENT MODEL
# ---------------------------------------------------

print("Loading alignment model...")

model_a, metadata = whisperx.load_align_model(
    language_code=result["language"],
    device=device)

print("Alignment model loaded")

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

    global current_phrase

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

    current_phrase = []


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