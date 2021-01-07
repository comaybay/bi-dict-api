using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    internal class WikiParserHelperEN : WikiParserHelperBase {

        public override string GetLanguageSectionId(string language)
            => language switch {
                "en" => "English",
                "vi" => "Vietnamese",
                "ja" => "Japanese",
                _ => throw LanguageIDNotImplementedException(language),
            };
    }
}