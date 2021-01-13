using bi_dict_api.Others.DefinitionParser;
using bi_dict_api.Others.DefinitionParser.EN;
using bi_dict_api.Others.DefinitionParser.VN;
using System;
using System.Net.Http;

namespace bi_dict_api.Others
{

    public class DefinitionProviderFactory
    {

        public static IDefinitionProvider Create(IHttpClientFactory clientFactory, string definitionLanguage, string wordLanguage)
        {
            return definitionLanguage switch
            {
                "en" => new DefinitionProviderWiki(clientFactory, new WikiParserEN(wordLanguage)),
                "vi" => new DefinitionProviderWiki(clientFactory, new WikiParserVN(wordLanguage)),
                "ja" => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}