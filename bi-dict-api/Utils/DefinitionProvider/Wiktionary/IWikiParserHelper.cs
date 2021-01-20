namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser
{
    using HtmlAgilityPack;
    using System.Collections.Generic;

    public interface IWikiParserHelper
    {

        public IEnumerable<string> ParsePronunciationFrom(HtmlNode PronunciationSection);

        public string GetLanguageSectionId(string language);

        public string RemoveCiteNotes(string text);

        public IEnumerable<HtmlNode> QuerySelectorAllDirectChildren(HtmlNode elem, string query);

        public HtmlNode QuerySelectorDirectChildren(HtmlNode elem, string query);
    }
}