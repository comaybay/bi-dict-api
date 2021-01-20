namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using bi_dict_api.Models;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    //using https://jdict.net/api/v1/ API
    public class WordSuggestionsProviderJdict : IWordSuggestionsProvider
    {
        private readonly IHttpClientFactory clientFactory;

        public WordSuggestionsProviderJdict(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<IEnumerable<WordSuggestion>> Get(string word)
        {
            var response = await SendSuggestionsRequest(word);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get content. Status code: {response.StatusCode}");

            var rawSuggestions = await response.Content.ReadAsStringAsync();
            JdictSuggestions jdictSuggestions = GetJdictSuggestions(rawSuggestions);
            return ParseSuggestions(jdictSuggestions);
        }

        private static JdictSuggestions GetJdictSuggestions(string jsonText)
        {
            jsonText = jsonText.Replace("suggest_mean", "suggestMean"); //snake_case sucks!!
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            return JsonSerializer.Deserialize<JdictSuggestions>(jsonText, options) ?? new JdictSuggestions();
        }

        private static IEnumerable<WordSuggestion> ParseSuggestions(JdictSuggestions jdictSuggestions)
            => jdictSuggestions.List.Select(suggestion =>
                new WordSuggestion()
                {
                    Word = $"{suggestion.Word} / {suggestion.Kana}",
                    Meaning = suggestion.SuggestMean.Replace("  ", " "), //remove weird typos.
                });

        private async Task<HttpResponseMessage> SendSuggestionsRequest(string word)
        {
            string URL = $"https://jdict.net/api/v1/suggest?keyword={word}&type=word";
            var request = new HttpRequestMessage(HttpMethod.Get, URL);

            var client = clientFactory.CreateClient();
            return await client.SendAsync(request);
        }
    }
}