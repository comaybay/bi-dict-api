using bi_dict_api.Models.DefinitionEN;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class EtymologyParserBase : IEtymologyParser {
        protected EtymologyParserOptions Config { get; set; }

        public IList<EtymologySection> Parse(HtmlNode languageSection) {
            var rawEtymologys = languageSection.QuerySelectorAll(Config.EtymologySectionQuery)
                                                .Select(elem => elem.ParentNode);

            //Normal Case: Multiple Etymologys (definitions are inside of etymology sections)
            var etymologys = rawEtymologys.Select(rawEtymology => ParseEtymology(rawEtymology))
                                          .ToList();

            if (rawEtymologys.Count() == 1) {
                //Special Case where there is only one etymology (definitions might be outside of etymology section)
                //Example: https://en.wiktionary.org/api/rest_v1/page/html/person (definitions are outside)
                //         https://en.wiktionary.org/api/rest_v1/page/html/ma     (definitions are not outside)
                var etymology = ParseEtymologySpecialCase(languageSection);
                bool isEmpty = etymology.Pronunciations.Count == 0 && etymology.InnerSections.Count == 0;
                if (!isEmpty)
                    etymologys.Add(etymology);
            }

            return etymologys;
        }

        private EtymologySection ParseEtymologySpecialCase(HtmlNode languageSection) => new EtymologySection {
            Pronunciations = new List<string>(), //no need since these are in globalPronunciations
            InnerSections = ParseEtymologyInnerSections(languageSection)
        };

        private EtymologySection ParseEtymology(HtmlNode rawEtymology) => new EtymologySection {
            Pronunciations = ParseEtymologyPronunciations(rawEtymology),
            InnerSections = ParseEtymologyInnerSections(rawEtymology),
        };

        private IList<string> ParseEtymologyPronunciations(HtmlNode rawEtymology) {
            var pronunciationSection = rawEtymology.QuerySelector(Config.EtymologyPronunciationQuery)
                                                   ?.ParentNode;
            if (pronunciationSection is null)
                return new List<string>();
            else
                return Config.Helper.ParsePronunciationFrom(pronunciationSection);
        }

        private IList<EtymologyInnerSection> ParseEtymologyInnerSections(HtmlNode rawEtymology) {
            var innerSections = rawEtymology.Elements("section")
                                            ?.Where(rawSection => {
                                                var sectionTitle = rawSection.FirstChild.InnerText;
                                                //if section title contains one of these words
                                                return Config.DefinitionSectionFilter.Contains(sectionTitle);
                                            })
                                            ?.Select(rawSection => ParseEtymologyInnerSection(rawSection))
                                            .ToList();

            return innerSections ?? new List<EtymologyInnerSection>();
        }

        private EtymologyInnerSection ParseEtymologyInnerSection(HtmlNode rawSection) => new EtymologyInnerSection() {
            PartOfSpeech = rawSection.FirstChild?.InnerText,
            Inflection = rawSection.SelectSingleNode("p")?.InnerText,
            DefinitionSections = ParseDefinitionSections(rawSection),
            Synonyms = ParseInnerSectionSynonymSection(rawSection),
            Antonyms = ParseInnerSectionAntonymSection(rawSection),
        };

        private IList<string> ParseInnerSectionSynonymSection(HtmlNode rawSection) {
            var synonyms = rawSection.QuerySelector(Config.InnerSectionSynonymQuery)
                                     ?.ParentNode
                                     ?.QuerySelectorAll("ul > li")
                                     ?.Select(elem => elem.InnerText)
                                     .ToList();

            return synonyms ?? new List<string>();
        }

        private IList<string> ParseInnerSectionAntonymSection(HtmlNode rawEtymology) {
            var antonyms = rawEtymology.QuerySelector(Config.InnerSectionAntonymQuery)
                                       ?.ParentNode
                                       ?.QuerySelectorAll("ul > li")
                                       ?.Select(elem => elem.InnerText)
                                       .ToList();

            return antonyms ?? new List<string>();
        }

        private IList<DefinitionSection> ParseDefinitionSections(HtmlNode rawSection) {
            var definitionSections = rawSection.ParentNode
                                               ?.QuerySelectorAll($"#{rawSection.Id} > ol > li")
                                               ?.Select(rawDefinitionSection => ParseDefinitionSection(rawDefinitionSection))
                                               .ToList();

            return definitionSections ?? new List<DefinitionSection>();
        }

        private DefinitionSection ParseDefinitionSection(HtmlNode rawDefinitionSection) {
            //rawDefinitionSection should be an li element that might
            //contains an ul element (examples with citation)
            //or an dl element (examples without citation)
            //and might also contain synonyms or antonyms

            return new DefinitionSection() {
                Definition = ParseDefinitionSectionDefinition(rawDefinitionSection),
                Examples = ParseExamples(rawDefinitionSection),
                Synonyms = ParseDefinitionSectionSynonyms(rawDefinitionSection),
                Antonyms = ParseDefinitionSectionAntonyms(rawDefinitionSection),
                SubDefinitions = ParseDefinitionSections(rawDefinitionSection) //recursion
            };
        }

        private static string ParseDefinitionSectionDefinition(HtmlNode rawDefinitionSection) {
            string definition = rawDefinitionSection.InnerText; //definition text, includes examples if they exist
            int definitionLength = definition.IndexOf('\n');    //definition length will be -1 if there are no examples

            return definitionLength == -1 ? definition : definition.Substring(0, definitionLength);
        }

        private static List<string> ParseExamples(HtmlNode rawDefinitionSection) {
            var examples = new List<string>();

            static bool isElementContainsExamples(HtmlNode elem) {
                var text = elem.InnerText;
                return text.Length != 0 && //some elements in a wiki have no text (even though they should have).
                       !elem.InnerText.StartsWith("Synonym") &&
                       !elem.InnerText.StartsWith("Antonym");
            }

            //examples without citation
            var withoutCitation = rawDefinitionSection.ParentNode
                                                      ?.QuerySelectorAll($"#{rawDefinitionSection.Id} > dl > dd")
                                                      ?.Where(node => isElementContainsExamples(node))
                                                      ?.Select(node => node.InnerText);

            //exaples with citation
            var withCitation = rawDefinitionSection.ParentNode
                                                   ?.QuerySelectorAll($"#{rawDefinitionSection.Id} > ul > li dl > dd")
                                                   ?.Where(node => isElementContainsExamples(node))
                                                   ?.Select(node => node.InnerText);

            examples.AddRange(withoutCitation);
            examples.AddRange(withCitation);
            return examples;
        }

        private static IList<string> ParseDefinitionSectionSynonyms(HtmlNode rawDefinitionSection) {
            var synonyms = rawDefinitionSection.QuerySelectorAll("dl > dd")
                                               ?.Where(elem => elem.InnerText.StartsWith("Synonym"))
                                               ?.SelectMany(elem => {
                                                   //remove string "Synonym(s): "
                                                   int startIndex = elem.InnerText.IndexOf(' ') + 1;
                                                   return elem.InnerText[startIndex..].Split(", ");
                                               })
                                               .ToList();

            return synonyms ?? new List<string>();
        }

        private static IList<string> ParseDefinitionSectionAntonyms(HtmlNode rawDefinitionSection) {
            var antonyms = rawDefinitionSection.QuerySelectorAll("dl > dd")
                                       ?.Where(elem => elem.InnerText.StartsWith("Antonym"))
                                       ?.SelectMany(node => {
                                           //remove string "Antonym(s): "
                                           int startIndex = node.InnerText.IndexOf(' ') + 1;
                                           return node.InnerText[startIndex..].Split(", ");
                                       })
                                       .ToList();

            return antonyms ?? new List<string>();
        }
    }
}