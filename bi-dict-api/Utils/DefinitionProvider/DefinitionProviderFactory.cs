using System;
using System.Net.Http;

namespace bi_dict_api.Utils.DefinitionProvider
{
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
                (_, _) => throw new NotImplementedException()
            };

        private static IEnumerable<IDefinitionProvider> EnToENGroup(IHttpClientFactory clientFactory)
            => new IDefinitionProvider[] {
                    new DefinitionProviderWiki(clientFactory, new WikiParserEN("en")),
                    new DefinitionProviderTratuSoha("en_en", new TratuSohaParserENToEN()),
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
    }
}