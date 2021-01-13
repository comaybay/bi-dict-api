using bi_dict_api.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser
{

    public abstract class WikiParserBase : IWikiParser
    {
        protected IWikiEtymologyParser EtymologyParser { get; init; }
        protected IWikiParserHelper Helper { get; init; }
        protected WikiParserOptions Config { get; init; }
        public string PageLanguage { get => Config.PageLanguage; }

        public Definition Parse(string html)
        {
            (var document, var languageSection) = Initialization(html);

            var definition = new Definition
            {
                Word = ParseNameOfWord(document),
                DefinitionLanguage = Config.PageLanguage,
                WordLanguage = Config.WordLanguage,
                GlobalPronunciations = ParseGlobalPronunciation(languageSection),
                Etymologies = EtymologyParser.Parse(languageSection),
            };

            return definition;
        }

        private (HtmlDocument, HtmlNode) Initialization(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var id = Helper.GetLanguageSectionId(Config.WordLanguage);
            var languageSection = document.DocumentNode.QuerySelector($"section > [id='{id}']")?.ParentNode;
            if (languageSection is null)
                throw new ArgumentException("Language section to find definition not found.");

            return (document, languageSection);
        }

        protected virtual string ParseNameOfWord(HtmlDocument document)
            => document.DocumentNode.SelectSingleNode("//head/title").InnerText;

        protected virtual IEnumerable<string> ParseGlobalPronunciation(HtmlNode languageSection)
        {
            //check whether such section exists
            var globalaPronunciationSection = languageSection.QuerySelector($"section > [id^='{Config.GlobalPronunciationId}']")
                                                             ?.ParentNode;

            return (globalaPronunciationSection is null)
            ? Array.Empty<string>() : Helper.ParsePronunciationFrom(globalaPronunciationSection);
        }
    }
}