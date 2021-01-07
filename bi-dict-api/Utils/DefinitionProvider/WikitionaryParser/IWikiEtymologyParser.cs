using bi_dict_api.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser {

    public interface IWikiEtymologyParser {

        public IEnumerable<EtymologySection> Parse(HtmlNode LanguageSection);
    }
}