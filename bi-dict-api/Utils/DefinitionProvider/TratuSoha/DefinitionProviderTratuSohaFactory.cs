using System;

namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    internal class DefinitionProviderTratuSohaFactory
    {
        public static DefinitionProviderTratuSoha Create(string definitionLanguage, string wordLanguage)
            => (definitionLanguage, wordLanguage) switch
            {
                ("en", "vi") => new DefinitionProviderTratuSoha("vn_en", new TratuSohaParserDefault(definitionLanguage, wordLanguage)),
                ("vi", "vi") => new DefinitionProviderTratuSoha("vn_vn", new TratuSohaParserDefault(definitionLanguage, wordLanguage)),
                ("vi", "en") => new DefinitionProviderTratuSoha("en_vn", new TratuSohaParserENToVN()),
                ("en", "en") => new DefinitionProviderTratuSoha("en_en", new TratuSohaParserENToEN()),
                _ => throw new NotImplementedException(),
            };
    }
}