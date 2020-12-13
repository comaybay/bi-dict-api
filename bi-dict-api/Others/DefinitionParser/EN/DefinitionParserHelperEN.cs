using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public class DefinitionParserHelperEN : IDefinitionParserHelper {

        public IList<string> ParsePronunciationFrom(HtmlNode PronunciationSection) {
            return PronunciationSection.QuerySelectorAll("span.IPA")
                          .Select(span => span.InnerText)
                          .ToList();
        }

        public string GetLanguageSectionId(string language) {
            return language switch {
                "EN" => "English",
                "VN" => "Vietnamese",
                _ => throw new ArgumentException("not implemented or unkown language", nameof(language))
            };
        }
    }
}