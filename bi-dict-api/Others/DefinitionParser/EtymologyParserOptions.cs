using HtmlAgilityPack;

namespace bi_dict_api.Others.DefinitionParser {

    public class EtymologyParserOptions {
        public string EtymologySectionQuery { get; set; }
        public string EtymologyPronunciationQuery { get; set; }
        public string DefinitionSectionFilter { get; set; }
        public string InnerSectionSynonymQuery { get; set; }
        public string InnerSectionAntonymQuery { get; set; }
        public string SynonymTextQuery { get; set; }
        public string AntonymTextQuery { get; set; }
        public IDefinitionParserHelper Helper { get; set; }
    }
}