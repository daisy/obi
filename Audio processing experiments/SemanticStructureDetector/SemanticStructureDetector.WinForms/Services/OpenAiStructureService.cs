using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Polly;
using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Services;

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
You are an AI semantic structure detector.

Analyze phrase-level transcript data and determine:

- headings
- heading levels
- paragraph grouping

Rules:

1. Preserve phrase IDs exactly
2. Never rewrite text
3. Never merge phrases
4. Preserve original ordering
5. Return valid JSON only

Allowed roles:
- heading
- paragraph

Heading guidelines:
- headings are usually short
- headings usually do not end with punctuation
- headings represent titles or section labels
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
                    text = JsonConvert.SerializeObject(new
                    {
                        phrases = simplified
                    })
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
                                        }
                                    },

                                    required = new[]
                             {
                                "phraseId",
                                "role",
                                "level",
                                "paragraphId"
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