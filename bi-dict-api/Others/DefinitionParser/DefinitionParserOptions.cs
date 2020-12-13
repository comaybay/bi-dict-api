namespace bi_dict_api.Others.DefinitionParser {

    public class DefinitionParserOptions {
        public string GlobalPronunciationQuery { get; set; }
        public IEtymologyParser EtymologyParser { get; set; }
        public string WordLanguage { get; set; }
        public string DefinitionLanguage { get; set; }
        public IDefinitionParserHelper Helper { get; set; }
    }
}