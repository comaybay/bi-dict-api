namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.VN
{
    using Base;
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class WikiParserHelperVN : WikiParserHelperBase
    {
        public override IList<string> ParsePronunciationsFrom(HtmlNode pronunciationSection)
        {
            var pronunciations = base.ParsePronunciationsFrom(pronunciationSection);
            var others = pronunciationSection.QuerySelectorAll("li > span > [title='Wiktionary:IPA']")
                                             .Select(span => span.InnerText);
            return pronunciations.Concat(others).ToList();
        }

        public override string GetLanguageSectionId(string language)
        {
            return language switch
            {
                "en" => "Tiếng_Anh",
                "vi" => "Tiếng_Việt",
                "ja" => "Tiếng_Nhật",
                _ => throw LanguageIDNotImplementedException(language),
            };
        }
    }
}