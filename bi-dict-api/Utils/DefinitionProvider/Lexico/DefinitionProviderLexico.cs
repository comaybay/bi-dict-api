namespace bi_dict_api.Utils.DefinitionProvider.Lexico
{
    using bi_dict_api.Models;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DefinitionProviderLexico : IDefinitionProvider
    {
        private readonly LexicoParser parser;

        public DefinitionProviderLexico(LexicoParser parser)
        {
            this.parser = parser;
        }

        public async Task<Definition> Get(string word)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync($"https://www.lexico.com/en/definition/{word}");

            return parser.Parse(doc.DocumentNode);
        }
    }
}
