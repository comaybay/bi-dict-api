using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.VN {

    public class WikiParserVN : WikiParserBase {
        private static readonly IWikiEtymologyParser etymologyParser = new WikiEtymologyParserVN();
        private static readonly IWikiParserHelper helper = new WikiParserHelperVN();

        public WikiParserVN(string wordLanguage) {
            Config = new WikiParserOptions() {
                GlobalPronunciationId = "Cách_phát_âm",
                PageLanguage = "vi",
                WordLanguage = wordLanguage,
            };
            Helper = helper;
            EtymologyParser = etymologyParser;
        }
    }
}