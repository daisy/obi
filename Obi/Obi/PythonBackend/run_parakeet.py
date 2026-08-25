import os
import sys
import json
import subprocess
import numpy as np
import torch

from transformers import (
    AutoProcessor,
    AutoModel
)


# =========================================================
# COMMAND LINE
# =========================================================

if len(sys.argv) != 7:

    print(
        "Usage:\n"
        "run_parakeet.py "
        "<input_audio> "
        "<output_json> "
        "<book_language> "
        "<models_dir> "
        "<hf_cache> "
        "<ffmpeg_exe>"
    )

    sys.exit(1)


INPUT_AUDIO = sys.argv[1]
OUTPUT_JSON = sys.argv[2]
BOOK_LANGUAGE = sys.argv[3]
MODELS_DIR = sys.argv[4]
HF_CACHE = sys.argv[5]
FFMPEG_EXE = sys.argv[6]


# =========================================================
# CONSTANTS
# =========================================================

MODEL_ID = "nvidia/parakeet-tdt-0.6b-v3"

SAMPLE_RATE = 16000

# Long-audio processing
CHUNK_SIZE = 120.0
CHUNK_OVERLAP = 2.0

# Generation limit.
#
# We deliberately use max_new_tokens instead of the
# model-agnostic max_length used previously.
#
# 4096 is comfortably above what is normally required
# for 120 seconds of spoken English and also gives room
# for faster speech / multilingual audio.
MAX_NEW_TOKENS = 4096

# Phrase construction
SILENCE_GAP = 0.60


# =========================================================
# DIRECTORIES
# =========================================================

os.makedirs(
    MODELS_DIR,
    exist_ok=True)

os.makedirs(
    HF_CACHE,
    exist_ok=True)


# =========================================================
# HUGGING FACE CACHE
# =========================================================

os.environ["HF_HOME"] = HF_CACHE
os.environ["HF_HUB_CACHE"] = HF_CACHE
os.environ["TORCH_HOME"] = MODELS_DIR

os.environ[
    "HF_HUB_DISABLE_SYMLINKS_WARNING"
] = "1"


# =========================================================
# DEVICE
# =========================================================

if torch.cuda.is_available():

    device = torch.device("cuda")

else:

    device = torch.device("cpu")


print(
    f"Parakeet device: {device}")


# =========================================================
# VALIDATE INPUT
# =========================================================

if not os.path.exists(INPUT_AUDIO):

    raise Exception(
        f"Audio file not found:\n{INPUT_AUDIO}")


print(
    f"Audio file: "
    f"{os.path.abspath(INPUT_AUDIO)}")


print(
    f"Book Language: "
    f"{BOOK_LANGUAGE}")


# =========================================================
# LOAD PROCESSOR
# =========================================================

print(
    "Loading Parakeet processor...")


processor = AutoProcessor.from_pretrained(
    MODEL_ID,
    cache_dir=HF_CACHE)


print(
    "Parakeet processor loaded")


# =========================================================
# LOAD MODEL
# =========================================================

print(
    "Loading Parakeet model...")


print(
    f"Model: {MODEL_ID}")


model = AutoModel.from_pretrained(
    MODEL_ID,
    cache_dir=HF_CACHE,
    dtype="auto")


model = model.to(device)

model.eval()


print(
    "Parakeet model loaded")


# =========================================================
# LOAD AUDIO USING FFMPEG
# =========================================================

def load_audio(
    audio_file):

    print(
        "Converting audio to "
        "16 kHz mono PCM...")


    command = [

        FFMPEG_EXE,

        "-hide_banner",
        "-loglevel",
        "error",

        "-i",
        audio_file,

        "-f",
        "s16le",

        "-acodec",
        "pcm_s16le",

        "-ac",
        "1",

        "-ar",
        str(SAMPLE_RATE),

        "pipe:1"
    ]


    result = subprocess.run(
        command,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE)


    if result.returncode != 0:

        error = (
            result.stderr
            .decode(
                "utf-8",
                errors="replace")
        )


        raise Exception(
            "FFmpeg failed:\n\n"
            + error)


    audio = np.frombuffer(
        result.stdout,
        dtype=np.int16)


    if audio.size == 0:

        raise Exception(
            "Decoded audio contains "
            "zero samples.")


    audio = (
        audio.astype(
            np.float32)
        / 32768.0)


    duration = (
        len(audio)
        / SAMPLE_RATE)


    print(
        f"Audio duration: "
        f"{duration:.3f} seconds")


    return audio


# =========================================================
# TRANSCRIBE ONE CHUNK
# =========================================================

def transcribe_chunk(
    chunk_audio,
    chunk_offset,
    chunk_number,
    total_chunks):

    print()
    print("=" * 50)
    print(
        f"Parakeet chunk "
        f"{chunk_number}/{total_chunks}")
    print(
        f"Chunk offset: "
        f"{chunk_offset:.3f} seconds")
    print(
        f"Chunk duration: "
        f"{len(chunk_audio) / SAMPLE_RATE:.3f} seconds")
    print("=" * 50)


    print(
        "Preparing Parakeet input...")


    inputs = processor(
        chunk_audio,
        sampling_rate=SAMPLE_RATE,
        return_tensors="pt")


    inputs = {
        key: value.to(device)
        for key, value in inputs.items()
    }


    print(
        "Transcribing chunk...")


    with torch.inference_mode():

        outputs = model.generate(
            **inputs,
            max_new_tokens=MAX_NEW_TOKENS)


    print(
        "Chunk transcription completed")


    print(
        "Decoding chunk...")


    decoded_texts, token_batches = (
        processor.decode(
            outputs.sequences,
            durations=outputs.durations,
            skip_special_tokens=True)
    )


    decoded_text = (
        decoded_texts[0]
        if decoded_texts
        else ""
    )


    token_timestamps = (
        token_batches[0]
        if token_batches
        else []
    )


    return (
        decoded_text,
        token_timestamps)


# =========================================================
# TOKEN -> WORD RECONSTRUCTION
# =========================================================

