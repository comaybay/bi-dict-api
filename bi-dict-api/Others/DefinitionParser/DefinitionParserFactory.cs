using bi_dict_api.Others.DefinitionParser;
using bi_dict_api.Others.DefinitionParser.EN;
using bi_dict_api.Others.DefinitionParser.VN;
using System;

namespace bi_dict_api.Others {

    public class DefinitionParserFactory {

        public static IDefinitionParser Create(string definitionLanguage) {
            return definitionLanguage switch {
                "EN" => DefinitionParserEN(),
                "VN" => DefinitionParserVN(),
                "JP" => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }

        private static IDefinitionParser DefinitionParserVN() {
            var wikiParser = new WikiParserVN();
            return new DefinitionParserImp(wikiParser);
        }

        private static IDefinitionParser DefinitionParserEN() {
            var wikiParser = new WikiParserEN();
            return new DefinitionParserImp(wikiParser);
        }
    }
}