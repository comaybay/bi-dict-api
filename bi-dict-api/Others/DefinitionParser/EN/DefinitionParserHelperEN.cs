using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    internal class DefinitionParserHelperEN : DefinitionParserHelperBase {

        public override string GetLanguageSectionId(string language) {
            return language switch {
                "EN" => "English",
                "VN" => "Vietnamese",
                "JP" => "Japanese",
                _ => throw new ArgumentException("not implemented or unkown language", nameof(language))
            };
        }
    }
}