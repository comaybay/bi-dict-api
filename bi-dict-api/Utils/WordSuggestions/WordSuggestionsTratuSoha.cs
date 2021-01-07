using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bi_dict_api.Utils.WordSuggestions {

    //using http://tratu.soha.vn/extensions/curl_suggest.php API
    public class WordSuggestionsTratuSoha : IWordSuggestions {
        private readonly IHttpClientFactory clientFactory;
        private readonly string languageCode;

        public WordSuggestionsTratuSoha(IHttpClientFactory clientFactory, string languageCode) {
            this.clientFactory = clientFactory;
            this.languageCode = languageCode;
        }

        public async Task<IEnumerable<WordSuggestion>> Get(string word) {
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
                  .Where(rs => rs.Attribute("type").Value == "0") //avoid "word in phrase" section.
                  .Select(rs => new WordSuggestion() {
                      Word = rs.Value,
                      Meaning = ParseMeaning(rs.Attribute("mean").Value),
                  });

        private static string ParseMeaning(string rawMeaning) {
            rawMeaning = rawMeaning.Trim();
            var index = rawMeaning.IndexOf("  ");  //API uses double space to separate part of speech and meaning
            if (index != -1) {
                int startIndex = index + 2; //add "  " offset
                return rawMeaning[startIndex..];
            }

            if (rawMeaning.Contains("</font>"))
                return "";

            return rawMeaning;
        }

        private async Task<HttpResponseMessage> SendSuggestionsRequest(string word) {
            string URL = $"http://tratu.soha.vn/extensions/curl_suggest.php?search={word}&dict={languageCode}";
            var request = new HttpRequestMessage(HttpMethod.Get, URL);

            var client = clientFactory.CreateClient();
            return await client.SendAsync(request);
        }
    }
}