using bi_dict_api.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class EtymologyParserBase : IEtymologyParser {
        protected EtymologyParserOptions Config { get; set; }

        public IList<EtymologySection> Parse(HtmlNode languageSection) {
            var rawEtymologySections = languageSection.QuerySelectorAll($"section > [id^='{Config.EtymologySectionId}']")
                                               .Select(elem => elem.ParentNode);
            var EtymologySections = new List<EtymologySection>();

            if (rawEtymologySections.Count() > 1) {
                //Normal Case: Multiple EtymologySections (definitions are inside of EtymologySection sections)
                EtymologySections.AddRange(rawEtymologySections.Select(rawEtymologySection => ParseEtymologySection(rawEtymologySection))
                                                 .ToList());
            }
            else {
                //Special Case where there is only one EtymologySection (definitions might be outside of EtymologySection section or no EtymologySection section at all)
                //Example: https://en.wiktionary.org/api/rest_v1/page/html/person (definitions are outside)
                //         https://en.wiktionary.org/api/rest_v1/page/html/ma     (definitions are not outside)
                var EtymologySection = ParseEtymologySectionSpecialCase(languageSection);
                bool isEmpty = EtymologySection.Pronunciations.Count == 0 && EtymologySection.InnerSections.Count == 0;
                if (!isEmpty)
                    EtymologySections.Add(EtymologySection);
            }

            return EtymologySections;
        }

        protected virtual EtymologySection ParseEtymologySectionSpecialCase(HtmlNode languageSection) {
            var rawEtymologySection = languageSection.QuerySelector($"section > [id^='{Config.EtymologySectionId}']")?.ParentNode;
            return new EtymologySection {
                Etymology = rawEtymologySection == null ? null : ParseEtymologyText(rawEtymologySection),
                Pronunciations = new List<string>(), //no need since these are in globalPronunciations
                InnerSections = ParseEtymologyInnerSections(languageSection)
            };
        }

        protected virtual EtymologySection ParseEtymologySection(HtmlNode rawEtymologySection) => new EtymologySection {
            Etymology = ParseEtymologyText(rawEtymologySection),
            Pronunciations = ParseEtymologySectionPronunciations(rawEtymologySection),
            InnerSections = ParseEtymologyInnerSections(rawEtymologySection),
        };

        protected virtual string ParseEtymologyText(HtmlNode rawEtymologySection) {
            var pTexts = rawEtymologySection.Elements("p")?.Select(elem => elem.InnerText);
            return pTexts.Count() switch {
                0 => null,
                1 => pTexts.First(),
                _ => pTexts.Aggregate((acc, p) => acc + "\n" + p)
            };
        }

        protected virtual IList<string> ParseEtymologySectionPronunciations(HtmlNode rawEtymologySection) {
            var pronunciationSection = rawEtymologySection.QuerySelector($"section > [id^='{Config.EtymologyPronunciationId}']")
                                                   ?.ParentNode;
            if (pronunciationSection is null)
                return new List<string>();
            else
                return Config.Helper.ParsePronunciationFrom(pronunciationSection);
        }

        protected virtual IList<EtymologyInnerSection> ParseEtymologyInnerSections(HtmlNode rawEtymologySection) {
            var innerSections = rawEtymologySection.Elements("section")
                                            ?.Where(rawSection => {
                                                //select first elem text
                                                var sectionTitle = rawSection.QuerySelector("*")?.InnerText;
                                                //if section title contains one of these words
                                                return Config.DefinitionSectionFilter.Contains(sectionTitle ?? "NULL");
                                            })
                                            ?.Select(rawSection => ParseEtymologyInnerSection(rawSection))
                                            .ToList();

            return innerSections ?? new List<EtymologyInnerSection>();
        }

        protected virtual EtymologyInnerSection ParseEtymologyInnerSection(HtmlNode rawSection) => new EtymologyInnerSection() {
            PartOfSpeech = rawSection.QuerySelector("*")?.InnerText,
            Inflection = rawSection.SelectSingleNode("p")?.InnerText,
            DefinitionSections = ParseDefinitionSections(rawSection),
            Synonyms = ParseInnerSectionSynonymSection(rawSection),
            Antonyms = ParseInnerSectionAntonymSection(rawSection),
        };

        protected virtual IList<string> ParseInnerSectionSynonymSection(HtmlNode rawSection) {
            var synonyms = rawSection.QuerySelector($"section > [id^='{Config.InnerSectionSynonymId}']")
                                     ?.ParentNode
                                     ?.QuerySelectorAll("ul > li")
                                     .Select(elem => elem.InnerText)
                                     .ToList();

            return synonyms ?? new List<string>();
        }

        protected virtual IList<string> ParseInnerSectionAntonymSection(HtmlNode rawEtymologySection) {
            var antonyms = rawEtymologySection.QuerySelector($"section > [id^='{Config.InnerSectionAntonymId}']")
                                       ?.ParentNode
                                       ?.QuerySelectorAll("ul > li")
                                       .Select(elem => elem.InnerText)
                                       .ToList();

            return antonyms ?? new List<string>();
        }

        protected virtual IList<DefinitionSection> ParseDefinitionSections(HtmlNode rawSection) {
            var definitionSections = rawSection.ParentNode
                                               ?.QuerySelectorAll($"[id='{rawSection.Id}'] > ol > li")
                                               .Select(rawDefinitionSection => ParseDefinitionSection(rawDefinitionSection))
                                               .ToList();

            return definitionSections ?? new List<DefinitionSection>();
        }

        protected virtual DefinitionSection ParseDefinitionSection(HtmlNode rawDefinitionSection) {
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

        protected virtual string ParseDefinitionSectionDefinition(HtmlNode rawDefinitionSection) {
            string definition = rawDefinitionSection.InnerText; //definition text, includes examples if they exist
            int definitionLength = definition.IndexOf('\n');    //definition length will be -1 if there are no examples

            return definitionLength == -1 ? definition : definition.Substring(0, definitionLength);
        }

        protected virtual List<string> ParseExamples(HtmlNode rawDefinitionSection) {
            var examples = new List<string>();

            bool isElementContainsExamples(HtmlNode elem) {
                var text = elem.InnerText;
                return text.Length != 0 && //some elements in a wiki have no text (even though they should have).
                       !elem.InnerText.StartsWith(Config.SynonymTextQuery) &&
                       !elem.InnerText.StartsWith(Config.AntonymTextQuery);
            }

            //examples without citation
            var withoutCitation = rawDefinitionSection.ParentNode
                                                      ?.QuerySelectorAll($"[id='{rawDefinitionSection.Id}'] > dl > dd")
                                                      .Where(node => isElementContainsExamples(node))
                                                      .Select(node => node.InnerText);

            //exaples with citation
            var withCitation = rawDefinitionSection.ParentNode
                                                   ?.QuerySelectorAll($"[id='{rawDefinitionSection.Id}'] > ul > li dl > dd")
                                                   .Where(node => isElementContainsExamples(node))
                                                   .Select(node => node.InnerText);

            examples.AddRange(withoutCitation);
            examples.AddRange(withCitation);
            return examples;
        }

        protected virtual IList<string> ParseDefinitionSectionSynonyms(HtmlNode rawDefinitionSection) {
            return rawDefinitionSection.QuerySelectorAll("dl > dd")
                                                .Where(elem => elem.InnerText.StartsWith(Config.SynonymTextQuery))
                                                .SelectMany(elem => {
                                                    //remove string "Synonym(s): "
                                                    int startIndex = elem.InnerText.IndexOf(' ') + 1;
                                                    return elem.InnerText[startIndex..].Split(", ");
                                                })
                                                .ToList();
        }

        protected virtual IList<string> ParseDefinitionSectionAntonyms(HtmlNode rawDefinitionSection) {
            return rawDefinitionSection.QuerySelectorAll("dl > dd")
                                       .Where(elem => elem.InnerText.StartsWith(Config.AntonymTextQuery))
                                       .SelectMany(node => {
                                           //remove string "Antonym(s): "
                                           int startIndex = node.InnerText.IndexOf(' ') + 1;
                                           return node.InnerText[startIndex..].Split(", ");
                                       })
                                       .ToList();
        }
    }
}