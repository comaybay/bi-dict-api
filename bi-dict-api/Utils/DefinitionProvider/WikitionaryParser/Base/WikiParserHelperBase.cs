using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace bi_dict_api.Others.DefinitionParser
{

    public abstract class WikiParserHelperBase : IWikiParserHelper
    {

        public virtual IList<string> ParsePronunciationFrom(HtmlNode pronunciationSection)
        {
            return pronunciationSection.QuerySelectorAll("ul > li")
                          .Select(li => li.QuerySelector("span.IPA")?.InnerText)
                          .Where(pronunciation => pronunciation != null
                                 && pronunciation.StartsWith('/') /*avoid rhymes*/)
                          .ToList();
        }

        public virtual string RemoveCiteNotes(string text)
        {
            return Regex.Replace(text, @"\[[0-9]+\]", String.Empty);
        }

        public abstract string GetLanguageSectionId(string language);

        public virtual IEnumerable<HtmlNode> QuerySelectorAllDirectChildren(HtmlNode elem, string query)
        {
            return elem.ParentNode.QuerySelectorAll($"[id='{elem.Id}'] > {query}");
        }

        public HtmlNode QuerySelectorDirectChildren(HtmlNode elem, string query)
        {
            return elem.ParentNode.QuerySelector($"[id='{elem.Id}'] > {query}");
        }

        protected virtual ArgumentException LanguageIDNotImplementedException(string language)
            => new ArgumentException("not implemented or unknown ISO language", nameof(language));
    }
}