"""Shared multilingual abbreviation handling for Obi ASR phrase segmentation.

This module is intentionally independent of any ASR engine. WhisperX,
Parakeet, and future transcription backends can pass their aligned word list
and language code to the same helper.
"""

from __future__ import annotations

import re
from typing import Any, Iterable, Mapping

# Abbreviations whose trailing period normally does NOT end a sentence when
# another token follows. Keep this list conservative: forms such as "etc."
# can legitimately occur at the end of a sentence and are intentionally not
# included here.
#
# In addition to dictionary entries, the shared check recognizes standalone
# initials (e.g. "W." in "Theodor W. Adorno") and numeric ordinals written
# with a period (e.g. German "20. Jahrhundert"). These are context-sensitive:
# they are protected only when another token follows.
#
# Entries may be a single token ("mr.") or a multi-token expression
# ("z.", "b."). Language keys use ISO 639-1 codes.
NON_TERMINAL_ABBREVIATIONS: dict[str, set[tuple[str, ...]]] = {
    "en": {
        ("mr.",), ("mrs.",), ("ms.",), ("dr.",), ("prof.",),
        ("rev.",), ("fr.",), ("sr.",), ("jr.",), ("st.",),
        ("mt.",), ("gen.",), ("col.",), ("maj.",), ("capt.",),
        ("cmdr.",), ("lt.",), ("sgt.",), ("hon.",), ("pres.",),
        ("gov.",), ("sen.",), ("rep.",), ("messrs.",),
    },
    "fr": {
        ("m.",), ("mme.",), ("mlle.",), ("mm.",), ("m.",),
        ("dr.",), ("pr.",), ("me.",), ("st.",), ("ste.",),
    },
    "de": {
        ("hr.",), ("fr.",), ("dr.",), ("prof.",), ("ing.",),
        ("dipl.-ing.",), ("bzw.",), ("vgl.",), ("nr.",),
        ("str.",), ("bd.",), ("s.",), ("abb.",), ("kap.",),
        ("z.", "b."), ("u.", "a."), ("d.", "h."),
    },
    "es": {
        ("sr.",), ("sra.",), ("srta.",), ("dr.",), ("dra.",),
        ("ud.",), ("uds.",), ("d.",), ("dña.",), ("d.", "ª"),
        ("núm.",), ("n.º",),
    },
    "it": {
        ("sig.",), ("sig.ra.",), ("sig.na.",), ("dott.",),
        ("dott.ssa.",), ("prof.",), ("avv.",), ("ing.",),
        ("geom.",),
    },
    "pt": {
        ("sr.",), ("sra.",), ("srta.",), ("dr.",), ("dra.",),
        ("prof.",), ("profa.",), ("eng.",), ("exmo.",),
        ("exma.",), ("ilmo.",), ("ilma.",),
    },
    "nl": {
        ("dhr.",), ("mevr.",), ("mw.",), ("dr.",), ("prof.",),
        ("ds.",), ("ir.",), ("mr.",), ("drs.",),
    },
    "ru": {
        ("г.",), ("ул.",), ("д.",), ("стр.",), ("пер.",),
        ("пр-т.",), ("им.",), ("акад.",), ("проф.",),
        ("доц.",), ("т.", "д."), ("т.", "п."), ("т.", "е."),
    },
    # Additional languages supported by Parakeet TDT v3.
    "bg": {
        ("г.",), ("ул.",), ("бул.",), ("проф.",), ("д-р.",),
        ("акад.",), ("т.", "н."),
    },
    "hr": {
        ("g.",), ("ul.",), ("br.",), ("dr.",), ("prof.",),
        ("mr.",), ("npr.",), ("tj.",), ("itd.",),
    },
    "cs": {
        ("p.",), ("pan.",), ("pani.",), ("dr.",), ("prof.",),
        ("ing.",), ("ul.",), ("č.",), ("např.",), ("atd.",),
        ("tj.",), ("tzn.",),
    },
    "da": {
        ("hr.",), ("fru.",), ("dr.",), ("prof.",), ("cand.",),
        ("bl.a.",), ("dvs.",), ("fx.",), ("nr.",), ("osv.",),
    },
    "et": {
        ("hr.",), ("pr.",), ("dr.",), ("prof.",), ("lk.",),
        ("tn.",), ("nt.",), ("st.",), ("jne.",), ("vms.",),
    },
    "fi": {
        ("k.",), ("t.",), ("p.",), ("prof.",), ("tri.",),
        ("toht.",), ("esim.",), ("jne.",), ("ym.",), ("n:o.",),
    },
    "el": {
        ("κ.",), ("π.",), ("δρ.",), ("καθ.",), ("αρ.",),
        ("π.χ.",), ("κ.ά.",), ("κ.λπ.",), ("δηλ.",),
    },
    "hu": {
        ("dr.",), ("ifj.",), ("id.",), ("prof.",), ("u.",),
        ("sz.",), ("pl.",), ("stb.",), ("ill.",), ("kb.",),
    },
    "lv": {
        ("g.",), ("iel.",), ("lpp.",), ("nr.",), ("prof.",),
        ("dr.",), ("š.g.",), ("u.c.",), ("piem.",),
    },
    "lt": {
        ("g.",), ("a.",), ("m.",), ("p.",), ("nr.",),
        ("prof.",), ("dr.",), ("pvz.",), ("t. y.",), ("ir t. t.",),
    },
    "mt": {
        ("dr.",), ("prof.",), ("mr.",), ("mrs.",), ("nru.",),
        ("eż.",), ("ċċ.",),
    },
    "pl": {
        ("pan.",), ("pani.",), ("dr.",), ("prof.",), ("inż.",),
        ("ul.",), ("al.",), ("św.",), ("np.",), ("itd.",),
        ("tj.",), ("tzn.",), ("r.",), ("nr.",),
    },
    "ro": {
        ("dl.",), ("dna.",), ("dr.",), ("prof.",), ("ing.",),
        ("str.",), ("nr.",), ("ș.a.",), ("etc.",),
    },
    "sk": {
        ("p.",), ("pani.",), ("dr.",), ("prof.",), ("ing.",),
        ("ul.",), ("č.",), ("napr.",), ("atď.",), ("t. j.",),
    },
    "sl": {
        ("g.",), ("ga.",), ("dr.",), ("prof.",), ("mag.",),
        ("ul.",), ("št.",), ("npr.",), ("itd.",), ("tj.",),
    },
    "sv": {
        ("dr.",), ("prof.",), ("hr.",), ("fru.",), ("t.ex.",),
        ("bl.a.",), ("m.m.",), ("d.v.s.",), ("nr.",),
    },
    "uk": {
        ("м.",), ("вул.",), ("р.",), ("ст.",), ("ім.",),
        ("проф.",), ("доц.",), ("акад.",), ("напр.",),
        ("т.", "д."), ("т.", "п."), ("т.", "е."),
    },
}


