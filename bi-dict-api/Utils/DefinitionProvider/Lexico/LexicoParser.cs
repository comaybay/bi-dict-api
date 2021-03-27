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

        private string ParseWord(HtmlNode entryWrapper)
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

                else if (sibling.HasClass("usage"))
                    innerSections.Add(ParseInnerSectionUsage(sibling));

                else if (sibling.HasClass("etymology"))
                    innerSections.Add(ParseInnerSectionSpecial(sibling));
            }

            return new Etymology()
            {
                InnerSections = innerSections,
                Pronunciations = pronunciations,
                Origin = new string[] { origin },
                Audio = ParseAudio(entryHead),
            };
        }

        private EtymologyInnerSection ParseInnerSectionUsage(HtmlNode usage)
            => new EtymologyInnerSection()
            {
                PartOfSpeech = "Usage",
                Inflection = usage.QuerySelector("div[class='senseInnerWrapper']")?.InnerText ?? "",
                SubSenses = Array.Empty<Sense>(),
                Synonyms = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
            };

        private string ParseAudio(HtmlNode entryHead)
            => entryHead.QuerySelector("a[class='speaker'] > audio")
                        ?.Attributes["src"]
                        ?.Value
                        ?? "";

        private IEnumerable<string> ParsePronunciations(HtmlNode entryHead)
            => entryHead.QuerySelector("h3[class='pronunciations']")
                ?.QuerySelectorAll("span[class='phoneticspelling']")
                .Where(span => span.InnerText != "")
                .Select(span => span.InnerText)
                ?? Array.Empty<string>();

        private EtymologyInnerSection ParseInnerSection(HtmlNode gramb)
        {
            var sense = gramb.QuerySelectorAll("ul[class='semb'] > li > div[class='trg']");
            return new EtymologyInnerSection()
            {
                Inflection = ParseInflection(gramb),
                PartOfSpeech = ParsePartOfSpeech(gramb),
                SubSenses = sense.Select(rs => ParseSense(rs)),
                Synonyms = Array.Empty<string>(),
                Antonyms = Array.Empty<string>(),
            };
        }

        private string ParseInflection(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos-inflections']")?.InnerText ?? "";

        private string ParsePartOfSpeech(HtmlNode gramb)
            => gramb.QuerySelector("h3 > span[class='pos']")?.InnerText ?? "";

        //argument sense: is a div element wih class='trg'
        private Sense ParseSense(HtmlNode sense)
        {
            var subsenses = getSubsenses(sense);
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

        private IEnumerable<HtmlNode> getSubsenses(HtmlNode sense)
           => sense.QuerySelectorAllDirect("ol[class='subSenses'] > li[class='subSense']");

        private string ParseSenseRegisters(HtmlNode container)
            => container.QuerySelector("span[class='sense-registers']")?.InnerText.Trim() ?? "";

        private string ParseRegion(HtmlNode container)
            => container.QuerySelector("span[class='sense-regions']")?.InnerText.Trim() ?? "";

        private string ParseGrammaticalNote(HtmlNode container)
            => container.QuerySelector("span[class='grammatical_note']")?.InnerText ?? "";

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

        private Sense ParseSubSense(HtmlNode subsense)
        {
            var childSubsenses = getSubsenses(subsense);
            return new Sense()
            {
                Meaning = ParseMeaning(subsense),
                GrammaticalNote = ParseGrammaticalNote(subsense),
                SenseRegisters = ParseSenseRegisters(subsense),
                Region = ParseRegion(subsense),
                Examples = ParseExamples(subsense),
                Synonyms = ParseSynonyms(subsense),
                Antonyms = Array.Empty<string>(), //lexico's definitions do not contain antonyms
                SubSenses = childSubsenses.Select(s => ParseSubSense(s)),
            };
        }

        //examples: parse Phrases section, Phrasal verbs section, ... (except Usage section)
        private EtymologyInnerSection ParseInnerSectionSpecial(HtmlNode etym)
        {
            var strongs = etym.QuerySelectorAllDirect("div[class='senseInnerWrapper'] > ul > strong[class='phrase']");
            return new EtymologyInnerSection()
            {
                PartOfSpeech = etym.QuerySelector("h3[class='phrases-title'] > strong")?.InnerText ?? "",
                SubSenses = strongs.Select(s => ParseSubsenseSpecial(s)),
                Antonyms = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
                Inflection = "",
            };
        }

        private Sense ParseSubsenseSpecial(HtmlNode strong)
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