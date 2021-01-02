namespace bi_dict_api.Others.DefinitionParser.EN {

    internal class WikiParserEN : WikiParserBase {

        public WikiParserEN(WikiParserOptions config, IWikiEtymologyParser etymologyParser, IWikiParserHelper helper)
            : base(config, etymologyParser, helper) { }
    }
}