_CLOSING_CHARS = "\"'”’»)]}"
_OPENING_CHARS = "\"'“‘«([{"


def _normalise_token(value: Any) -> str:
    """Normalize token casing and surrounding quote/bracket characters."""
    token = str(value or "").strip().casefold()
    token = token.strip(_CLOSING_CHARS).strip(_OPENING_CHARS)
    return token


def language_key(language_code: str | None) -> str:
    """Return the base language key from codes such as en-US or pt_BR."""
    if not language_code:
        return ""
    return re.split(r"[-_]", str(language_code).strip().casefold(), maxsplit=1)[0]


def is_non_terminal_abbreviation(
    words: Iterable[Mapping[str, Any]],
    index: int,
    language_code: str | None,
) -> bool:
    """Return True when words[index] ends an abbreviation, not a sentence.

    A following token is required. This prevents an abbreviation at the very
    end of a transcript from suppressing the final phrase boundary. The
    function supports both single-token and multi-token abbreviations.
    """
    key = language_key(language_code)
    patterns = NON_TERMINAL_ABBREVIATIONS.get(key)
    if not patterns:
        return False

    word_list = words if isinstance(words, (list, tuple)) else list(words)
    if index < 0 or index >= len(word_list) - 1:
        return False

    current = _normalise_token(word_list[index].get("word", ""))
    if not current.endswith("."):
        return False

    # A single-letter initial before another token is not a sentence end.
    # This also handles initials in names such as "J. D. Vance".
    if re.fullmatch(r"[^\W\d_]\.", current, flags=re.UNICODE):
        return True

    # In German and other languages using this notation, a number followed by
    # a period can be an ordinal ("20. Jahrhundert"), not sentence-final
    # punctuation. Do not suppress a final transcript boundary: the
    # next-token requirement above is intentional.
    if re.fullmatch(r"\d+\.", current):
        return True

    max_pattern_length = max((len(item) for item in patterns), default=1)
    available_start = max(0, index - max_pattern_length + 1)

    for pattern in patterns:
        pattern_start = index - len(pattern) + 1
        if pattern_start < available_start:
            continue
        observed = tuple(
            _normalise_token(word_list[position].get("word", ""))
            for position in range(pattern_start, index + 1)
        )
        if observed == pattern:
            return True

    return False


def is_sentence_ending_word(
    words: Iterable[Mapping[str, Any]],
    index: int,
    language_code: str | None,
    sentence_endings: tuple[str, ...] = (".", "!", "?", "。", "！", "？", "…"),
) -> bool:
    """Shared sentence-end test with multilingual abbreviation protection."""
    word_list = words if isinstance(words, (list, tuple)) else list(words)
    if index < 0 or index >= len(word_list):
        return False

    token = str(word_list[index].get("word", "") or "").strip()
    token = token.rstrip(_CLOSING_CHARS)
    if not token:
        return False

    if token.endswith(".") and is_non_terminal_abbreviation(
        word_list, index, language_code
    ):
        return False

    return token.endswith(sentence_endings)
