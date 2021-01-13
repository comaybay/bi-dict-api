namespace bi_dict_api.Others.DefinitionParser.EN
{

    internal class WikiEtymologyParserEN : WikiEtymologyParserBase
    {

        public WikiEtymologyParserEN()
        {
            Helper = new WikiParserHelperEN();
            Config = new WikiEtymologyParserOptions()
            {
                DefinitionSectionFilter = "Adjective Adverb Ambiposition Article "
                                              + "Circumposition Classifier Conjunction Contraction Counter Determiner Ideophone Interjection "
                                              + "Noun Numeral Participle Particle Postposition Preposition Pronoun Proper noun Verb",
                EtymologyPronunciationId = "Pronunciation",
                EtymologySectionId = "Etymology",
                InnerSectionAntonymId = "Antonym",
                InnerSectionSynonymId = "Synonym",
                AntonymTextQuery = "Antonym",
                SynonymTextQuery = "Synonym",
            };
        }
    }
}