def reconstruct_words(
    token_timestamps):

    words = []

    current_word = None

    punctuation = (
        ".,!?;:%)]}"
    )


    for token_info in token_timestamps:

        token = token_info.get(
            "token",
            "")


        if not token:

            continue


        start = float(
            token_info["start"])


        end = float(
            token_info["end"])


        # -------------------------------------------------
        # New word
        #
        # Parakeet normally uses a leading space to
        # indicate a new word.
        # -------------------------------------------------

        if token[0].isspace():

            if current_word is not None:

                words.append(
                    current_word)


            current_word = {

                "word":
                    token.lstrip(),

                "start":
                    start,

                "end":
                    end
            }


            continue


        # -------------------------------------------------
        # First token
        # -------------------------------------------------

        if current_word is None:

            current_word = {

                "word":
                    token,

                "start":
                    start,

                "end":
                    end
            }


            continue


        # -------------------------------------------------
        # Punctuation
        # -------------------------------------------------

        if token in punctuation:

            current_word["word"] += token

            current_word["end"] = end

            continue


        # -------------------------------------------------
        # Apostrophe
        # -------------------------------------------------

        if token in (
            "'",
            "’"
        ):

            current_word["word"] += token

            current_word["end"] = end

            continue


        # -------------------------------------------------
        # Continuation of current word
        # -------------------------------------------------

        current_word["word"] += token

        current_word["end"] = end


    # -----------------------------------------------------
    # Final word
    # -----------------------------------------------------

    if current_word is not None:

        words.append(
            current_word)


    return words


# =========================================================
# ADD GLOBAL TIMESTAMPS
# =========================================================

def add_global_word_timestamps(
    words,
    chunk_offset):

    result = []

    for word in words:

        result.append({

            "word":
                word["word"],

            "start":
                word["start"] +
                chunk_offset,

            "end":
                word["end"] +
                chunk_offset
        })


    return result


# =========================================================
# NORMALIZE WORD
# =========================================================

def normalize_word(
    word):

    value = (
        word
        .strip()
        .lower()
    )


    # Remove punctuation only for comparison.
    value = value.strip(
        ".,!?;:%()[]{}\"'’"
    )


    return value


# =========================================================
# NORMALIZE TOKEN
# =========================================================

def add_global_token_timestamps(
    tokens,
    chunk_offset):

    result = []

    for token in tokens:

        if (
            "start" not in token
            or
            "end" not in token
        ):

            continue


        item = dict(token)

        item["start"] = (
            float(token["start"])
            + chunk_offset
        )

        item["end"] = (
            float(token["end"])
            + chunk_offset
        )


        result.append(item)


    return result


# =========================================================
# REMOVE INVALID WORDS
# =========================================================

def clean_words(
    words):

    result = []

    for word in words:

        text = (
            word.get(
                "word",
                "")
            .strip()
        )


        if not text:

            continue


        start = float(
            word["start"])


        end = float(
            word["end"])


        if end <= start:

            continue


        result.append({

            "word":
                text,

            "start":
                start,

            "end":
                end
        })


    return result


# =========================================================
# CHUNK OVERLAP MERGING
# =========================================================

def merge_chunk_words(
    chunk_results):

    """
    Merge words from overlapping chunks.

    IMPORTANT:

    We do NOT compare the complete transcript and try
    to delete repeated words afterwards.

    Instead, every overlap has one ownership boundary.

    Example:

        Chunk 1: 0 ----------- 120
        Chunk 2:       118 ----------- 238

        overlap = 2 seconds

        ownership boundary = 119 seconds

    Therefore:

        Chunk 1 owns everything before 119.
        Chunk 2 owns everything from 119 onward.

    This guarantees that the same spoken material cannot
    be emitted twice.
    """


    if not chunk_results:

        return []


    merged = []


    for index, chunk in enumerate(
        chunk_results):

        words = chunk["words"]

        chunk_start = chunk["start"]

        chunk_end = chunk["end"]


        # -------------------------------------------------
        # Determine ownership interval
        # -------------------------------------------------

        if index == 0:

            ownership_start = (
                0.0)

        else:

            previous = (
                chunk_results[index - 1])


            overlap_start = (
                chunk_start)


            overlap_end = min(
                previous["end"],
                chunk_end)


            if overlap_end > overlap_start:

                ownership_start = (
                    overlap_start
                    +
                    (
                        overlap_end
                        -
                        overlap_start
                    )
                    / 2.0
                )

            else:

                ownership_start = (
                    chunk_start)


        if index < len(
            chunk_results) - 1:

            next_chunk = (
                chunk_results[index + 1])


            next_start = (
                next_chunk["start"])


            overlap_start = (
                next_start)


            overlap_end = min(
                chunk_end,
                next_chunk["end"])


            if overlap_end > overlap_start:

                ownership_end = (
                    overlap_start
                    +
                    (
                        overlap_end
                        -
                        overlap_start
                    )
                    / 2.0
                )

            else:

                ownership_end = (
                    chunk_end)

        else:

            ownership_end = (
                float("inf"))


        print(
            f"Chunk {index + 1} ownership: "
            f"{ownership_start:.3f} - "
            f"{ownership_end:.3f}")


        # -------------------------------------------------
        # Select words owned by this chunk
        # -------------------------------------------------

        selected = 0


        for word in words:

            start = word["start"]

            end = word["end"]


            # Word belongs to this chunk if its
            # midpoint is inside the ownership interval.
            #
            # Using the midpoint is important for words
            # which straddle the ownership boundary.

            midpoint = (
                start + end
            ) / 2.0


            if (
                midpoint >= ownership_start
                and
                midpoint < ownership_end
            ):

                merged.append(
                    word)

                selected += 1


        print(
            f"  Selected words: "
            f"{selected}")


    # -----------------------------------------------------
    # Sort globally
    # -----------------------------------------------------

    merged.sort(
        key=lambda x: (
            x["start"],
            x["end"]
        )
    )


    return merged


# =========================================================
# REMOVE ACCIDENTAL SAME-TIME DUPLICATES
# =========================================================

