namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using bi_dict_api.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class WordSuggestionsProviderWiki : IWordSuggestions
    {
        //using https://jdict.net/api/v1/ API
        private readonly IHttpClientFactory clientFactory;

        private readonly string language;
        private static readonly int Limit = 5;

        public WordSuggestionsProviderWiki(IHttpClientFactory clientFactory, string language)
        {
            this.clientFactory = clientFactory;
            this.language = language;
        }

        public async Task<IEnumerable<WordSuggestion>> Get(string word)
        {
            var response = await SendSuggestionsRequest(word);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get content. Status code: {response.StatusCode}");

            var rawSuggestions = await response.Content.ReadAsStringAsync();
            var openSearchSuggestions = GetOpenSearchSuggestions(rawSuggestions);
            return ParseSuggestions(openSearchSuggestions);
        }

        private async Task<HttpResponseMessage> SendSuggestionsRequest(string word)
        {
            string URL = $"https://{language}.wiktionary.org/w/api.php?action=opensearch&search={word}&limit={Limit}&namespace=0&format=json";
            var request = new HttpRequestMessage(HttpMethod.Get, URL);

            var client = clientFactory.CreateClient();
            return await client.SendAsync(request);
        }

        private static IList<string[]> GetOpenSearchSuggestions(string jsonText)
        {
            int start = jsonText.IndexOf(',') + 1;
            jsonText = '[' + jsonText[start..];
            return JsonSerializer.Deserialize<IList<string[]>>(jsonText) ?? new List<string[]>();
        }

        private static IEnumerable<WordSuggestion> ParseSuggestions(IList<string[]> osSuggestions)
        {
            //index:
            // 0 - suggestions
            // 1 - descriptions (disabled due to performance reasons)
            // 2 - suggestions page links
            int numberOfSuggestions = osSuggestions[0].Length;
            int suggestionLimit = (numberOfSuggestions < Limit) ? numberOfSuggestions : Limit;

            var suggestions = new WordSuggestion[suggestionLimit];
            for (int i = 0; i < suggestionLimit; i++)
            {
                suggestions[i] = new WordSuggestion()
                {
                    Word = osSuggestions[0][i],
                    Meaning = osSuggestions[1][i],
                };
            }
            return suggestions;
        }
    }
}