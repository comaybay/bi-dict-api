using bi_dict_api.Models;
using bi_dict_api.Models.DefinitionEN;
using bi_dict_api.Others.DefinitionParser.EN;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public class DefinitionParserEN : IDefinitionParser {
        private HtmlDocument Document { get; set; }
        private HtmlNode LanguageSection { get; set; }

        private readonly string _globalPronunciationQuery = "section > [id^='Pronunciation']";
        private EtymologyParserEN EtymologySectionParser { get; set; }

        public Definition ParseFromWikitionaryHtml(string html, string wordLanguage) {
            Initialization(html, wordLanguage);

            if (LanguageSection is null)
                return null;

            EtymologySectionParser = new EtymologyParserEN(LanguageSection);

            var definition = new DefinitionEN {
                Word = ParseNameOfWord(),
                WordLanguage = wordLanguage,
                DefinitionLanguage = "EN",
                GlobalPronunciations = ParseGlobalPronunciation(),
                Etymologys = EtymologySectionParser.Parse(),
            };

            return definition;
        }

        private void Initialization(string html, string wordLanguage) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            var id = DefinitionParserHelperEN.GetLanguageSectionId(wordLanguage);
            LanguageSection = Document.DocumentNode.QuerySelector($"section > #{id}")?.ParentNode;
        }

        private string ParseNameOfWord() => Document.DocumentNode.SelectSingleNode("//head/title").InnerText;

        private IList<string> ParseGlobalPronunciation() {
            //check wheter such section exists
            var globalaPronunciationSection = LanguageSection.QuerySelector(_globalPronunciationQuery)
                                                             ?.ParentNode;

            if (globalaPronunciationSection is null)
                return new List<string>();
            else
                return DefinitionParserHelperEN.ParsePronunciationFrom(globalaPronunciationSection);
        }
    }
}