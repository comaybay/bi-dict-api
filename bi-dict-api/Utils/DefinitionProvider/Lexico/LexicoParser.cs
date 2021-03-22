using bi_dict_api.Models;
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
            var entryHeads = entryWrapper.QuerySelectorAll("div[class*='entryHead']");
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
        private EtymologySection ParseEtymology(HtmlNode entryHead)
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

            return new EtymologySection()
            {
                InnerSections = innerSections,
                Pronunciations = pronunciations,
                EtymologyTexts = new string[] { origin },
            };
        }

        private EtymologyInnerSection ParseInnerSection(HtmlNode gramb)
        {
            //TODO: finish this shit
            return new EtymologyInnerSection()
            {
                PartOfSpeech = ParsePartOfSpeech(gramb),
                Inflection = ParseInflection(gramb),
            };
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