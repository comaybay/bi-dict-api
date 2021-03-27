using System;
using System.Net.Http;

namespace bi_dict_api.Utils.DefinitionProvider
{
    using bi_dict_api.Utils.DefinitionProvider.Lexico;
    using bi_dict_api.Utils.DefinitionProvider.TratuSoha;
    using bi_dict_api.Utils.DefinitionProvider.Wiktionary;
    using System.Collections.Generic;
    using WiktionaryParser.EN;
    using WiktionaryParser.VN;

    public class DefinitionProviderFactory
    {
        public static IDefinitionProvider Create(IHttpClientFactory clientFactory, string definitionLanguage, string wordLanguage)
            => (definitionLanguage, wordLanguage) switch
            {
                ("en", "en") => new DefinitionProviderGroup(EnToENGroup(clientFactory)),
                ("en", "vi") => new DefinitionProviderGroup(VIToENGroup(clientFactory)),
                ("vi", "en") => new DefinitionProviderGroup(EnToVIGroup(clientFactory)),
                ("vi", "vi") => new DefinitionProviderGroup(VIToVIGroup(clientFactory)),
                (_, _) => throw new NotImplementedException(),
            };

        private static IEnumerable<IDefinitionProvider> EnToENGroup(IHttpClientFactory clientFactory)
            => new IDefinitionProvider[] {
                    //new DefinitionProviderLexico(new LexicoParserENToEN()),
                    new DefinitionProviderWiki(clientFactory, new WikiParserEN("en")),
                    //new DefinitionProviderTratuSoha("en_en", new TratuSohaParserENToEN()),
            };
        private static IEnumerable<IDefinitionProvider> EnToVIGroup(IHttpClientFactory clientFactory)
            => new IDefinitionProvider[] {
                    new DefinitionProviderWiki(clientFactory, new WikiParserVN("en")),
                    new DefinitionProviderTratuSoha("en_vn", new TratuSohaParserENToVN()),
            };
        private static IEnumerable<IDefinitionProvider> VIToENGroup(IHttpClientFactory clientFactory)
            => new IDefinitionProvider[] {
                    new DefinitionProviderWiki(clientFactory, new WikiParserEN("vi")),
                    new DefinitionProviderTratuSoha("vn_en", new TratuSohaParserVNToEN()),
            };
        private static IEnumerable<IDefinitionProvider> VIToVIGroup(IHttpClientFactory clientFactory)
            => new IDefinitionProvider[] {
                    new DefinitionProviderWiki(clientFactory, new WikiParserVN("vi")),
                    new DefinitionProviderTratuSoha("vn_vn", new TratuSohaParserDefault("vi", "vi")),
    };
    }
}