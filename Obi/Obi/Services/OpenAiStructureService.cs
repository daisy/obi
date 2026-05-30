using Newtonsoft.Json;
using Obi.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Obi.Services
{
    public class OpenAiStructureService
    {
        private readonly HttpClient _httpClient;

        public OpenAiStructureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StructureResponse> DetectStructureAsync(
            List<PhraseModel> phrases,
            CancellationToken cancellationToken)
        {
            var simplified = phrases.Select(p => new
            {
                phraseId = p.PhraseId,
                text = p.Text
            });

            string systemPrompt = """
You are an AI semantic structure detector for audiobooks.

You will receive a sequence of transcript phrases in reading order.

Your task is to analyze the transcript CONTEXTUALLY and identify:

- book titles
- section headings
- subsection headings
- paragraphs

IMPORTANT:

Headings are contextual.
Do NOT classify phrases independently.
Use neighboring phrases to understand document structure.

Extract clean semantic headings from transcript phrases.
The original spoken transcript and the extracted headingText are separate concepts.

The transcript phrase must remain fully preserved.
Only headingText should be semantically cleaned.
headingText should ideally be shorter, cleaner, and more navigation-friendly than the original spoken transcript phrase.
Do not rewrite transcript phrases themselves.

For heading phrases:

- Preserve original transcript phrases unchanged
- Extract a separate clean semantic heading
- headingText should contain a concise, clean, navigation-friendly semantic title
- headingText should represent the semantic topic only

Headings often appear as:

- topic titles
- section names
- short standalone semantic units

Good headings are usually:

- concise
- meaningful independently
- title-like
- topic-oriented
- not sentence-like

Examples of good headings:

- Fast Facts
- Timeline
- History
- Island Nation
- Transportation
- Government
- Plants and Animals

Good headingText should avoid:

- page numbers
- narrator artifacts
- repeated punctuation
- filler transcript noise
- unnecessary metadata

Paragraphs are usually:

- complete sentences
- descriptive narration
- explanatory content

Avoid creating headings from transcript fragments that lack meaningful semantic topic information.

However, meaningful semantic section titles should still be identified even when spoken alongside page numbers or metadata.

VERY IMPORTANT RULES:

1. Preserve phrase IDs exactly
2. Never rewrite transcript phrases
3. Never merge phrases
4. Never split phrases
5. Preserve original ordering
6. Analyze phrases CONTEXTUALLY
7. Use neighboring phrases to infer semantic structure
8. Meaningful semantic section titles should still be identified clearly
9. Headings usually appear near related semantic content
10. Return valid JSON only

If a phrase contains both spoken metadata and a semantic title:

- preserve the full transcript phrase separately
- extract only the clean semantic title into headingText

Examples:

Transcript phrase:
"Page 1, The Countries, New Zealand, Tamara L. Brittain"

Correct headingText:
"The Countries: New Zealand"

Reason:
"book title extracted from spoken metadata"

Transcript phrase:
"Page 3. Contents."

Correct headingText:
"Contents"

Reason:
"major section heading"

Transcript phrase:
"Page 6. Timeline."

Correct headingText:
"Timeline"

Reason:
"major section heading"

Return this JSON format:

[
  {
    "phraseId": "p1",
    "role": "heading",
    "level": 1,
    "headingText": "The Countries: New Zealand",
    "confidence": 0.95,
    "reason": "book title"
  },
  {
    "phraseId": "p25",
    "role": "paragraph",
    "level": null,
    "headingText": "",
    "confidence": 0.98,
    "reason": "descriptive narration"
  }
]

Allowed roles:
- heading
- paragraph

Heading levels:

- level 1 = book title
- level 2 = major section heading
- level 3 = subsection heading

For paragraphs:

- role must be "paragraph"
- level must be null
- headingText must be empty string

Return valid JSON only.

The spoken transcript may contain page numbers, narration artifacts, or metadata.

Do NOT remove these from transcript phrases.

Instead extract only the semantic navigation title into headingText.
""";

            var payload = new
            {
                //model = "gpt-5",
                model = "gpt-4.1-mini",

                input = new object[]
         {
        new
        {
            role = "system",
            content = new object[]
            {
                new
                {
                    type = "input_text",
                    text = systemPrompt
                }
            }
        },

        new
        {
            role = "user",
            content = new object[]
            {
                new
                {
                    type = "input_text",
                   text =
                    $"""
                    Analyze the following audiobook transcript phrases in reading order.

                    IMPORTANT:
                    - Analyze phrases contextually
                    - Use neighboring phrases to infer semantic structure
                    - Headings often appear in semantic clusters
                    - Identify meaningful section titles
                    - Most phrases are paragraphs

                    Transcript:

                    {JsonConvert.SerializeObject(
                        new
                        {
                            phrases = simplified
                        },
                        Formatting.Indented)}
                    """
                }
            }
        }
         },

                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "semantic_structure",
                        schema = new
                        {
                            type = "object",

                            properties = new
                            {
                                structure = new
                                {
                                    type = "array",

                                    items = new
                                    {
                                        type = "object",

                                        properties = new
                                        {
                                            phraseId = new
                                            {
                                                type = "string"
                                            },

                                            role = new
                                            {
                                                type = "string"
                                            },

                                            level = new
                                            {
                                                type = new[]
                                         {
                                        "integer",
                                        "null"
                                    }
                                            },

                                            paragraphId = new
                                            {
                                                type = new[]
                                         {
                                        "string",
                                        "null"
                                    }
                                            },

                                            headingText = new
                                            {
                                                type = "string"
                                            },

                                            confidence = new
                                            {
                                                type = "number"
                                            },

                                            reason = new
                                            {
                                                type = "string"
                                            }
                                        },

                                        required = new[]
                                 {
                                "phraseId",
                                "role",
                                "level",
                                "paragraphId",
                                "headingText",
                                "confidence",
                                "reason"
                            },

                                        additionalProperties = false
                                    }
                                }
                            },

                            required = new[]
                     {
                    "structure"
                },

                            additionalProperties = false
                        }
                    }
                }
            };

            string json =
        JsonConvert.SerializeObject(
            payload,
            Formatting.Indented);

            File.WriteAllText(
                "last_request.json",
                json);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    3,
                    retry => TimeSpan.FromSeconds(retry * 2));

            var response = await retryPolicy.ExecuteAsync(
                async () =>
                {
                    using var request =
                        new HttpRequestMessage(
                            HttpMethod.Post,
                            "https://api.openai.com/v1/responses");

                    string apiKey =
                        Environment.GetEnvironmentVariable(
                            "OPENAI_API_KEY")
                        ?? throw new Exception(
                            "OPENAI_API_KEY not found");

                    request.Headers.Authorization =
                        new System.Net.Http.Headers
                            .AuthenticationHeaderValue(
                                "Bearer",
                                apiKey);

                    request.Content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                    return await _httpClient.SendAsync(
                        request,
                        cancellationToken);
                });

            //     response.EnsureSuccessStatusCode();

            string responseContent =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            File.WriteAllText(
        "last_response.json",
        responseContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OpenAI API Error:\n\n{responseContent}");
            }

            dynamic parsed =
        JsonConvert.DeserializeObject(responseContent)!;

            string outputJson =
                parsed.output[0].content[0].text.ToString();

            return JsonConvert.DeserializeObject
                <StructureResponse>(outputJson)!;
        }
    }
}
