namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.EN
{
    using Base;
    using bi_dict_api.Models;
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class WikiEtymologyParserEN : WikiEtymologyParserBase
    {
        public WikiEtymologyParserEN()
        {
            Helper = new WikiParserHelperEN();
            Config = new WikiEtymologyParserOptions()
            {
                DefinitionSectionFilter = "Adjective Adverb Ambiposition Article "
                                              + "Circumposition Classifier Conjunction Contraction Counter Determiner Ideophone Interjection "
                                              + "Noun Numeral Participle Particle Postposition Preposition Pronoun Proper noun Verb Phrase Letter",
                EtymologyPronunciationId = "Pronunciation",
                EtymologySectionId = "Etymology",
                InnerSectionAntonymId = "Antonym",
                InnerSectionSynonymId = "Synonym",
                AntonymTextQuery = "Antonym",
                SynonymTextQuery = "Synonym",
            };
        }
        public override IEnumerable<EtymologySection> Parse(HtmlNode languageSection)
        {
            var rawEtymologySections = GetRawEtymologySections(languageSection);
            if (rawEtymologySections.Count() > 1)
                //Normal Case: Multiple EtymologySections (definitions are inside of EtymologySection sections)
                return rawEtymologySections.Select(rawEtymologySection => ParseEtymologySection(rawEtymologySection));
            else
                return new EtymologySection[] { ParseEtymologySectionSpecialCase(languageSection) };
        }

        private EtymologySection ParseEtymologySectionSpecialCase(HtmlNode languageSection)
        {
            //Special Case where there is only one EtymologySection
            //(definitions might be outside of Etymology section or no Etymology section at all (html))
            //Examples: https://en.wiktionary.org/api/rest_v1/page/html/person (definitions are outside)
            //          https://en.wiktionary.org/api/rest_v1/page/html/ma     (definitions are not outside)
            var rawEtymologySection = languageSection.QuerySelector($"section > [id^='{Config.EtymologySectionId}']")
                                                    ?.ParentNode ?? dummyElement;

            var rawTexts = GetRawEtymologyTexts(rawEtymologySection);
            var rawInnerSections = GetRawEtymologyInnerSections(languageSection);
            return new EtymologySection
            {
                EtymologyTexts = rawTexts.Select(raw => ParseEtymologyText(raw)),
                Pronunciations = Array.Empty<string>(), //no need since these are in globalPronunciations
                InnerSections = rawInnerSections.Select(rawSection => ParseEtymologyInnerSection(rawSection)),
            };
        }

        protected override IEnumerable<string> ParseDefinitionSectionSynonyms(HtmlNode rawDefinitionSection)
        {
            return base.ParseDefinitionSectionSynonyms(rawDefinitionSection)
                       .SelectMany(raw => ParseRawSynonymsOrAntonyms(raw));
        }
        protected override IEnumerable<string> ParseDefinitionSectionAntonyms(HtmlNode rawDefinitionSection)
        {
            return base.ParseDefinitionSectionAntonyms(rawDefinitionSection)
                       .SelectMany(raw => ParseRawSynonymsOrAntonyms(raw));
        }

        private static IEnumerable<string> ParseRawSynonymsOrAntonyms(string text)
        {
            int startIndex = text.IndexOf(' ') + 1; //remove "Antonyms(s) " or "Synonym(s): " from text
            int endIndex = text.LastIndexOf("; see also"); //remove "; see also: thesaurus"
            text = endIndex == -1 ? text[startIndex..] : text[startIndex..endIndex];
            return text.Split(", ");
        }

    }
}