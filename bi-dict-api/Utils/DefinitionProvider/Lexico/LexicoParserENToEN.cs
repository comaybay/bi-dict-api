namespace bi_dict_api.Utils.DefinitionProvider.Lexico
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LexicoParserENToEN : LexicoParser
    {
        private static LexicoParserOptions config = new LexicoParserOptions
        {
            DefinitionLanguage = "en",
            WordLanguage = "en",
        };

        public LexicoParserENToEN()
        {
            Config = config;
        }
    }
}