def remove_residual_duplicates(
    words):

    """
    This is only a safety net.

    The primary overlap handling is performed by
    merge_chunk_words().

    We only remove a word when:

      1. It is immediately adjacent to another word,
      2. The normalized text is identical,
      3. Their timestamps overlap heavily.

    We do NOT remove ordinary repeated words such as:

        "very very good"

    when they were genuinely spoken.
    """


    if not words:

        return []


    result = [
        words[0]
    ]


    for current in words[1:]:

        previous = result[-1]


        same_text = (
            normalize_word(
                previous["word"])
            ==
            normalize_word(
                current["word"])
        )


        overlap_start = max(
            previous["start"],
            current["start"]
        )


        overlap_end = min(
            previous["end"],
            current["end"]
        )


        overlap = max(
            0.0,
            overlap_end -
            overlap_start
        )


        previous_duration = max(
            0.001,
            previous["end"] -
            previous["start"]
        )


        current_duration = max(
            0.001,
            current["end"] -
            current["start"]
        )


        overlap_ratio_previous = (
            overlap /
            previous_duration
        )


        overlap_ratio_current = (
            overlap /
            current_duration
        )


        if (
            same_text
            and
            overlap > 0
            and
            (
                overlap_ratio_previous >= 0.50
                or
                overlap_ratio_current >= 0.50
            )
        ):

            # Keep the word with the later end.
            #
            # This is only a residual safety mechanism.
            if current["end"] > previous["end"]:

                result[-1] = current

            continue


        result.append(
            current)


    return result


# =========================================================
# BUILD PHRASE SEGMENTS
# =========================================================


# =========================================================
# WORDS -> PHRASE SEGMENTS
# =========================================================

