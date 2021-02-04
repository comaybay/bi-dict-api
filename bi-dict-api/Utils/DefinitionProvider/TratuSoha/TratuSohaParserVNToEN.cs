using bi_dict_api.Models;
using HtmlAgilityPack;
using System;
using System.Linq;

namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    internal class TratuSohaParserVNToEN : TratuSohaParserENToVN
    {
        public TratuSohaParserVNToEN()
        {
            Config = new TratuSohaParserOptions()
            {
                DefinitionLanguage = "en",
                WordLanguage = "vi",
                IgnoreSectionsQuery = "hình thái từ"
            };
        }

        /*example: 
         *  normal:  http://tratu.soha.vn/dict/vn_en/%C4%90i
         *  unusual: http://tratu.soha.vn/dict/vn_en/C%C3%A1
         */
        public override Definition Parse(HtmlNode doc)
        {
            var definition = base.Parse(doc);
            if (!definition.Etymologies.First().InnerSections.Any())
                throw new Exception("inconsisting formating.");

            return definition;
        }
    }
}
