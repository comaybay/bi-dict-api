namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using bi_dict_api.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
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

        private IEnumerable<WordSuggestion> ParseSuggestions(XDocument doc)
            => doc.Element("results")
                  ?.Elements("rs")
                  .Where(rs => rs.Attribute("type")?.Value == "0") //get word suggestions only
                  .Select(rs => new WordSuggestion()
                  {
                      Word = rs.Value,
                      Meaning = ParseMeaning(rs.Attribute("mean")?.Value ?? ""),
                  })
                  .Where(ws => ws.Meaning != "")
            ?? new List<WordSuggestion>();

        private string ParseMeaning(string rawMeaning)
        {
            //weird shit. Examples: http://tratu.soha.vn/extensions/curl_suggest.php?search=b&dict=en_vn
            //                      http://tratu.soha.vn/extensions/curl_suggest.php?search=c%C3%A1&dict=en_vn
            if (languageCode == "en_vn" && rawMeaning.Length != rawMeaning.Normalize(NormalizationForm.FormD).Length)
                return "";

            //rawMeaning contains no unnecessary parts (text inside <font> tag) 
            if (!rawMeaning.Contains("<font"))
                return rawMeaning.Trim().Replace("  ", " ");

            //rawMeaning contains unnecessary parts, remove them 
            return Regex.Replace(rawMeaning, @"<font.*?(</font>|$)", "");
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