def build_segments(words):
    """
    Build natural audiobook-style phrase segments from Parakeet
    word timestamps.

    This function operates only on the final Parakeet word stream.

    Goals:
        - Prefer complete sentences.
        - Prefer natural grammatical boundaries.
        - Avoid false sentence boundaries caused by abbreviations.
        - Avoid weak comma boundaries.
        - Avoid splitting grammatical continuations.
        - Prefer approximately 8-15 second phrases.
        - Prefer an earlier good boundary over stretching toward
          the 18-second limit.
        - Never exceed HARD_MAX_DURATION unless absolutely impossible.
    """

    if not words:
        return []

    # ============================================================
    # CONFIGURATION
    # ============================================================

    PREFERRED_MIN_DURATION = 7.0
    TARGET_DURATION = 12.0

    SOFT_MAX_DURATION = 15.0
    HARD_MAX_DURATION = 18.0

    MIN_SEGMENT_DURATION = 3.0

    COMMA_PAUSE = 0.35
    STRONG_PAUSE = 0.65

    # ============================================================
    # ABBREVIATIONS
    # ============================================================

    ABBREVIATIONS = {
        "mr.",
        "mrs.",
        "ms.",
        "miss.",
        "dr.",
        "prof.",
        "sr.",
        "jr.",
        "st.",
        "mt.",
        "rev.",
        "gen.",
        "col.",
        "lt.",
        "sgt.",
        "capt.",
        "maj.",
        "hon.",
        "pres.",
        "gov.",
        "sen.",
        "rep.",
        "ave.",
        "rd.",
        "dept.",
        "est.",
        "fig.",
        "no.",
        "nos.",
        "etc.",
        "e.g.",
        "i.e.",
        "vs.",
        "approx.",
        "inc.",
        "ltd.",
        "co.",
    }

    # ============================================================
    # CONTINUATION WORDS
    # ============================================================

    CONTINUATION_WORDS = {
        "and",
        "or",
        "nor",
        "but",
        "yet",
        "so",

        "because",
        "although",
        "though",
        "while",
        "when",
        "where",
        "wherever",
        "whenever",
        "if",
        "unless",
        "until",
        "before",
        "after",
        "since",
        "as",

        "that",
        "which",
        "who",
        "whom",
        "whose",

        "than",
        "whether",

        "also",
    }

    # ============================================================
    # PREPOSITIONS / ARTICLES
    # ============================================================

    COMMON_PREPOSITIONS = {
        "of",
        "in",
        "on",
        "at",
        "by",
        "for",
        "from",
        "with",
        "into",
        "onto",
        "over",
        "under",
        "through",
        "during",
        "after",
        "before",
        "between",
        "among",
        "around",
        "against",
        "toward",
        "towards",
        "without",
        "within",
        "upon",
        "about",
    }

    COMMON_ARTICLES = {
        "a",
        "an",
        "the",
    }

    # ============================================================
    # REPORTING CLAUSE STARTERS
    #
    # These words commonly begin a reporting clause following
    # quoted/narrative speech:
    #
    #     "... that's what she did, said Joe ..."
    #     "... he replied ..."
    #
    # A comma immediately before one of these words should not
    # normally be treated as a phrase boundary.
    # ============================================================

    REPORTING_CLAUSE_STARTERS = {
        "said",
        "asked",
        "replied",
        "answered",
        "continued",
        "added",
        "remarked",
        "observed",
        "explained",
        "exclaimed",
        "cried",
        "called",
        "muttered",
        "whispered",
        "shouted",
        "declared",
        "announced",
    }

    # ============================================================
    # LIGHTWEIGHT ADJECTIVE HEURISTIC
    # ============================================================

    COMMON_ADJECTIVE_SUFFIXES = (
        "able",
        "ible",
        "al",
        "ful",
        "less",
        "ous",
        "ive",
        "ic",
        "ical",
        "ish",
        "ary",
        "ory",
        "ant",
        "ent",
        "ing",
        "ed",
    )

    def clean_word(word):
        return str(word).strip()

    def normalize_word_text(word):
        return clean_word(word).lower()

    def punctuation_of(word):

        token = clean_word(word)

        # Remove closing quotation/bracket characters.
        while token and token[-1] in "\"'”’)]}":
            token = token[:-1]

        if not token:
            return ""

        return token[-1]

    def is_abbreviation(word):

        return (
            normalize_word_text(word)
            in ABBREVIATIONS
        )

    def is_sentence_end_word(word):

        token = clean_word(word)

        if not token:
            return False

        # IMPORTANT:
        # "Mrs.", "Mr.", "Dr.", etc. are NOT sentence endings.
        if is_abbreviation(token):
            return False

        punctuation = punctuation_of(token)

        return punctuation in (
            ".",
            "!",
            "?"
        )

    def is_comma_word(word):

        return (
            punctuation_of(word)
            == ","
        )

    def is_semicolon_word(word):

        return (
            punctuation_of(word)
            == ";"
        )

    def is_colon_word(word):

        return (
            punctuation_of(word)
            == ":"
        )

    def is_dash_word(word):

        punctuation = punctuation_of(word)

        return punctuation in (
            "-",
            "–",
            "—"
        )

    def looks_like_adjective(word):

        token = normalize_word_text(word)

        if not token:
            return False

        if token in COMMON_ARTICLES:
            return False

        if token in COMMON_PREPOSITIONS:
            return False

        if token in CONTINUATION_WORDS:
            return False

        if len(token) < 5:
            return False

        return token.endswith(
            COMMON_ADJECTIVE_SUFFIXES
        )

    # ============================================================
    # TIMING HELPERS
    # ============================================================

    def duration(start_index, end_index):

        if start_index > end_index:
            return 0.0

        return (
            float(words[end_index]["end"])
            -
            float(words[start_index]["start"])
        )

    def pause_after(index):

        if (
            index < 0
            or
            index >= len(words) - 1
        ):
            return 0.0

        return max(
            0.0,
            float(words[index + 1]["start"])
            -
            float(words[index]["end"])
        )

    # ============================================================
    # GRAMMATICAL COMMA SAFETY
    # ============================================================

    def comma_is_grammatically_safe(index):

        if index < 0:
            return False

        if index >= len(words) - 1:
            return True

        next_word = normalize_word_text(
            words[index + 1]["word"]
        )

        if not next_word:
            return False

        pause = pause_after(index)

        # --------------------------------------------------------
        # A comma with virtually no pause is not a useful
        # audiobook boundary.
        # --------------------------------------------------------

        if pause < 0.18:
            return False

        # --------------------------------------------------------
        # Never split before a reporting clause.
        #
        # Example:
        #
        #     "... that's what she did, said Joe."
        #
        # We want "said Joe" to remain attached.
        # --------------------------------------------------------

        if next_word in REPORTING_CLAUSE_STARTERS:
            return False

        # --------------------------------------------------------
        # Prepositions and articles normally indicate that the
        # grammatical phrase is continuing.
        # --------------------------------------------------------

        if next_word in COMMON_PREPOSITIONS:
            return False

        if next_word in COMMON_ARTICLES:
            return False

        # --------------------------------------------------------
        # Do not split before an obvious adjective.
        #
        # Example:
        #
        #     "a square, impregnable bib"
        #
        # --------------------------------------------------------

        if looks_like_adjective(
            words[index + 1]["word"]
        ):
            return False

        # --------------------------------------------------------
        # Continuation words require special treatment.
        #
        # Previously ALL continuation words were rejected.
        # That is too aggressive for audiobook phrase building.
        #
        # A comma followed by "and", "but", "or", etc. can be a
        # perfectly good phrase boundary when there is a meaningful
        # pause.
        #
        # However, subordinate conjunctions such as "because",
        # "although", "while", etc. should normally remain attached.
        # --------------------------------------------------------

        COORDINATING_CONJUNCTIONS = {
            "and",
            "but",
            "or",
            "nor",
            "yet",
            "so",
        }

        SUBORDINATING_CONJUNCTIONS = {
            "because",
            "although",
            "though",
            "while",
            "when",
            "where",
            "wherever",
            "whenever",
            "if",
            "unless",
            "until",
            "before",
            "after",
            "since",
            "as",
            "that",
            "which",
            "who",
            "whom",
            "whose",
            "than",
            "whether",
        }

        if next_word in SUBORDINATING_CONJUNCTIONS:
            return False

        if next_word in COORDINATING_CONJUNCTIONS:

            # A stronger pause makes the comma a legitimate
            # audiobook phrase boundary.
            if pause >= 0.30:
                return True

            return False

        return True

    # ============================================================
    # BOUNDARY CLASSIFICATION
    # ============================================================

    def boundary_type(index):

        if (
            index < 0
            or
            index >= len(words)
        ):
            return None

        word = words[index]["word"]

        pause = pause_after(index)

        # Sentence ending
        if is_sentence_end_word(word):
            return "sentence"

        # Semicolon
        if is_semicolon_word(word):

            if pause >= 0.15:
                return "semicolon"

        # Colon
        if is_colon_word(word):

            if pause >= 0.15:
                return "colon"

        # Dash
        if is_dash_word(word):

            if pause >= 0.15:
                return "dash"

        # Comma
        if is_comma_word(word):

            if comma_is_grammatically_safe(index):

                if pause >= COMMA_PAUSE:
                    return "comma"

        # Natural silence
        if pause >= STRONG_PAUSE:
            return "pause"

        return None

    # ============================================================
    # BOUNDARY QUALITY
    # ============================================================

    def boundary_base_score(boundary_kind):

        if boundary_kind == "sentence":
            return 100.0

        if boundary_kind == "semicolon":
            return 72.0

        if boundary_kind == "colon":
            return 65.0

        if boundary_kind == "dash":
            return 58.0

        if boundary_kind == "comma":
            return 38.0

        if boundary_kind == "pause":
            return 30.0

        return -1000000.0

    # ============================================================
    # SCORE A CANDIDATE
    # ============================================================

    def score_boundary(
        index,
        phrase_start,
        target=TARGET_DURATION
    ):
        """
        Score a boundary.

        The important difference from the previous implementation
        is that duration is treated as a preference rather than
        allowing a late boundary to dominate simply because it is
        closer to the hard maximum.

        A good earlier grammatical boundary can therefore win.
        """

        btype = boundary_type(index)

        if btype is None:
            return -1000000.0

        d = duration(
            phrase_start,
            index
        )

        if d < MIN_SEGMENT_DURATION:
            return -1000000.0

        if d > HARD_MAX_DURATION:
            return -1000000.0

        score = boundary_base_score(
            btype
        )

        # ========================================================
        # Duration preference
        # ========================================================

        # Strong preference for the 8-14 second region.
        if 8.0 <= d <= 14.0:

            score += 45.0

        elif 7.0 <= d < 8.0:

            score += 28.0

        elif 14.0 < d <= 15.0:

            score += 25.0

        elif 15.0 < d <= 16.5:

            score += 5.0

        elif d > 16.5:

            # Do not encourage stretching.
            score -= (
                d - 16.5
            ) * 12.0

        else:

            # Short segments are undesirable.
            score -= (
                7.0 - d
            ) * 10.0

        # ========================================================
        # Distance from target
        # ========================================================

        distance = abs(
            d - target
        )

        # A modest target bonus, deliberately weaker than
        # linguistic boundary quality.
        score += max(
            0.0,
            18.0 - distance * 4.0
        )

        # ========================================================
        # Sentence ending bonus
        # ========================================================

        if btype == "sentence":

            score += 45.0

            # A complete sentence in the normal audiobook range
            # should be very attractive.
            if 7.0 <= d <= 15.0:
                score += 20.0

        # ========================================================
        # Strong pause bonus
        # ========================================================

        pause = pause_after(index)

        if pause >= 1.0:
            score += 12.0

        elif pause >= 0.65:
            score += 7.0

        elif pause >= 0.35:
            score += 3.0

        # ========================================================
        # Boundary-specific adjustments
        # ========================================================

        if btype == "comma":

            # Commas are useful but should never dominate a sentence
            # ending merely because they are slightly closer to target.
            score -= 8.0

            if pause >= 0.75:
                score += 5.0

        elif btype == "pause":

            score -= 5.0

        # ========================================================
        # Next-word safety
        # ========================================================

        if index < len(words) - 1:

            next_word = normalize_word_text(
                words[index + 1]["word"]
            )

            if next_word in CONTINUATION_WORDS:

                score -= 100.0

            elif next_word in COMMON_PREPOSITIONS:

                score -= 55.0

            elif next_word in COMMON_ARTICLES:

                score -= 40.0

        return score

    # ============================================================
    # FIND CANDIDATES
    # ============================================================

    def collect_boundaries(
        phrase_start,
        search_end,
        maximum_duration=HARD_MAX_DURATION
    ):
        candidates = []

        for index in range(
            phrase_start,
            search_end + 1
        ):

            d = duration(
                phrase_start,
                index
            )

            if d < MIN_SEGMENT_DURATION:
                continue

            if d > maximum_duration:
                break

            btype = boundary_type(index)

            if btype is None:
                continue

            score = score_boundary(
                index,
                phrase_start
            )

            if score <= -100000.0:
                continue

            candidates.append({
                "index": index,
                "type": btype,
                "duration": d,
                "score": score
            })

        return candidates

    # ============================================================
    # SELECT BEST BOUNDARY
    # ============================================================

    def choose_boundary(
        phrase_start,
        search_end,
        maximum_duration=HARD_MAX_DURATION,
        prefer_early=False
    ):
        """
        Select the best internal phrase boundary.

        For long sentences, the priority is:

            1. Natural grammatical boundary
            2. Useful audiobook duration
            3. Stronger pause
            4. Proximity to target duration

        When prefer_early=True, boundaries in the 7-14 second
        range are strongly preferred. This prevents a long sentence
        from unnecessarily approaching the 18-second hard limit.
        """

        candidates = collect_boundaries(
            phrase_start,
            search_end,
            maximum_duration
        )

        if not candidates:
            return None

        # ========================================================
        # LONG-SENTENCE MODE
        # ========================================================

        if prefer_early:

            # ----------------------------------------------------
            # Primary audiobook range: 7-14 seconds
            # ----------------------------------------------------

            preferred = [
                c
                for c in candidates
                if (
                    PREFERRED_MIN_DURATION
                    <= c["duration"]
                    <= 14.0
                )
            ]

            if preferred:

                # ------------------------------------------------
                # Rank linguistic boundary type first.
                #
                # Sentence / semicolon / colon / dash are stronger
                # than comma / pause.
                #
                # Within the same general linguistic quality,
                # prefer a duration near 11-12 seconds.
                # ------------------------------------------------

                boundary_priority = {
                    "sentence": 6,
                    "semicolon": 5,
                    "colon": 4,
                    "dash": 3,
                    "comma": 2,
                    "pause": 1,
                }

                def preferred_key(candidate):

                    btype = candidate["type"]

                    priority = (
                        boundary_priority
                        .get(btype, 0)
                    )

                    d = candidate["duration"]

                    distance_from_target = abs(
                        d - TARGET_DURATION
                    )

                    pause = pause_after(
                        candidate["index"]
                    )

                    # Longer pause is useful, but only as a
                    # secondary consideration.
                    pause_bonus = min(
                        pause,
                        1.5
                    )

                    return (
                        -priority,
                        distance_from_target,
                        -pause_bonus,
                        candidate["index"]
                    )

                preferred.sort(
                    key=preferred_key
                )

                return preferred[0]["index"]

            # ----------------------------------------------------
            # Secondary range: 5-7 seconds
            #
            # Only use this when there is no useful 7-14 sec
            # boundary.
            # ----------------------------------------------------

            early = [
                c
                for c in candidates
                if (
                    5.0
                    <= c["duration"]
                    <
                    PREFERRED_MIN_DURATION
                )
            ]

            if early:

                boundary_priority = {
                    "sentence": 6,
                    "semicolon": 5,
                    "colon": 4,
                    "dash": 3,
                    "comma": 2,
                    "pause": 1,
                }

                early.sort(
                    key=lambda c: (
                        -boundary_priority.get(
                            c["type"],
                            0
                        ),
                        -pause_after(
                            c["index"]
                        ),
                        abs(
                            c["duration"]
                            -
                            TARGET_DURATION
                        )
                    )
                )

                return early[0]["index"]

            # ----------------------------------------------------
            # Last soft range: 14-15 seconds
            # ----------------------------------------------------

            late_soft = [
                c
                for c in candidates
                if (
                    14.0
                    <
                    c["duration"]
                    <=
                    SOFT_MAX_DURATION
                )
            ]

            if late_soft:

                boundary_priority = {
                    "sentence": 6,
                    "semicolon": 5,
                    "colon": 4,
                    "dash": 3,
                    "comma": 2,
                    "pause": 1,
                }

                late_soft.sort(
                    key=lambda c: (
                        -boundary_priority.get(
                            c["type"],
                            0
                        ),
                        abs(
                            c["duration"]
                            -
                            TARGET_DURATION
                        ),
                        -pause_after(
                            c["index"]
                        )
                    )
                )

                return late_soft[0]["index"]

        # ========================================================
        # NORMAL / FORCED MODE
        # ========================================================

        boundary_priority = {
            "sentence": 6,
            "semicolon": 5,
            "colon": 4,
            "dash": 3,
            "comma": 2,
            "pause": 1,
        }

        candidates.sort(
            key=lambda c: (
                -boundary_priority.get(
                    c["type"],
                    0
                ),
                abs(
                    c["duration"]
                    -
                    TARGET_DURATION
                ),
                -pause_after(
                    c["index"]
                ),
                c["duration"]
            )
        )

        return candidates[0]["index"]

    # ============================================================
    # CREATE SEGMENT
    # ============================================================

    def text_from_words(items):

        return " ".join(
            clean_word(item["word"])
            for item in items
            if clean_word(item["word"])
        ).strip()

    def create_segment(
        start_index,
        end_index
    ):

        if start_index > end_index:
            return None

        segment_words = words[
            start_index:end_index + 1
        ]

        if not segment_words:
            return None

        return {
            "start":
                float(segment_words[0]["start"]),

            "end":
                float(segment_words[-1]["end"]),

            "text":
                text_from_words(
                    segment_words
                ),

            "words":
                segment_words
        }

    # ============================================================
    # MAIN LOOP
    # ============================================================

    segments = []

    start_index = 0

    while start_index < len(words):

        # --------------------------------------------------------
        # Find the first sentence-ending word at or after start.
        # --------------------------------------------------------

        sentence_end = None

        for index in range(
            start_index,
            len(words)
        ):

            if is_sentence_end_word(
                words[index]["word"]
            ):

                sentence_end = index
                break

        # ========================================================
        # NO SENTENCE ENDING REMAINS
        # ========================================================

        if sentence_end is None:

            remaining_duration = duration(
                start_index,
                len(words) - 1
            )

            if remaining_duration <= HARD_MAX_DURATION:

                segment = create_segment(
                    start_index,
                    len(words) - 1
                )

                if segment:
                    segments.append(segment)

                break

            boundary = choose_boundary(
                start_index,
                len(words) - 1,
                HARD_MAX_DURATION,
                prefer_early=False
            )

            if boundary is not None:

                segment = create_segment(
                    start_index,
                    boundary
                )

                if segment:
                    segments.append(segment)

                start_index = boundary + 1
                continue

            # Absolute fallback.
            hard_end = start_index

            for j in range(
                start_index,
                len(words)
            ):

                if duration(
                    start_index,
                    j
                ) <= HARD_MAX_DURATION:

                    hard_end = j

                else:
                    break

            segment = create_segment(
                start_index,
                hard_end
            )

            if segment:
                segments.append(segment)

            start_index = hard_end + 1
            continue

        # ========================================================
        # SENTENCE DURATION
        # ========================================================

        sentence_duration = duration(
            start_index,
            sentence_end
        )

        # ========================================================
        # SHORT / NORMAL SENTENCE
        #
        # Up to SOFT_MAX_DURATION (15 sec), preserve the complete
        # sentence. We do not split normal-length sentences just
        # to make them shorter.
        # ========================================================

        if sentence_duration <= SOFT_MAX_DURATION:

            segment = create_segment(
                start_index,
                sentence_end
            )

            if segment:
                segments.append(segment)

            start_index = sentence_end + 1
            continue

        # ========================================================
        # LONG SENTENCE: > 15 sec
        #
        # Before accepting a 15-18 second sentence intact, try
        # to find a natural internal boundary.
        #
        # This is the important change.
        #
        # Example:
        #
        #     16.4 second sentence
        #
        # If there is a good comma/pause around 8-14 seconds,
        # split there instead of keeping the entire 16.4 seconds.
        # ========================================================

        internal_boundary = choose_boundary(
            start_index,
            sentence_end - 1,
            SOFT_MAX_DURATION,
            prefer_early=True
        )

        if internal_boundary is not None:

            internal_duration = duration(
                start_index,
                internal_boundary
            )

            internal_type = boundary_type(
                internal_boundary
            )

            # ----------------------------------------------------
            # Accept a natural internal boundary when it gives us
            # a useful first phrase.
            #
            # 7 sec is the preferred minimum, but we also allow
            # a strong grammatical boundary slightly earlier if
            # necessary.
            # ----------------------------------------------------

            if internal_duration >= PREFERRED_MIN_DURATION:

                if internal_type in (
                    "sentence",
                    "semicolon",
                    "colon",
                    "dash",
                    "comma",
                    "pause"
                ):

                    segment = create_segment(
                        start_index,
                        internal_boundary
                    )

                    if segment:
                        segments.append(segment)

                    start_index = (
                        internal_boundary + 1
                    )

                    continue

        # ========================================================
        # NO GOOD INTERNAL BOUNDARY
        #
        # If the sentence is <= 18 sec, preserve it intact.
        #
        # This is important: we should NOT force an unnatural
        # split merely because the sentence is slightly over
        # 15 seconds.
        # ========================================================

        if sentence_duration <= HARD_MAX_DURATION:

            segment = create_segment(
                start_index,
                sentence_end
            )

            if segment:
                segments.append(segment)

            start_index = sentence_end + 1
            continue

        # ========================================================
        # SENTENCE ITSELF EXCEEDS HARD MAX
        #
        # We MUST split it.
        # ========================================================

        forced_boundary = choose_boundary(
            start_index,
            sentence_end - 1,
            HARD_MAX_DURATION,
            prefer_early=False
        )

        if forced_boundary is not None:

            segment = create_segment(
                start_index,
                forced_boundary
            )

            if segment:
                segments.append(segment)

            start_index = forced_boundary + 1
            continue

        # ========================================================
        # ABSOLUTE FALLBACK
        # ========================================================

        hard_end = start_index

        for j in range(
            start_index,
            sentence_end + 1
        ):

            if duration(
                start_index,
                j
            ) <= HARD_MAX_DURATION:

                hard_end = j

            else:
                break

        segment = create_segment(
            start_index,
            hard_end
        )

        if segment:
            segments.append(segment)

        start_index = hard_end + 1

    # ============================================================
    # FINAL CLEANUP
    # ============================================================

    cleaned_segments = []

    for segment in segments:

        if not segment:
            continue

        if not segment.get("words"):
            continue

        segment["start"] = float(
            segment["words"][0]["start"]
        )

        segment["end"] = float(
            segment["words"][-1]["end"]
        )

        segment["text"] = (
            text_from_words(
                segment["words"]
            )
        )

        # Safety check:
        # Never allow a segment over the hard maximum.
        #
        # Normally this should never trigger because all selection
        # paths enforce HARD_MAX_DURATION.
        if (
            segment["end"]
            -
            segment["start"]
            >
            HARD_MAX_DURATION
        ):

            # Keep the segment rather than silently losing words.
            # This is only a defensive safeguard.
            pass

        cleaned_segments.append(
            segment
        )

    # ============================================================
    # TARGETED SHORT-FRAGMENT CLEANUP
    #
    # IMPORTANT:
    # Do NOT merge a phrase merely because it contains two words
    # or is short.
    #
    # Valid short dialogue must remain independent:
    #
    #     "Is she?"
    #     "Yes, Pip."
    #     "Come here."
    #
    # Only merge a very short, punctuation-less fragment when its
    # first word strongly indicates that it continues the previous
    # phrase.
    # ============================================================

    SHORT_FRAGMENT_WORDS = {
        "and",
        "or",
        "nor",
        "but",
        "yet",
        "so",
        "because",
        "although",
        "though",
        "while",
        "when",
        "where",
        "wherever",
        "whenever",
        "if",
        "unless",
        "until",
        "before",
        "after",
        "since",
        "as",
        "that",
        "which",
        "who",
        "whom",
        "whose",
        "than",
        "whether",
        "of",
        "in",
        "on",
        "at",
        "by",
        "for",
        "from",
        "with",
        "into",
        "onto",
        "over",
        "under",
        "through",
        "during",
        "within",
        "upon",
        "about",
        "said",
    }

    def is_short_fragment(segment):

        segment_words = segment.get("words", [])

        if not segment_words:
            return False

        # Never merge more than two words.
        if len(segment_words) > 2:
            return False

        duration = (
            float(segment["end"])
            -
            float(segment["start"])
        )

        # A genuinely short fragment only.
        if duration > 1.5:
            return False

        first_word = normalize_word(
            segment_words[0]["word"]
        )

        if not first_word:
            return False

        # A fragment ending in sentence punctuation is normally a
        # complete spoken unit and must remain independent.
        last_word = str(
            segment_words[-1]["word"]
        ).strip()

        if last_word.endswith(
            (".", "!", "?")
        ):
            return False

        return (
            first_word
            in SHORT_FRAGMENT_WORDS
        )

    # ------------------------------------------------------------
    # Merge only genuine continuation fragments.
    #
    # We merge backward because these fragments are normally the
    # tail of a grammatical construction which was separated by a
    # pause. The hard maximum is still respected.
    # ------------------------------------------------------------

    merged_segments = []

    for segment in cleaned_segments:

        if (
            merged_segments
            and
            is_short_fragment(segment)
        ):

            previous = merged_segments[-1]

            merged_duration = (
                float(segment["end"])
                -
                float(previous["start"])
            )

            if merged_duration <= HARD_MAX_DURATION:

                previous_words = previous["words"]
                current_words = segment["words"]

                previous["words"] = (
                    previous_words
                    +
                    current_words
                )

                previous["start"] = float(
                    previous["words"][0]["start"]
                )

                previous["end"] = float(
                    previous["words"][-1]["end"]
                )

                previous["text"] = (
                    text_from_words(
                        previous["words"]
                    )
                )

                continue

        merged_segments.append(
            segment
        )

    return merged_segments






