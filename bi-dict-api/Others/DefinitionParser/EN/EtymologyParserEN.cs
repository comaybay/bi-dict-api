namespace bi_dict_api.Others.DefinitionParser.EN {

    internal class EtymologyParserEN : EtymologyParserBase {

        public EtymologyParserEN() {
            Config = new EtymologyParserOptions() {
                DefinitionSectionFilter = "Adjective Adverb Ambiposition Article "
                                          + "Circumposition Classifier Conjunction Contraction Counter Determiner Ideophone Interjection "
                                          + "Noun Numeral Participle Particle Postposition Preposition Pronoun Proper noun Verb",
                EtymologyPronunciationId = "Pronunciation",
                EtymologySectionId = "Etymology",
                InnerSectionAntonymId = "Antonym",
                InnerSectionSynonymId = "Synonym",
                AntonymTextQuery = "Antonym",
                SynonymTextQuery = "Synonym",
                Helper = new DefinitionParserHelperEN(),
            };
        }
    }
}