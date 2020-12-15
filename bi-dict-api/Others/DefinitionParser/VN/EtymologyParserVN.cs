using bi_dict_api.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace bi_dict_api.Others.DefinitionParser.VN {

    internal class EtymologyParserVN : EtymologyParserBase {

        public EtymologyParserVN() {
            Config = new EtymologyParserOptions() {
                DefinitionSectionFilter = "Danh từ Động từ Nội động từ Tính từ Số từ Lượng từ Phó từ Đại từ Chỉ từ "
                                          + "Trợ từ Thán từ Tình thái từ Quan hệ từ Giới từ Thành ngữ Tục ngữ"
                                          + "Mạo từ hạn định Liên từ",
                EtymologySectionQuery = "[makeQueryReturn=NULL]",
                EtymologyPronunciationQuery = "section > [id^='Cách_phát_âm']",
                InnerSectionAntonymQuery = "section > [id='Trái_nghĩa']",
                InnerSectionSynonymQuery = "section > [id='Đồng_nghĩa']",
                AntonymTextQuery = "Trái nghĩa",
                SynonymTextQuery = "Đồng nghĩa",
                Helper = new DefinitionParserHelperVN(),
            };
        }
    }
}