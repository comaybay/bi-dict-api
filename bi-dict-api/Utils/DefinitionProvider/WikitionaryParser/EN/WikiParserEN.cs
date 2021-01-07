namespace bi_dict_api.Others.DefinitionParser.EN {

    internal class WikiParserEN : WikiParserBase {
        private static readonly IWikiEtymologyParser etymologyParser = new WikiEtymologyParserEN();
        private static readonly IWikiParserHelper helper = new WikiParserHelperEN();

        public WikiParserEN(string wordLanguage) {
            Config = new WikiParserOptions() {
                GlobalPronunciationId = "Pronunciation",
                PageLanguage = "en",
                WordLanguage = wordLanguage,
            };
            Helper = helper;
            EtymologyParser = etymologyParser;
        }
    }
}