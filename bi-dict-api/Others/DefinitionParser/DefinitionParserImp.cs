using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public class DefinitionParserImp : IDefinitionParser {
        private readonly IWikiParser wikiParser;

        public DefinitionParserImp(IWikiParser wikiParser) {
            this.wikiParser = wikiParser;
        }

        public Definition FromWikitionaryHtml(string html, string wordLanguage)
            => wikiParser.Parse(html, wordLanguage);
    }
}