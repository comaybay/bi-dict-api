namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser
{

    public class WikiEtymologyParserOptions
    {
        public string EtymologySectionId { get; set; } = default!;
        public string EtymologyPronunciationId { get; set; } = default!;
        public string DefinitionSectionFilter { get; set; } = default!;
        public string InnerSectionSynonymId { get; set; } = default!;
        public string InnerSectionAntonymId { get; set; } = default!;
        public string SynonymTextQuery { get; set; } = default!;
        public string AntonymTextQuery { get; set; } = default!;
    }
}