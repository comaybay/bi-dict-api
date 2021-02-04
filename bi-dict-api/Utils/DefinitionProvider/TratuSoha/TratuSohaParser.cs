﻿namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    using bi_dict_api.Models;
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class TratuSohaParser
    {
        static protected readonly HtmlNode dummyElement = new HtmlDocument().CreateElement("div");
        protected TratuSohaParserOptions Config { get; init; } = default!;

        public virtual Definition Parse(HtmlNode doc)
        {
            HtmlNode content = GetContent(doc);
            var bodyContent = content.SelectSingleNode("div[@id='bodyContent']") ?? dummyElement;
            var rawEtymologies = GetRawEtymologies(bodyContent);

            return new Definition()
            {
                Word = ParseWord(doc),
                DefinitionLanguage = Config.DefinitionLanguage,
                WordLanguage = Config.WordLanguage,
                GlobalPronunciations = ParseGlobalPronuncitaions(bodyContent),
                Etymologies = rawEtymologies.Select(raw => ParseEtymology(raw)),
            };
        }

        private static HtmlNode GetContent(HtmlNode doc)
        {
            var content = doc.QuerySelector("div[id='content']");
            if (content == null)
                throw new ArgumentNullException(nameof(content), "Unable to parse: content div not found");

            return content;
        }

        protected virtual IEnumerable<HtmlNode> GetRawEtymologies(HtmlNode bodyContent)
            => new HtmlNode[] { bodyContent };

        private EtymologySection ParseEtymology(HtmlNode rawEtymology)
        {
            var rawInnerSections = GetRawInnerSections(rawEtymology);
            return new EtymologySection()
            {
                EtymologyTexts = Array.Empty<string>(),
                Pronunciations = Array.Empty<string>(),
                InnerSections = rawInnerSections.Select(raw => ParseInnerSection(raw))
            };
        }

        protected virtual IEnumerable<HtmlNode> GetRawInnerSections(HtmlNode rawEtymology)
         => rawEtymology.SelectNodes("div[@id='content-3']")
                       ?.Where(section =>
                       {
                           var rawPartOfSpeech = GetRawPartOfSpeech(section);
                           return !Config.IgnoreSectionsQuery.Contains(ParsePartOfSpeech(rawPartOfSpeech));
                       })
                       ?? new HtmlNodeCollection(dummyElement);

        protected virtual EtymologyInnerSection ParseInnerSection(HtmlNode rawInnerSection)
        {
            var rawPartOfSpeech = GetRawPartOfSpeech(rawInnerSection);
            var rawDefintionSections = GetDefinitionSections(rawInnerSection);
            return new EtymologyInnerSection()
            {
                PartOfSpeech = ParsePartOfSpeech(rawPartOfSpeech),
                Inflection = "",
                Antonyms = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
                DefinitionSections = rawDefintionSections.Select(raw => ParseDefinitionSection(raw))
            };
        }

        protected virtual DefinitionSection ParseDefinitionSection(HtmlNode rawDefinitionSection)
        {
            var rawDefininitionText = GetRawDefinitionText(rawDefinitionSection);
            var rawExamples = GetRawExamples(rawDefinitionSection);
            return new DefinitionSection()
            {
                Definition = ParseDefinitionText(rawDefininitionText),
                Examples = rawExamples.Select(raw => ParseExample(raw)),
                Antonyms = Array.Empty<string>(),
                Synonyms = Array.Empty<string>(),
                SubDefinitions = Array.Empty<DefinitionSection>()
            };
        }

        private static IEnumerable<HtmlNode> GetRawExamples(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.SelectNodes("dl/dd") ?? new HtmlNodeCollection(dummyElement);

        protected virtual string ParseExample(HtmlNode rawExample)
            => rawExample.InnerText.Trim();

        protected virtual string ParseDefinitionText(HtmlNode rawDefinition)
            => rawDefinition.InnerText.Trim();

        protected virtual HtmlNode GetRawDefinitionText(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.SelectSingleNode("h5") ?? dummyElement;

        protected virtual IEnumerable<HtmlNode> GetDefinitionSections(HtmlNode rawInnerSection)
            => rawInnerSection.SelectNodes("div[@id='content-5']") ?? new HtmlNodeCollection(dummyElement);

        protected virtual string ParsePartOfSpeech(HtmlNode rawPartOfSpeech)
            => rawPartOfSpeech.InnerText.Trim();

        protected virtual HtmlNode GetRawPartOfSpeech(HtmlNode rawInnerSection)
            => rawInnerSection.SelectSingleNode("h3/span") ?? dummyElement;

        protected virtual string ParseWord(HtmlNode doc)
        {
            string title = doc.SelectSingleNode("//head/title")?.InnerText ?? "";
            int start = ("Nghĩa của từ ".Length);
            int end = title.IndexOf(" - Từ điển");
            return title[start..end];
        }

        protected virtual IEnumerable<string> ParseGlobalPronuncitaions(HtmlNode bodyContent)
        {
            var pronunciation = bodyContent.SelectSingleNode("div[@id='content-5']/h5/span")?.InnerText.Trim();
            return pronunciation is null ? Array.Empty<string>() : new string[] { pronunciation };
        }
    }
}
