using bi_dict_api.Models.DefinitionEN;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser {

    public interface IEtymologyParser {

        public IList<EtymologySection> Parse(HtmlNode LanguageSection);
    }
}