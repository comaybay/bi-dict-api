namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.EN
{
    using Base;

    internal class WikiParserEN : WikiParserBase
    {
        private static readonly IWikiEtymologyParser etymologyParser = new WikiEtymologyParserEN();
        private static readonly IWikiParserHelper helper = new WikiParserHelperEN();

        public WikiParserEN(string wordLanguage)
        {
            Config = new WikiParserOptions()
            {
                GlobalPronunciationId = "Pronunciation",
                PageLanguage = "en",
                WordLanguage = wordLanguage,
            };
            Helper = helper;
            EtymologyParser = etymologyParser;
        }
    }
}