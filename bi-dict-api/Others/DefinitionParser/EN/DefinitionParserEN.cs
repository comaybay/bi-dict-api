using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.EN {

    public class DefinitionParserEN : DefinitionParserBase {

        public DefinitionParserEN(string wordLanguage, string definitionLanguage) {
            Config = new DefinitionParserOptions() {
                DefinitionLanguage = definitionLanguage,
                EtymologyParser = new EtymologyParserEN(),
                GlobalPronunciationId = "Pronunciation",
                WordLanguage = wordLanguage,
                Helper = new DefinitionParserHelperEN(),
            };
        }
    }
}