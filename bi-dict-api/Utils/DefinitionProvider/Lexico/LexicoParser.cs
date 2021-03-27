using bi_dict_api.Models;
using bi_dict_api.Utils.Extensions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bi_dict_api.Utils.DefinitionProvider.Lexico
{
    public abstract class LexicoParser
    {
        protected LexicoParserOptions Config { get; set; } = default!;

        //example: https://www.lexico.com/en/definition/go
        public Definition Parse(HtmlNode doc)
        {
            var entryWrapper = GetEntryWrapper(doc);
            IEnumerable<HtmlNode> entryHeads = GetEntryHeads(entryWrapper);

            return new Definition()
            {
                SourceLink = "https://www.lexico.com",
                SourceName = "Lexico",
                WordLanguage = Config.WordLanguage,
                DefinitionLanguage = Config.DefinitionLanguage,
                GlobalPronunciations = Array.Empty<string>(),
                Word = ParseWord(entryWrapper),
                Etymologies = entryHeads.Select(entryHead => ParseEtymology(entryHead)),
            };
        }

        private static string ParseWord(HtmlNode entryWrapper)
            => entryWrapper.QuerySelector(".entryHead > header > h2 > span")
                           ?.Attributes["data-headword-id"]
                           ?.Value
                           ?? "";

        private static IEnumerable<HtmlNode> GetEntryHeads(HtmlNode entryWrapper)
        {
            var entryHeads = entryWrapper.QuerySelectorAll("div[class~='entryHead']");
            if (entryHeads.Any())
                return entryHeads;
            else
                throw new DefinitionException("entryHead elements not found");
        }

        private static HtmlNode GetEntryWrapper(HtmlNode doc)
            => doc.QuerySelector("div[class='entryWrapper']") ??
            throw new DefinitionException("entryWrapper element not found");

        /* Parses information through entryHead element and it's siblings that are before another entryHead element
         * sibling is also an entryHead element
         */
        private Etymology ParseEtymology(HtmlNode entryHead)
        {
            var pronunciations = ParsePronunciations(entryHead);

            var innerSections = new List<EtymologyInnerSection>();

            string origin = "";
            for (var sibling = entryHead.NextSibling; sibling != null; sibling = sibling.NextSibling)
            {
                if (!sibling.IsElement())
                    continue;

                else if (sibling.HasClass("entryHead"))
                    break;

                else if (sibling.HasClass("gramb"))
                    innerSections.Add(ParseInnerSection(sibling));

                else if (sibling.QuerySelector("h3 > strong")?.InnerText == "Origin")
                    origin = sibling.QuerySelector("div > p")?.InnerText ?? "";

                else if (sibling.HasClass("etymology"))
                    innerSections.Add(ParsePhrasesSection(sibling));
            }

            return new Etymology()
            {
                InnerSections = innerSections,
                Pronunciations = pronunciations,
                Origin = new string[] { origin },
                Audio = ParseAudio(entryHead),
            };
        }

        private static string ParseAudio(HtmlNode entryHead)
            => entryHead.QuerySelector("a[class='speaker'] > audio")
                        ?.Attributes["src"]
                        ?.Value
                        ?? "";

        private static IEnumerable<string> ParsePronunciations(HtmlNode entryHead)
            => entryHead.QuerySelector("h3[class='pronunciations']")
                ?.QuerySelectorAll("span[class='phoneticspelling']")
                .Where(span => span.InnerText != "")
                .Select(span => span.InnerText)
                ?? Array.Empty<string>();

        private EtymologyInnerSection ParseInnerSection(HtmlNode gramb)
        {
            return new EtymologyInnerSection()
            {
                Inflection = ParseInflection(gramb),
                PartOfSpeech = ParsePartOfSpeech(gramb),
                Senses = GetSenses(gramb),
                Synonyms = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
            };
        }

        //examples: parse Phrases section, Phrasal verbs section, ... (except Usage section)
        private EtymologyInnerSection ParsePhrasesSection(HtmlNode etym)
        {
            var strongs = etym.QuerySelectorAllDirect("div[class='senseInnerWrapper'] > ul > strong[class='phrase']");
            return new EtymologyInnerSection()
            {
                PartOfSpeech = etym.QuerySelector("h3[class='phrases-title'] > strong")?.InnerText ?? "",
                Senses = strongs.Select(s => ParsePhrasesSectionSenses(s)),
                Antonyms = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
                Inflection = "",
            };
        }

        private static string ParseInflection(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos-inflections']")?.InnerText ?? "";

        private static string ParsePartOfSpeech(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos']")?.InnerText ?? "";

        private IEnumerable<Sense> GetSenses(HtmlNode gramb)
        {
            if (GetEmptySense(gramb) != null)
                return new Sense[] { ParseSenseCrossReferenceNoExamples(gramb) };

            var trgs = gramb.QuerySelectorAll("ul[class='semb'] > li > div[class='trg']");
            return trgs.Select(trg =>
            {
                if (ParseMeaningCrossReference(trg) != "")
                    return ParseSenseCrossReference(gramb);

                return ParseSense(trg);
            });
        }

        //argument sense: is a div element wih class='trg'
        private Sense ParseSense(HtmlNode sense)
        {
            var subsenses = GetSubsenses(sense);
            var container = sense.QuerySelectorDirect("p");
            return new Sense()
            {
                Meaning = ParseMeaning(container),
                Examples = ParseExamples(sense),
                GrammaticalNote = ParseGrammaticalNote(container),
                SenseRegisters = ParseSenseRegisters(container),
                Region = ParseRegion(container),
                Antonyms = Array.Empty<string>(), //lexico's definitions do not contain antonyms
                Synonyms = ParseSynonyms(sense),
                SubSenses = subsenses.Select(s => ParseSubSense(s)),
            };
        }
        private static Sense ParseSenseCrossReference(HtmlNode gramb)
        {
            var trg = gramb.QuerySelector("ul[class='semb'] > li > div[class='trg']");
            return new Sense()
            {
                Meaning = ParseMeaningCrossReference(trg),
                Examples = ParseExamples(trg),
                GrammaticalNote = ParseGrammaticalNote(gramb),
                SenseRegisters = ParseSenseRegisters(gramb),
                Region = ParseRegion(gramb),
                Synonyms = ParseSynonyms(trg),
                Antonyms = Array.Empty<string>(),
                SubSenses = Array.Empty<Sense>(),
            };
        }
        private static Sense ParseSenseCrossReferenceNoExamples(HtmlNode gramb)
        {
            var emptySense = GetEmptySense(gramb)!;
            return new Sense()
            {
                Meaning = ParseMeaningCrossReference(emptySense),
                Region = ParseRegion(gramb),
                GrammaticalNote = ParseGrammaticalNote(gramb),
                SenseRegisters = ParseSenseRegisters(gramb),
                Examples = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
                SubSenses = Array.Empty<Sense>(),
            };
        }
        private static HtmlNode? GetEmptySense(HtmlNode gramb)
            => gramb.QuerySelector("div[class='empty_sense']");

        private static IEnumerable<HtmlNode> GetSubsenses(HtmlNode sense)
           => sense.QuerySelectorAllDirect("ol[class~='subSenses'] > li[class='subSense']");

        private static string ParseSenseRegisters(HtmlNode container)
            => container.QuerySelectorDirect("span[class~='sense-registers']")?.InnerText.Trim() ?? "";

        private static string ParseRegion(HtmlNode container)
            => container.QuerySelectorDirect("span[class~='sense-regions']")?.InnerText.Trim() ?? "";

        private static string ParseGrammaticalNote(HtmlNode container)
            => container.QuerySelectorDirect("span[class~='grammatical_note']")?.InnerText ?? "";

        private static string ParseMeaning(HtmlNode container)
            => container.QuerySelector("span[class='ind']")
                        ?.InnerText
                        ?? "";

        private static string ParseMeaningCrossReference(HtmlNode sense)
            => sense.QuerySelectorDirect("div[class='crossReference']")?.InnerText ?? "";

        private static IEnumerable<string> ParseExamples(HtmlNode sense)
        {
            var exampleElem = sense.QuerySelectorDirect("div[class='exg'] > div > em");
            var otherExampleElems = sense.QuerySelectorAllDirect("div[class='examples'] > div > ul > li > em");

            if (exampleElem != null)
                otherExampleElems = otherExampleElems.Prepend(exampleElem);

            return otherExampleElems.Select(em => em.InnerText[7..^7]); //remove &lsquo; and &rsquo; (‘ and ’)
        }

        private static IEnumerable<string> ParseSynonyms(HtmlNode sense)
            => sense.QuerySelectorDirect("div[class='synonyms'] > div[class='exg'] > div")
                       ?.InnerText
                       .Replace("&#39;", "'")
                       .Split(", ")
                       ?? Array.Empty<string>();

        private Sense ParseSubSense(HtmlNode subsense)
        {
            var childSubsenses = GetSubsenses(subsense);
            return new Sense()
            {
                Meaning = ParseMeaning(subsense),
                GrammaticalNote = ParseGrammaticalNote(subsense),
                SenseRegisters = ParseSenseRegisters(subsense),
                Region = ParseRegion(subsense),
                Examples = ParseExamples(subsense),
                Synonyms = ParseSynonyms(subsense),
                Antonyms = Array.Empty<string>(),
                SubSenses = childSubsenses.Select(s => ParseSubSense(s)),
            };
        }

        private Sense ParsePhrasesSectionSenses(HtmlNode strong)
        {
            string region = "";
            string registers = "";
            string note = "";
            IEnumerable<HtmlNode> subsenses = Array.Empty<HtmlNode>();
            for (var sibling = strong.NextSibling; sibling != null; sibling = sibling.NextSibling)
            {
                if (!sibling.IsElement())
                    continue;

                else if (sibling.HasClass("phrase"))
                    break;

                else if (sibling.HasClass("sense-regions"))
                    region = sibling.InnerText.Trim();

                else if (sibling.HasClass("sense-registers"))
                    registers = sibling.InnerText.Trim();

                else if (sibling.HasClass("grammatical_note"))
                    note = sibling.InnerText;

                else if (sibling.HasClass("semb"))
                    subsenses = sibling.QuerySelectorAllDirect("li > div[class='trg']");
            }

            return new Sense()
            {
                Meaning = strong.InnerText,
                Region = region,
                SenseRegisters = registers,
                GrammaticalNote = note,
                SubSenses = subsenses.Select(s => ParseSense(s)),
                Examples = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
            };
        }
    }
}