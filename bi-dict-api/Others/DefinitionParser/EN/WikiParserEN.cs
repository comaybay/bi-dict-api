namespace bi_dict_api.Others.DefinitionParser.EN {

    internal class WikiParserEN : WikiParserBase {

        private static readonly WikiParserOptions config = new WikiParserOptions() {
            GlobalPronunciationId = "Pronunciation",
            DefinitionLanguage = "EN"
        };

        private static readonly IWikiEtymologyParser etymologyParser = new WikiEtymologyParserEN();
        private static readonly IWikiParserHelper helper = new WikiParserHelperEN();

        public WikiParserEN() {
            Helper = helper;
            Config = config;
            EtymologyParser = etymologyParser;
        }
    }
}