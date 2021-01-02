using bi_dict_api.Models;
using bi_dict_api.Others.DefinitionParser;
using bi_dict_api.Others.DefinitionParser.EN;
using bi_dict_api.Others.DefinitionParser.VN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others {

    public class DefinitionParserFactory {
        private readonly string definitionLanguage;

        public DefinitionParserFactory(string definitionLanguage) {
            this.definitionLanguage = definitionLanguage;
        }

        public IDefinitionParser Create() {
            return definitionLanguage switch {
                "EN" => new DefinitionParserEN(),
                "VN" => new DefinitionParserVN(),
                "JP" => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}