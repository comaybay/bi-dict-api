using HtmlAgilityPack;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser
{

    public interface IWikiParserHelper
    {

        public IList<string> ParsePronunciationFrom(HtmlNode PronunciationSection);

        public string GetLanguageSectionId(string language);

        public string RemoveCiteNotes(string text);

        public IEnumerable<HtmlNode> QuerySelectorAllDirectChildren(HtmlNode elem, string query);

        public HtmlNode QuerySelectorDirectChildren(HtmlNode elem, string query);
    }
}