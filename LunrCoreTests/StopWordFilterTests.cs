﻿using Lunr;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LunrCoreTests
{
    public class StopWordFilterTests
    {
        private readonly StopWordFilterBase filter = new EnglishStopWordFilter();

        [Fact]
        public async Task StopWordFilterFiltersStopWords()
        {
            string[] stopWords = new[] { "the", "and", "but", "than", "when" };
            
            foreach (string word in stopWords)
            {
                Assert.True(filter.IsStopWord(word));
                Assert.Empty(await FilterHelper(word));
            }
        }

        [Fact]
        public async Task StopWordFilterIgnoresNonStopWords()
        {
            string[] nonStopWords = new[] { "interesting", "words", "pass", "through" };

            foreach (string word in nonStopWords)
            {
                Assert.False(filter.IsStopWord(word));
                Assert.Equal(new[] { word }, await FilterHelper(word));
            }
        }

        // Note: the lunr.js library has other tests here that are more
        // implementation specific and that I'm not replicating here.

        private async Task<string[]> FilterHelper(string word)
        {
            var result = new List<string>();
            var cancellationToken = new CancellationToken();
            await foreach (Token token in filter.FilterFunction(
                new Token(word),
                0,
                AsyncEnumerableExtensions.Empty<Token>(),
                cancellationToken))
            {
                result.Add(token.String);
            }
            return result.ToArray();
        }
    }
}
