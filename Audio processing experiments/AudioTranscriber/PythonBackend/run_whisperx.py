
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
# IMPORTANT:
# MUST BE BEFORE importing whisperx
# ---------------------------------------------------

os.environ["HF_HOME"] = MODELS_DIR
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
# WHISPERX SETTINGS
# ---------------------------------------------------

device = "cpu"

MODEL_NAME = "large-v3"

print("Loading WhisperX model...")

model = whisperx.load_model(
    MODEL_NAME,
    device,
    compute_type="int8")

# ---------------------------------------------------
# LOAD AUDIO
# ---------------------------------------------------

print("Loading audio...")

audio = whisperx.load_audio(
    input_audio)

# ---------------------------------------------------
# TRANSCRIBE
# ---------------------------------------------------

print("Transcribing audio...")

result = model.transcribe(
    audio)

# ---------------------------------------------------
# ALIGNMENT MODEL
# ---------------------------------------------------

print("Loading alignment model...")

model_a, metadata = whisperx.load_align_model(
    language_code=result["language"],
    device=device)

# ---------------------------------------------------
# ALIGN TIMESTAMPS
# ---------------------------------------------------

print("Aligning timestamps...")

result = whisperx.align(
    result["segments"],
    model_a,
    metadata,
    audio,
    device)

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

    phrases.append({
        "phraseId":
            f"p{len(phrases) + 1}",

        "start":
            current_phrase[0]["start"],

        "end":
            current_phrase[-1]["end"],

        "text":
            " ".join(
                w["word"]
                for w in current_phrase)
    })

    current_phrase = []


for i, word in enumerate(word_segments):

    current_phrase.append(word)

    should_break = False

    # Break on punctuation
    if word["word"].endswith(
        (".", "!", "?", "।")):

        should_break = True

    # Break on speech pause
    if i < len(word_segments) - 1:

        next_word = word_segments[i + 1]

        pause = (
            next_word["start"] -
            word["end"])

        if pause > 0.6:
            should_break = True

    # Break on phrase size
    if len(current_phrase) >= 12:
        should_break = True

    if should_break:
        flush_phrase()

flush_phrase()

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