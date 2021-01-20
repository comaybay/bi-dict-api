namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.VN
{
    using Base;

    public class WikiParserVN : WikiParserBase
    {
        private static readonly IWikiEtymologyParser etymologyParser = new WikiEtymologyParserVN();
        private static readonly IWikiParserHelper helper = new WikiParserHelperVN();

        public WikiParserVN(string wordLanguage)
        {
            Config = new WikiParserOptions()
            {
                GlobalPronunciationId = "Cách_phát_âm",
                PageLanguage = "vi",
                WordLanguage = wordLanguage,
            };
            Helper = helper;
            EtymologyParser = etymologyParser;
        }
    }
}