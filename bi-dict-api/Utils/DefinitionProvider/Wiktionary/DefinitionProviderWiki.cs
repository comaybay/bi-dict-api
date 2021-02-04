namespace bi_dict_api.Utils.DefinitionProvider.Wiktionary
{
    using Models;
    using System.Net.Http;
    using System.Threading.Tasks;
    using WiktionaryParser;

    public class DefinitionProviderWiki : IDefinitionProvider
    {
        private readonly IWikiParser wikiParser;
        private readonly IHttpClientFactory clientFactory;

        public DefinitionProviderWiki(IHttpClientFactory clientFactory, IWikiParser wikiParser)
        {
            this.clientFactory = clientFactory;
            this.wikiParser = wikiParser;
        }

        public async Task<Definition> Get(string word)
        {
            var response = await SendPageHtmlRequest(word);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get wiktionary page content.");

            var html = await response.Content.ReadAsStringAsync();
            return wikiParser.Parse(html);
        }

        private async Task<HttpResponseMessage> SendPageHtmlRequest(string word)
        {
            var definitionLanguage = wikiParser.PageLanguage;
            string URL = $"https://{definitionLanguage}.wiktionary.org/api/rest_v1/page/html/{word}";

            var request = new HttpRequestMessage(HttpMethod.Get, URL);
            request.Headers.Add("Api-User-Agent", "thaichibao@gmail.com");

            var client = clientFactory.CreateClient();
            return await client.SendAsync(request);
        }
    }
}