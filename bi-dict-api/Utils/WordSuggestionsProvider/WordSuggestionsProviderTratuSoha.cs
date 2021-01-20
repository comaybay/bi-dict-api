namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using bi_dict_api.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    //using http://tratu.soha.vn/extensions/curl_suggest.php API
    public class WordSuggestionsProviderTratuSoha : IWordSuggestionsProvider
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly string languageCode;

        public WordSuggestionsProviderTratuSoha(IHttpClientFactory clientFactory, string languageCode)
        {
            this.clientFactory = clientFactory;
            this.languageCode = languageCode;
        }

        public async Task<IEnumerable<WordSuggestion>> Get(string word)
        {
            //example: http://tratu.soha.vn/extensions/curl_suggest.php?search=la&dict=vn_vn
            var response = await SendSuggestionsRequest(word);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get content. Status code: {response.StatusCode}");

            string rawSuggestions = await response.Content.ReadAsStringAsync();
            var doc = XDocument.Parse(rawSuggestions); //doc contains suggestions
            return ParseSuggestions(doc);
        }

        private static IEnumerable<WordSuggestion> ParseSuggestions(XDocument doc)
            => doc.Element("results")
                  ?.Elements("rs")
                  .Where(rs => rs.Attribute("type")?.Value == "0") //get word suggestions only
                  .Select(rs => new WordSuggestion()
                  {
                      Word = rs.Value,
                      Meaning = ParseMeaning(rs.Attribute("mean")?.Value ?? ""),
                  })
            ?? new List<WordSuggestion>();

        private static string ParseMeaning(string rawMeaning)
        {
            string meaning = rawMeaning.Replace("  ", " ");
            //case 1: no unrelated stuff
            if (!meaning.Contains("<font"))
                return meaning.Trim();

            //case 2: unrelated stuff on both side
            //case 3: unrelated stuff on the left
            int startIndex = meaning.IndexOf("</font>") + "</font>".Length;
            int endIndex = meaning.LastIndexOf("<font");
            meaning = (startIndex < endIndex) ? rawMeaning[startIndex..endIndex] : rawMeaning[startIndex..];
            return meaning.Trim();
        }

        private async Task<HttpResponseMessage> SendSuggestionsRequest(string word)
        {
            string URL = $"http://tratu.soha.vn/extensions/curl_suggest.php?search={word}&dict={languageCode}";
            var request = new HttpRequestMessage(HttpMethod.Get, URL);

            var client = clientFactory.CreateClient();
            return await client.SendAsync(request);
        }
    }
}