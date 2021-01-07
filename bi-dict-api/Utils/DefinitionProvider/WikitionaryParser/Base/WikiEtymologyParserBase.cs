using bi_dict_api.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class WikiEtymologyParserBase : IWikiEtymologyParser {
        protected IWikiParserHelper Helper { get; init; }
        protected WikiEtymologyParserOptions Config { get; init; }
        static private readonly HtmlNode dummyElement = new HtmlDocument().CreateElement("div");

        public IEnumerable<EtymologySection> Parse(HtmlNode languageSection) {
            var rawEtymologySections = languageSection.QuerySelectorAll($"section > [id^='{Config.EtymologySectionId}']")
                                               .Select(elem => elem.ParentNode);
            var etymologySections = Array.Empty<EtymologySection>();

            if (rawEtymologySections.Count() > 1) {
                //Normal Case: Multiple EtymologySections (definitions are inside of EtymologySection sections)
                return rawEtymologySections.Select(rawEtymologySection => ParseEtymologySection(rawEtymologySection));
            }
            else {
                return new EtymologySection[] { ParseEtymologySectionSpecialCase(languageSection) };
            }
        }

        protected virtual EtymologySection ParseEtymologySectionSpecialCase(HtmlNode languageSection) {
            //Special Case where there is only one EtymologySection
            //(definitions might be outside of Etymology section or no Etymology section at all (html))
            //Examples: https://en.wiktionary.org/api/rest_v1/page/html/person (definitions are outside)
            //          https://en.wiktionary.org/api/rest_v1/page/html/ma     (definitions are not outside)
            var rawEtymologySection = languageSection.QuerySelector($"section > [id^='{Config.EtymologySectionId}']")
                                                    ?.ParentNode ?? dummyElement;

            var rawTexts = GetRawEtymologyTexts(rawEtymologySection);
            var rawInnerSections = GetRawEtymologyInnerSections(languageSection);
            return new EtymologySection {
                EtymologyTexts = rawTexts.Select(raw => ParseEtymologyText(raw)),
                Pronunciations = Array.Empty<string>(), //no need since these are in globalPronunciations
                InnerSections = rawInnerSections.Select(rawSection => ParseEtymologyInnerSection(rawSection)),
            };
        }

        protected virtual IEnumerable<HtmlNode> GetRawEtymologyTexts(HtmlNode rawEtymologySection)
            => rawEtymologySection.Elements("p");

        protected virtual IEnumerable<HtmlNode> GetRawEtymologyInnerSections(HtmlNode rawEtymologySection)
            => rawEtymologySection.Elements("section")
                                  .Where(rawSection => {
                                      //select first elem text
                                      var sectionTitle = GetEtymologyInnerSectionTitle(rawSection);
                                      if (String.IsNullOrEmpty(sectionTitle))
                                          return false;
                                      else
                                          //if section title contains one of these words
                                          return Config.DefinitionSectionFilter.Contains(sectionTitle);
                                  });

        private string GetEtymologyInnerSectionTitle(HtmlNode rawSection) {
            //for some reason querySelector("h6 h5 h4 h3 h2 h1") doesn't work, had to settle with this.
            string f(string q) => Helper.QuerySelectorDirectChildren(rawSection, q)?.InnerText;
            return f("h6") ?? f("h5") ?? f("h4") ?? f("h3") ?? f("h2") ?? f("h1");
        }

        protected virtual EtymologySection ParseEtymologySection(HtmlNode rawEtymologySection) {
            var rawEtymologyTexts = GetRawEtymologyTexts(rawEtymologySection);
            var rawInnerSections = GetRawEtymologyInnerSections(rawEtymologySection);

            return new EtymologySection {
                EtymologyTexts = rawEtymologyTexts.Select(raw => ParseEtymologyText(raw)),
                Pronunciations = ParseEtymologySectionPronunciations(rawEtymologySection),
                InnerSections = rawInnerSections.Select(rawSection => ParseEtymologyInnerSection(rawSection)),
            };
        }

        protected virtual string ParseEtymologyText(HtmlNode rawText)
            => Helper.RemoveCiteNotes(rawText.InnerText);

        protected virtual IEnumerable<string> ParseEtymologySectionPronunciations(HtmlNode rawEtymologySection) {
            var pronunciationSection = rawEtymologySection.QuerySelector($"section > [id^='{Config.EtymologyPronunciationId}']")
                                                   ?.ParentNode;
            if (pronunciationSection is null)
                return Array.Empty<string>();
            else
                return Helper.ParsePronunciationFrom(pronunciationSection);
        }

        protected virtual EtymologyInnerSection ParseEtymologyInnerSection(HtmlNode rawSection) {
            var rawDefinitionSections = GetRawDefinitionSections(rawSection);
            var rawSynonymSection = GetRawInnerSectionSynonymSection(rawSection) ?? dummyElement;
            var rawAntonymSection = GetRawInnerSectionAntonymSection(rawSection) ?? dummyElement;

            return new EtymologyInnerSection() {
                PartOfSpeech = GetEtymologyInnerSectionTitle(rawSection),
                Inflection = rawSection.SelectSingleNode("p")?.InnerText ?? "",
                DefinitionSections = rawDefinitionSections.Select(raw => ParseDefinitionSection(raw)),
                Synonyms = ParseInnerSectionSynonymSection(rawSynonymSection),
                Antonyms = ParseInnerSectionAntonymSection(rawAntonymSection),
            };
        }

        protected virtual IEnumerable<string> ParseInnerSectionSynonymSection(HtmlNode rawSynonymSection)
            => rawSynonymSection.QuerySelectorAll("ul > li")
                         .Where(elem => !elem.InnerText.Contains("See also")) // avoid "see also thesaurus"
                         .Select(elem => elem.InnerText);

        private HtmlNode GetRawInnerSectionSynonymSection(HtmlNode rawSection)
            => rawSection.QuerySelector($"section > [id^='{Config.InnerSectionSynonymId}']")
                        ?.ParentNode;

        protected virtual IEnumerable<string> ParseInnerSectionAntonymSection(HtmlNode rawAntonymSection)
            => rawAntonymSection.QuerySelectorAll("ul > li")
                                .Where(elem => !elem.InnerText.Contains("See also")) // avoid "see also thesaurus"
                                .Select(elem => elem.InnerText);

        protected virtual HtmlNode GetRawInnerSectionAntonymSection(HtmlNode rawEtymologySection)
            => rawEtymologySection.QuerySelector($"section > [id^='{Config.InnerSectionAntonymId}']")
                                  ?.ParentNode;

        protected virtual IEnumerable<HtmlNode> GetRawDefinitionSections(HtmlNode rawInnerSection)
            => Helper.QuerySelectorAllDirectChildren(rawInnerSection, "ol > li");

        protected virtual DefinitionSection ParseDefinitionSection(HtmlNode rawDefinitionSection) {
            //rawDefinitionSection should be an li element that might
            //contains an ul element (examples with citation)
            //or an dl element (examples without citation)
            //and might also contain synonyms or antonyms
            var rawSubDefinitions = GetRawDefinitionSections(rawDefinitionSection);

            return new DefinitionSection() {
                Definition = ParseDefinitionSectionDefinition(rawDefinitionSection),
                Examples = ParseExamples(rawDefinitionSection),
                Synonyms = ParseDefinitionSectionSynonyms(rawDefinitionSection),
                Antonyms = ParseDefinitionSectionAntonyms(rawDefinitionSection),
                SubDefinitions = rawSubDefinitions.Select(raw => ParseDefinitionSection(raw)) //recursion
            };
        }

        protected virtual string ParseDefinitionSectionDefinition(HtmlNode rawDefinitionSection) {
            string definition = rawDefinitionSection.InnerText; //definition text, includes examples if they exist
            int definitionLength = definition.IndexOf('\n');    //definition length will be -1 if there are no examples
            return definitionLength == -1 ? definition : definition.Substring(0, definitionLength);
        }

        protected virtual IEnumerable<string> ParseExamples(HtmlNode rawDefinitionSection) {
            bool isElementContainsExamples(HtmlNode elem) {
                var text = elem.InnerText;
                return text.Length != 0 && //some elements in a wiki have no text (even though they should have).
                       !elem.InnerText.StartsWith(Config.SynonymTextQuery) &&
                       !elem.InnerText.StartsWith(Config.AntonymTextQuery);
            }

            //examples without citation
            var withoutCitation = Helper.QuerySelectorAllDirectChildren(rawDefinitionSection, "dl > dd")
                                               .Where(node => isElementContainsExamples(node))
                                               .Select(node => node.InnerText);

            //exaples with citation
            var withCitation = Helper.QuerySelectorAllDirectChildren(rawDefinitionSection, "ul > li dl > dd")
                                            .Where(node => isElementContainsExamples(node))
                                            .Select(node => node.InnerText);

            return withoutCitation.Concat(withCitation);
        }

        protected virtual IEnumerable<string> ParseDefinitionSectionSynonyms(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.QuerySelectorAll("dl > dd")
                                   .Where(elem => elem.InnerText.StartsWith(Config.SynonymTextQuery))
                                   .SelectMany(elem => GetWordsWithoutTherasus(elem.InnerText));

        protected virtual IEnumerable<string> ParseDefinitionSectionAntonyms(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.QuerySelectorAll("dl > dd")
                                   .Where(elem => elem.InnerText.StartsWith(Config.AntonymTextQuery))
                                   .SelectMany(elem => GetWordsWithoutTherasus(elem.InnerText));

        private static IEnumerable<string> GetWordsWithoutTherasus(string text) {
            int startIndex = text.IndexOf(' ') + 1; //remove "Antonyms(s) " or "Synonym(s): " from text
            int endIndex = text.LastIndexOf("; see also"); //remove "; see also: thesaurus"
            text = endIndex == -1 ? text[startIndex..] : text[startIndex..endIndex];
            return text.Split(", ");
        }
    }
}