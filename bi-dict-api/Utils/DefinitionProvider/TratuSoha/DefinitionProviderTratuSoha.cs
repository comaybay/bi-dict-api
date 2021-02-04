namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    using bi_dict_api.Models;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DefinitionProviderTratuSoha : IDefinitionProvider
    {
        private readonly string languageQuery;
        private readonly TratuSohaParser parser;

        public DefinitionProviderTratuSoha(string languageQuery, TratuSohaParser parser)
        {
            this.languageQuery = languageQuery;
            this.parser = parser;
        }

        public async Task<Definition> Get(string word)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync($"http://tratu.soha.vn/dict/{languageQuery}/{word}");
            CheckIfValid(doc);
            return parser.Parse(doc.DocumentNode);
        }

        private static void CheckIfValid(HtmlDocument doc)
        {
            var noArticleDiv = doc.GetElementbyId("bodyContent")?.SelectSingleNode("div[@class='noarticletext']");
            if (noArticleDiv != null)
                throw new Exception("Word not found");
        }
    }
}
