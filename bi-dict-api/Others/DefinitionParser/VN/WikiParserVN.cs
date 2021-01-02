using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.VN {

    public class WikiParserVN : WikiParserBase {

        public WikiParserVN(WikiParserOptions config, IWikiEtymologyParser etymologyParser, IWikiParserHelper helper)
            : base(config, etymologyParser, helper) { }
    }
}