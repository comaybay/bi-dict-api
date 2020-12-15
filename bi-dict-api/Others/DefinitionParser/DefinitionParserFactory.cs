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
        private readonly string _wordLanguage;
        private readonly string _definitionLanguage;

        public DefinitionParserFactory(string wordLanguage, string definitionLanguage) {
            _wordLanguage = wordLanguage;
            _definitionLanguage = definitionLanguage;
        }

        public DefinitionParserBase Create() {
            return _definitionLanguage switch {
                "EN" => new DefinitionParserEN(_wordLanguage, _definitionLanguage),
                "VN" => new DefinitionParserVN(_wordLanguage, _definitionLanguage),
                "JP" => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}