using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.VN {

    public class DefinitionParserVN : IDefinitionParser {
        private readonly WikiParserBase wikiParser;

        public DefinitionParserVN() {
            var config = new WikiParserOptions {
                GlobalPronunciationId = "Cách_phát_âm",
                DefinitionLanguage = "VN"
            };
            wikiParser = new WikiParserVN(config, new WikiEtymologyParserVN(), new WikiParserHelperVN());
        }

        public Definition FromWikitionaryHtml(string html, string wordLanguage) {
            return wikiParser.Parse(html, wordLanguage);
        }
    }
}