namespace bi_dict_api.Others.DefinitionParser
{

    public class WikiEtymologyParserOptions
    {
        public string EtymologySectionId { get; set; }
        public string EtymologyPronunciationId { get; set; }
        public string DefinitionSectionFilter { get; set; }
        public string InnerSectionSynonymId { get; set; }
        public string InnerSectionAntonymId { get; set; }
        public string SynonymTextQuery { get; set; }
        public string AntonymTextQuery { get; set; }
    }
}