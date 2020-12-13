using HtmlAgilityPack;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser {

    public interface IDefinitionParserHelper {

        public IList<string> ParsePronunciationFrom(HtmlNode PronunciationSection);

        public string GetLanguageSectionId(string language);
    }
}