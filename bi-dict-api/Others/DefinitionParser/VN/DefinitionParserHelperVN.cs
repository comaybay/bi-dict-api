using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bi_dict_api.Others.DefinitionParser.VN {

    internal class DefinitionParserHelperVN : DefinitionParserHelperBase {

        public override IList<string> ParsePronunciationFrom(HtmlNode pronunciationSection) {
            var pronunciations = base.ParsePronunciationFrom(pronunciationSection);
            var others = pronunciationSection.QuerySelectorAll("li > span > [title='Wiktionary:IPA']")
                                             .Select(span => span.InnerText);
            pronunciations.Concat(others);
            return pronunciations;
        }

        public override string GetLanguageSectionId(string language) {
            return language switch {
                "EN" => "Tiếng_Anh",
                "VN" => "Tiếng_Việt",
                "JP" => "Tiếng_Nhật",
                _ => throw new ArgumentException("not implemented or unkown language", nameof(language))
            };
        }
    }
}