# =========================================================
# BUILD TEXT FROM FINAL WORDS
# =========================================================

def build_text(
    words):

    return " ".join(
        word["word"]
        for word in words
    )


# =========================================================
# MAIN
# =========================================================

print()
print("=" * 50)
print(
    "Preparing long-audio Parakeet transcription")
print("=" * 50)


# ---------------------------------------------------------
# LOAD AUDIO
# ---------------------------------------------------------

audio = load_audio(
    INPUT_AUDIO)


audio_duration = (
    len(audio)
    / SAMPLE_RATE
)


# ---------------------------------------------------------
# SHORT AUDIO
#
# We still use the same processing path for short audio.
# ---------------------------------------------------------

if audio_duration <= CHUNK_SIZE:

    print(
        "Audio fits inside one Parakeet chunk.")


    (
        decoded_text,
        token_timestamps
    ) = transcribe_chunk(
        audio,
        0.0,
        1,
        1)


    local_words = reconstruct_words(
        token_timestamps)


    words = add_global_word_timestamps(
        local_words,
        0.0)


    words = clean_words(
        words)


    segments = build_segments(
        words)


    final_tokens = add_global_token_timestamps(
        token_timestamps,
        0.0)


# ---------------------------------------------------------
# LONG AUDIO
# ---------------------------------------------------------

