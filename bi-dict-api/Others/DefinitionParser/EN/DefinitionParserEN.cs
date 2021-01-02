using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.EN {

    public class DefinitionParserEN : IDefinitionParser {
        private readonly WikiParserBase wikiParser;

        public DefinitionParserEN() {
            var config = new WikiParserOptions() {
                GlobalPronunciationId = "Pronunciation",
                DefinitionLanguage = "EN"
            };
            wikiParser = new WikiParserEN(config, new WikiEtymologyParserEN(), new WikiParserHelperEN());
        }

        public Definition FromWikitionaryHtml(string html, string wordLanguage) {
            return wikiParser.Parse(html, wordLanguage);
        }
    }
}