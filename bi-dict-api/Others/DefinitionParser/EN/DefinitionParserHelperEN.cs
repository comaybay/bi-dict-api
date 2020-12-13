using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public class DefinitionParserHelperEN {

        public static IList<string> ParsePronunciationFrom(HtmlNode section) {
            return section.QuerySelectorAll("span.IPA")
                          .Select(span => span.InnerText)
                          .ToList();
        }

        public static string GetLanguageSectionId(string language) {
            return language switch {
                "EN" => "English",
                "VN" => "Vietnamese",
                _ => throw new ArgumentException("not implemented or unkown language", nameof(language))
            };
        }
    }
}