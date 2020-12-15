using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser {

    public abstract class DefinitionParserHelperBase : IDefinitionParserHelper {

        public virtual IList<string> ParsePronunciationFrom(HtmlNode pronunciationSection) {
            return pronunciationSection.QuerySelectorAll("span.IPA")
                          .Select(span => span.InnerText)
                          .ToList();
        }

        public abstract string GetLanguageSectionId(string language);
    }
}