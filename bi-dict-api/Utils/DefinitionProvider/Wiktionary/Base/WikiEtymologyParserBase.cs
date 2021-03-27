namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.Base
{
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class WikiEtymologyParserBase : IWikiEtymologyParser
    {
        protected IWikiParserHelper Helper { get; init; } = default!;
        protected WikiEtymologyParserOptions Config { get; init; } = default!;
        static protected readonly HtmlNode dummyElement = new HtmlDocument().CreateElement("div");

        public virtual IEnumerable<Etymology> Parse(HtmlNode languageSection)
        {
            var rawEtymologySections = GetRawEtymologySections(languageSection);
            return rawEtymologySections.Select(rawEtymologySection => ParseEtymologySection(rawEtymologySection));
        }

        protected virtual IEnumerable<HtmlNode> GetRawEtymologySections(HtmlNode languageSection)
            => languageSection.QuerySelectorAll($"section > [id^='{Config.EtymologySectionId}']")
                                               .Select(elem => elem.ParentNode);

        protected virtual IEnumerable<HtmlNode> GetRawEtymologyTexts(HtmlNode rawEtymologySection)
            => rawEtymologySection.Elements("p");

        protected virtual IEnumerable<HtmlNode> GetRawEtymologyInnerSections(HtmlNode rawEtymologySection)
            => rawEtymologySection.Elements("section")
                                  .Where(rawSection =>
                                  {
                                      //select first elem text
                                      var rawSectionTitle = GetRawPartOfSpeech(rawSection) ?? dummyElement;
                                      var sectionTitle = ParsePartOfSpeech(rawSectionTitle);
                                      if (String.IsNullOrEmpty(sectionTitle))
                                          return false;
                                      else
                                          //if section title contains one of these words
                                          return Config.DefinitionSectionFilter.Contains(sectionTitle);
                                  });
        protected virtual HtmlNode? GetRawPartOfSpeech(HtmlNode rawSection)
            => rawSection.SelectSingleNode("(h6 | h5 | h4 | h3 | h2 | h1)");
        protected virtual string ParsePartOfSpeech(HtmlNode rawSection)
            => rawSection.InnerText;

        protected virtual Etymology ParseEtymologySection(HtmlNode rawEtymologySection)
        {
            var rawEtymologyTexts = GetRawEtymologyTexts(rawEtymologySection);
            var rawInnerSections = GetRawEtymologyInnerSections(rawEtymologySection);
            var rawPronunciationSection = GetRawEtymologyPronunciationSection(rawEtymologySection) ?? dummyElement;

            return new Etymology
            {
                Origin = rawEtymologyTexts.Select(raw => ParseEtymologyText(raw)),
                Pronunciations = ParseEtymologySectionPronunciations(rawPronunciationSection),
                InnerSections = rawInnerSections.Select(rawSection => ParseEtymologyInnerSection(rawSection)),
                Audio = "",
            };
        }

        protected virtual HtmlNode? GetRawEtymologyPronunciationSection(HtmlNode rawEtymologySection)
            => rawEtymologySection.QuerySelector($"section > [id^='{Config.EtymologyPronunciationId}']")
                                 ?.ParentNode;
        protected virtual string ParseEtymologyText(HtmlNode rawText)
            => Helper.RemoveCiteNotes(rawText.InnerText);

        protected virtual IEnumerable<string> ParseEtymologySectionPronunciations(HtmlNode rawPronunciaionSection)
            => Helper.ParsePronunciationsFrom(rawPronunciaionSection);

        protected virtual EtymologyInnerSection ParseEtymologyInnerSection(HtmlNode rawSection)
        {
            var rawPartOfSpeech = GetRawPartOfSpeech(rawSection) ?? dummyElement;
            var rawInflection = GetRawInflection(rawSection) ?? dummyElement;
            var rawDefinitionSections = GetRawDefinitionSections(rawSection);
            var rawSynonymSection = GetRawInnerSectionSynonymSection(rawSection) ?? dummyElement;
            var rawAntonymSection = GetRawInnerSectionAntonymSection(rawSection) ?? dummyElement;

            return new EtymologyInnerSection()
            {
                PartOfSpeech = ParsePartOfSpeech(rawPartOfSpeech),
                Inflection = ParseInfection(rawInflection),
                Senses = rawDefinitionSections.Select(raw => ParseDefinitionSection(raw)),
                Synonyms = ParseInnerSectionSynonymSection(rawSynonymSection),
                Antonyms = ParseInnerSectionAntonymSection(rawAntonymSection),
            };
        }

        protected virtual string ParseInfection(HtmlNode rawInflection)
            => rawInflection.InnerText;

        protected virtual HtmlNode? GetRawInflection(HtmlNode rawSection)
            => rawSection.SelectSingleNode("p");

        protected virtual IEnumerable<string> ParseInnerSectionSynonymSection(HtmlNode rawSynonymSection)
            => rawSynonymSection.QuerySelectorAll("ul > li")
                         .Where(elem => !Regex.IsMatch(elem.InnerText, @"see( also)? thesaurus", RegexOptions.IgnoreCase)) // avoid "see also thesaurus"
                         .Select(elem => elem.InnerText);

        protected virtual HtmlNode? GetRawInnerSectionSynonymSection(HtmlNode rawSection)
            => rawSection.QuerySelector($"section > [id^='{Config.InnerSectionSynonymId}']")
                        ?.ParentNode;

        protected virtual IEnumerable<string> ParseInnerSectionAntonymSection(HtmlNode rawAntonymSection)
            => rawAntonymSection.QuerySelectorAll("ul > li")
                                .Where(elem => !elem.InnerText.Contains("See also")) // avoid "see also thesaurus"
                                .Select(elem => elem.InnerText);

        protected virtual HtmlNode? GetRawInnerSectionAntonymSection(HtmlNode rawEtymologySection)
            => rawEtymologySection.QuerySelector($"section > [id^='{Config.InnerSectionAntonymId}']")
                                  ?.ParentNode;

        protected virtual IEnumerable<HtmlNode> GetRawDefinitionSections(HtmlNode rawInnerSection)
            => Helper.QuerySelectorAllDirectChildren(rawInnerSection, "ol > li");

        protected virtual Sense ParseDefinitionSection(HtmlNode rawDefinitionSection)
        {
            //rawDefinitionSection should be an li element that might
            //contains an ul element (examples with citation)
            //or an dl element (examples without citation)
            //and might also contain synonyms or antonyms
            var rawSubDefinitions = GetRawDefinitionSections(rawDefinitionSection);

            return new Sense()
            {

                Meaning = ParseDefinitionSectionDefinition(rawDefinitionSection),
                Examples = ParseExamples(rawDefinitionSection),
                Synonyms = ParseDefinitionSectionSynonyms(rawDefinitionSection),
                Antonyms = ParseDefinitionSectionAntonyms(rawDefinitionSection),
                SubSenses = rawSubDefinitions.Select(raw => ParseDefinitionSection(raw)),
                GrammaticalNote = "", //too complicated to parse, these things are in Meaning string.
                Region = "",
                SenseRegisters = "",
            };
        }

        protected virtual string ParseDefinitionSectionDefinition(HtmlNode rawDefinitionSection)
        {
            string definition = rawDefinitionSection.InnerText; //definition text, includes examples if they exist
            int definitionLength = definition.IndexOf('\n');    //definition length will be -1 if there are no examples
            return definitionLength == -1 ? definition : definition.Substring(0, definitionLength);
        }

        protected virtual IEnumerable<string> ParseExamples(HtmlNode rawDefinitionSection)
        {
            bool IsElementContainsExamples(HtmlNode elem)
            {
                var text = elem.InnerText;
                return text.Length != 0 && //some elements in a wiki have no text (even though they should have).
                       !elem.InnerText.StartsWith(Config.SynonymTextQuery) &&
                       !elem.InnerText.StartsWith(Config.AntonymTextQuery);
            }

            //examples without citation
            var withoutCitation = Helper.QuerySelectorAllDirectChildren(rawDefinitionSection, "dl > dd")
                                               .Where(node => IsElementContainsExamples(node))
                                               .Select(node => node.InnerText);

            //exaples with citation
            var withCitation = Helper.QuerySelectorAllDirectChildren(rawDefinitionSection, "ul > li dl > dd")
                                            .Where(node => IsElementContainsExamples(node))
                                            .Select(node => node.InnerText);

            return withoutCitation.Concat(withCitation);
        }

        protected virtual IEnumerable<string> ParseDefinitionSectionSynonyms(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.QuerySelectorAll("dl > dd")
                                   .Where(elem => elem.InnerText.StartsWith(Config.SynonymTextQuery))
                                   .Select(elem => elem.InnerText);

        protected virtual IEnumerable<string> ParseDefinitionSectionAntonyms(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.QuerySelectorAll("dl > dd")
                                   .Where(elem => elem.InnerText.StartsWith(Config.AntonymTextQuery))
                                   .Select(elem => elem.InnerText);
    }
}