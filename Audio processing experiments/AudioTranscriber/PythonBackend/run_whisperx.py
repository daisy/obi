import whisperx
import json
import sys
import os

if len(sys.argv) < 3:
    print("Usage: python run_whisperx.py input.wav output.json")
    sys.exit(1)

input_audio = sys.argv[1]
output_json = sys.argv[2]

device = "cpu"

print("Loading WhisperX model...")

model = whisperx.load_model(
    "large-v3",
    device,
    compute_type="int8"
)

print("Loading audio...")

audio = whisperx.load_audio(input_audio)

print("Transcribing audio...")

result = model.transcribe(audio)

print("Loading alignment model...")

model_a, metadata = whisperx.load_align_model(
    language_code=result["language"],
    device=device
)

print("Aligning timestamps...")

result = whisperx.align(
    result["segments"],
    model_a,
    metadata,
    audio,
    device
)

word_segments = result.get("word_segments", [])

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

    # punctuation
    if word["word"].endswith((".", "!", "?", "।")):
        should_break = True

    # pause detection
    if i < len(word_segments) - 1:

        next_word = word_segments[i + 1]

        pause = (
            next_word["start"] -
            word["end"]
        )

        if pause > 0.6:
            should_break = True

    # readability limit
    if len(current_phrase) >= 12:
        should_break = True

    if should_break:
        flush_phrase()

flush_phrase()

print("Saving JSON...")

output = {
    "phrases": phrases
}

with open(output_json, "w", encoding="utf-8") as f:
    json.dump(output, f, ensure_ascii=False, indent=2)

print("Completed")