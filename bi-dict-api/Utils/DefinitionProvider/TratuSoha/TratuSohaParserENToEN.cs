namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TratuSohaParserENToEN : TratuSohaParser
    {
        public TratuSohaParserENToEN()
        {
            Config = new TratuSohaParserOptions()
            {
                WordLanguage = "en",
                DefinitionLanguage = "en",
                IgnoreSectionsQuery = "Synonyms Antonyms"
            };
        }

        //Example: http://tratu.soha.vn/dict/en_en/Fish ; "Astrology . the constellation" => "Astrology. the constellation"
        protected override string ParseDefinitionText(HtmlNode rawDefinition)
            => rawDefinition.InnerText.Replace(" .", ".").Trim();
    }
}
