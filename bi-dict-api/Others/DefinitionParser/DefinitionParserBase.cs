using bi_dict_api.Models;
using bi_dict_api.Models.DefinitionEN;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class DefinitionParserBase : IDefinitionParser {
        private HtmlDocument Document { get; set; }
        private HtmlNode LanguageSection { get; set; }

        protected DefinitionParserOptions Config { get; set; }

        public Definition ParseFromWikitionaryHtml(string html) {
            Initialization(html, Config.WordLanguage);

            if (LanguageSection is null)
                return null;

            var definition = new Definition {
                Word = ParseNameOfWord(),
                WordLanguage = Config.WordLanguage,
                DefinitionLanguage = Config.DefinitionLanguage,
                GlobalPronunciations = ParseGlobalPronunciation(),
                Etymologys = Config.EtymologyParser.Parse(LanguageSection),
            };

            return definition;
        }

        private void Initialization(string html, string wordLanguage) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            var id = Config.Helper.GetLanguageSectionId(wordLanguage);
            LanguageSection = Document.DocumentNode.QuerySelector($"section > [id='{id}']")?.ParentNode;
        }

        private string ParseNameOfWord() => Document.DocumentNode.SelectSingleNode("//head/title").InnerText;

        private IList<string> ParseGlobalPronunciation() {
            //check wheter such section exists
            var globalaPronunciationSection = LanguageSection.QuerySelector(Config.GlobalPronunciationQuery)
                                                             ?.ParentNode;

            if (globalaPronunciationSection is null)
                return new List<string>();
            else
                return Config.Helper.ParsePronunciationFrom(globalaPronunciationSection);
        }
    }
}