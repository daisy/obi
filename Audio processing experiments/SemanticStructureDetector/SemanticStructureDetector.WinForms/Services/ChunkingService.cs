using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Services;

public class ChunkingService
{
    public List<List<PhraseModel>> Chunk(
        List<PhraseModel> phrases,
        //int chunkSize = 250,
        int chunkSize = 50,
        int overlap = 3)
    {
        var result = new List<List<PhraseModel>>();

        int index = 0;

        while (index < phrases.Count)
        {
            int size = Math.Min(
                chunkSize,
                phrases.Count - index);

            var chunk = phrases
                .Skip(index)
                .Take(size)
                .ToList();

            result.Add(chunk);

            index += chunkSize - overlap;
        }

        return result;
    }
}