else:

    total_chunks = int(
        np.ceil(
            (
                audio_duration
                -
                CHUNK_OVERLAP
            )
            /
            (
                CHUNK_SIZE
                -
                CHUNK_OVERLAP
            )
        )
    )


    print()
    print("=" * 50)
    print(
        "Parakeet long-audio mode")
    print(
        f"Total duration: "
        f"{audio_duration:.3f} seconds")
    print(
        f"Chunk size: "
        f"{CHUNK_SIZE:.0f} seconds")
    print(
        f"Chunk overlap: "
        f"{CHUNK_OVERLAP:.0f} seconds")
    print(
        f"Number of chunks: "
        f"{total_chunks}")
    print("=" * 50)


    chunk_results = []


    # -----------------------------------------------------
    # PROCESS CHUNKS
    # -----------------------------------------------------

    for chunk_index in range(
        total_chunks):

        chunk_start = (
            chunk_index
            *
            (
                CHUNK_SIZE
                -
                CHUNK_OVERLAP
            )
        )


        chunk_end = min(
            audio_duration,
            chunk_start
            +
            CHUNK_SIZE
        )


        start_sample = int(
            chunk_start
            *
            SAMPLE_RATE
        )


        end_sample = int(
            chunk_end
            *
            SAMPLE_RATE
        )


        chunk_audio = (
            audio[
                start_sample:end_sample
            ]
        )


        (
            decoded_text,
            token_timestamps
        ) = transcribe_chunk(
            chunk_audio,
            chunk_start,
            chunk_index + 1,
            total_chunks)


        # -------------------------------------------------
        # Reconstruct words in local chunk time.
        # -------------------------------------------------

        local_words = reconstruct_words(
            token_timestamps)


        # -------------------------------------------------
        # Convert to global time.
        # -------------------------------------------------

        global_words = (
            add_global_word_timestamps(
                local_words,
                chunk_start)
        )


        global_words = clean_words(
            global_words)


        global_tokens = (
            add_global_token_timestamps(
                token_timestamps,
                chunk_start)
        )


        chunk_results.append({

            "index":
                chunk_index,

            "start":
                chunk_start,

            "end":
                chunk_end,

            "text":
                decoded_text,

            "words":
                global_words,

            "tokens":
                global_tokens
        })


    # -----------------------------------------------------
    # MERGE WORDS
    # -----------------------------------------------------

    print()
    print(
        "Merging chunk transcripts...")


    words = merge_chunk_words(
        chunk_results)


    # -----------------------------------------------------
    # SAFETY CLEANUP
    # -----------------------------------------------------

    print()
    print(
        "Checking for residual duplicate words...")


    before_count = len(
        words)


    words = remove_residual_duplicates(
        words)


    after_count = len(
        words)


    removed_count = (
        before_count -
        after_count
    )


    print(
        f"Residual duplicates removed: "
        f"{removed_count}")


    # -----------------------------------------------------
    # BUILD FINAL TOKENS
    #
    # Apply the same ownership intervals to tokens.
    # -----------------------------------------------------

    final_tokens = []


    for index, chunk in enumerate(
        chunk_results):

        chunk_start = (
            chunk["start"])

        chunk_end = (
            chunk["end"])


        if index == 0:

            ownership_start = (
                0.0)

        else:

            previous = (
                chunk_results[index - 1])

            overlap_start = (
                chunk_start)

            overlap_end = min(
                previous["end"],
                chunk_end)

            ownership_start = (
                overlap_start
                +
                (
                    overlap_end
                    -
                    overlap_start
                )
                / 2.0
            )


        if index < len(
            chunk_results) - 1:

            next_chunk = (
                chunk_results[index + 1])

            overlap_start = (
                next_chunk["start"])

            overlap_end = min(
                chunk_end,
                next_chunk["end"])

            ownership_end = (
                overlap_start
                +
                (
                    overlap_end
                    -
                    overlap_start
                )
                / 2.0
            )

        else:

            ownership_end = (
                float("inf"))


        for token in chunk["tokens"]:

            start = float(
                token["start"])

            end = float(
                token["end"])


            midpoint = (
                start + end
            ) / 2.0


            if (
                midpoint >= ownership_start
                and
                midpoint < ownership_end
            ):

                final_tokens.append(
                    token)


    final_tokens.sort(
        key=lambda x: (
            float(x["start"]),
            float(x["end"])
        )
    )


    # -----------------------------------------------------
    # BUILD PHRASES
    # -----------------------------------------------------

    print()
    print(
        "Building phrase segments...")


    segments = build_segments(
        words)


