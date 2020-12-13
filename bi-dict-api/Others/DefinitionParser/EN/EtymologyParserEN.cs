namespace bi_dict_api.Others.DefinitionParser.EN {

    internal class EtymologyParserEN : EtymologyParserBase {

        public EtymologyParserEN() {
            Config = new EtymologyParserOptions() {
                DefinitionSectionFilter = "Adjective Adverb Ambiposition Article "
                                          + "Circumposition Classifier Conjunction Contraction Counter Determiner Ideophone Interjection "
                                          + "Noun Numeral Participle Particle Postposition Preposition Pronoun Proper noun Verb",
                EtymologyPronunciationQuery = "section > [id^='Pronunciation']",
                EtymologySectionQuery = "section > [id^='Etymology']",
                InnerSectionAntonymQuery = "section > [id^='Antonym']",
                InnerSectionSynonymQuery = "section > [id^='Synonym']",
                Helper = new DefinitionParserHelperEN(),
            };
        }
    }
}