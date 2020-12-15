using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser.VN {

    public class DefinitionParserVN : DefinitionParserBase {

        public DefinitionParserVN(string wordLanguage, string definitionLanguage) {
            Config = new DefinitionParserOptions() {
                WordLanguage = wordLanguage,
                DefinitionLanguage = definitionLanguage,
                EtymologyParser = new EtymologyParserVN(),
                GlobalPronunciationQuery = "section > [id^='Cách_phát_âm']",
                Helper = new DefinitionParserHelperVN(),
            };
        }
    }
}