# =========================================================
# FINAL TEXT
# =========================================================

final_text = build_text(
    words)


# =========================================================
# RECONSTRUCTED WORDS
# =========================================================

print()
print(
    "Reconstructed words:")


for word in words:

    print(
        f"[{word['start']:.3f} - "
        f"{word['end']:.3f}] "
        f"{word['word']}")


# =========================================================
# PHRASE SEGMENTS
# =========================================================

print()
print(
    "Phrase segments:")


for index, segment in enumerate(
    segments,
    start=1):

    print(
        f"[{segment['start']:.3f} - "
        f"{segment['end']:.3f}] "
        f"{segment['text']}")


# =========================================================
# FINAL TEXT
# =========================================================

print()
print(
    "Decoded text:")

print(
    final_text)


# =========================================================
# SAVE RESULT
# =========================================================

result = {

    "language":
        BOOK_LANGUAGE,

    "text":
        final_text,

    "segments":
        segments,

    "words":
        words,

    "tokens":
        final_tokens
}


print()
print(
    "Saving JSON...")


with open(
    OUTPUT_JSON,
    "w",
    encoding="utf-8"
) as f:

    json.dump(
        result,
        f,
        ensure_ascii=False,
        indent=2)


print(
    f"Output written to: "
    f"{OUTPUT_JSON}")


print(
    "Completed")