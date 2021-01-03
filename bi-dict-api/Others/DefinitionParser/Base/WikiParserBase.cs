using bi_dict_api.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class WikiParserBase : IWikiParser {
        private HtmlDocument Document { get; set; }
        private HtmlNode LanguageSection { get; set; }
        protected IWikiEtymologyParser EtymologyParser { get; init; }
        protected IWikiParserHelper Helper { get; init; }
        protected WikiParserOptions Config { get; init; }

        public Definition Parse(string html, string wordLanguage) {
            Initialization(html, wordLanguage);
            if (LanguageSection is null)
                return null;

            var definition = new Definition {
                Word = ParseNameOfWord(),
                WordLanguage = wordLanguage,
                DefinitionLanguage = Config.DefinitionLanguage,
                GlobalPronunciations = ParseGlobalPronunciation(),
                Etymologies = EtymologyParser.Parse(LanguageSection),
            };

            return definition;
        }

        private void Initialization(string html, string wordLanguage) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            var id = Helper.GetLanguageSectionId(wordLanguage);
            LanguageSection = Document.DocumentNode.QuerySelector($"section > [id='{id}']")?.ParentNode;
        }

        private string ParseNameOfWord() => Document.DocumentNode.SelectSingleNode("//head/title").InnerText;

        protected IList<string> ParseGlobalPronunciation() {
            //check wheter such section exists
            var globalaPronunciationSection = LanguageSection.QuerySelector($"section > [id^='{Config.GlobalPronunciationId}']")
                                                             ?.ParentNode;

            if (globalaPronunciationSection is null)
                return new List<string>();
            else
                return Helper.ParsePronunciationFrom(globalaPronunciationSection);
        }
    }
}