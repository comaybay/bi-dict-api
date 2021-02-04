namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TratuSohaParserDefault : TratuSohaParser
    {
        public TratuSohaParserDefault(string definitionLanguage, string wordLanguage)
        {
            Config = new TratuSohaParserOptions()
            {
                DefinitionLanguage = definitionLanguage,
                WordLanguage = wordLanguage,
                IgnoreSectionsQuery = "",
            };
        }
    }
}
