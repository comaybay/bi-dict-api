using bi_dict_api.Models;
using bi_dict_api.Others.DefinitionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others {

    public static class DefinitionParserFactory {

        public static IDefinitionParser Create(string language) {
            return language switch {
                "EN" => new DefinitionParserEN(),
                "VN" => new DefinitionParserEN(),
                "JP" => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}