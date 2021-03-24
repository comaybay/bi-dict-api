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

        private static IEnumerable<HtmlNode> GetEntryHeads(HtmlNode entryWrapper)
        {
            var entryHeads = entryWrapper.QuerySelectorAll("div[class~='entryHead']");
            if (entryHeads.Any())
                return entryHeads;
            else
                throw new DefinitionException("entryHead elements not found");
        }

        private HtmlNode GetEntryWrapper(HtmlNode doc)
            => doc.QuerySelector("div[class='entryWrapper']") ??
            throw new DefinitionException("entryWrapper element not found");

        /* Parses information through entryHead element and it's siblings that are before another entryHead element
         * sibling is also an entryHead element
         */
        private Etymology ParseEtymology(HtmlNode entryHead)
        {
            var pronunciations = ParsePronunciations(entryHead);

            var innerSections = new List<EtymologyInnerSection>();
            var sibling = entryHead.NextSibling;
            string origin = "";
            while (sibling != null)
            {
                if (!sibling.IsElement())
                    continue;

                if (sibling.HasClass("entryHead"))
                    break;

                if (sibling.HasClass("gramb"))
                    innerSections.Add(ParseInnerSection(sibling));

                if (sibling.QuerySelector("h3 > strong")?.InnerText == "Origin")
                    origin = sibling.QuerySelector("div > p")?.InnerText ?? "";

                sibling = sibling.NextSibling;
            }

            return new Etymology()
            {
                InnerSections = innerSections,
                Pronunciations = pronunciations,
                Origin = new string[] { origin },
            };
        }

        private EtymologyInnerSection ParseInnerSection(HtmlNode gramb)
        {
            var sense = gramb.QuerySelectorAll("ul[class='semb'] > li > div[class='trg']");
            return new EtymologyInnerSection()
            {
                Meaning = "",
                Inflection = ParseInflection(gramb),
                PartOfSpeech = ParsePartOfSpeech(gramb),
                SubSenses = sense.Select(rs => ParseSense(rs)),
                Synonyms = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
            };
        }

        private Sense ParseSense(HtmlNode sense)
        {
            IEnumerable<HtmlNode> enumerable = sense.QuerySelectorAllDirect("ol[class='subSenses'] > li[class='subSense']");
            var subsenses = enumerable;
            var container = sense.QuerySelectorDirect("p");
            //TODO: parse Subsenses
            return new Sense()
            {
                Examples = ParseExamples(sense),
                Meaning = ParseMeaning(container),
                GrammaticalNote = ParseGrammaticalNote(container),
                SenseRegisters = ParseSenseRegisters(container),
                Antonyms = Array.Empty<string>(), //lexico's definitions do not contain antonyms
                Synonyms = ParseSynonyms(sense),
                SubSenses = subsenses.Select(s => ParseSubSense(s)),
            };
        }

        private string ParseSenseRegisters(HtmlNode container)
            => container.QuerySelector("span[class='sense-registers']")?.InnerText.Trim() ?? "";

        private Sense ParseSubSense(HtmlNode subsense)
        {
            return new Sense()
            {
                Meaning = ParseMeaning(subsense),
                GrammaticalNote = ParseGrammaticalNote(subsense),
                SenseRegisters = ParseSenseRegisters(subsense),
                Examples = ParseExamples(subsense),
                Synonyms = ParseSynonyms(subsense),
                Antonyms = Array.Empty<string>(), //lexico's definitions do not contain antonyms
            };
        }

        private string ParseSubsenseMeaning(HtmlNode subsense)
            => subsense.QuerySelector("span[class='ind']")?.InnerText ?? "";

        private string ParseGrammaticalNote(HtmlNode rootSubsense)
            => rootSubsense.QuerySelector("span[class='grammatical_note']")
                        ?.InnerText ?? "";

        private string ParseMeaning(HtmlNode container)
            => container.QuerySelector("span[class='ind']")
                        ?.InnerText
                        ?? "";

        private IEnumerable<string> ParseSynonyms(HtmlNode sense)
            => sense.QuerySelector("div[class='synonyms'] > div[class='exg'] > div")
                       ?.InnerText
                       .Replace("&#39;", "'")
                       .Split(", ")
                       ?? Array.Empty<string>();

        private IEnumerable<string> ParseExamples(HtmlNode sense)
        {
            var exampleElem = sense.QuerySelector("div[class='exg'] > div > em");
            var otherExampleElems = sense.QuerySelectorAllDirect("div[class='examples'] > div > ul > li > em");

            if (exampleElem != null)
                otherExampleElems = otherExampleElems.Prepend(exampleElem);

            return otherExampleElems.Select(em => em.InnerText[7..^7]); //remove &lsquo; and &rsquo; (‘ and ’)
        }


        private string ParseInflection(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos-inflections']")?.InnerText ?? "";

        private string ParsePartOfSpeech(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos']")?.InnerText ?? "";

        private IEnumerable<string> ParsePronunciations(HtmlNode entryHead)
            => entryHead.QuerySelector("h3[class='pronunciations']")
                        ?.QuerySelectorAll("span[class='phoneticspelling']")
                        .Where(span => span.InnerText != "")
                        .Select(span => span.InnerText)
                        ?? Array.Empty<string>();

        private string ParseWord(HtmlNode entryWrapper)
            => entryWrapper.QuerySelector(".entryHead > header > h2 > span")?.Attributes["data-headword-id"].Value ?? "";
    }
}