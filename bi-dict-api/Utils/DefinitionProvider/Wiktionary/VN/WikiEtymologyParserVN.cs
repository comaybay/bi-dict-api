namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.VN
{
    using Base;
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class WikiEtymologyParserVN : WikiEtymologyParserBase
    {
        public WikiEtymologyParserVN()
        {
            Helper = new WikiParserHelperVN();
            Config = new WikiEtymologyParserOptions()
            {
                DefinitionSectionFilter = "Danh từ Động từ Nội động từ Ngoại động từ Tính từ Số từ Lượng từ Phó từ Đại từ Chỉ từ "
                                      + "Trợ từ Thán từ Tình thái từ Quan hệ từ Giới từ Thành ngữ Tục ngữ"
                                      + "Mạo từ hạn định Liên từ Danh từ riêng",
                EtymologySectionId = "Từ_nguyên",
                EtymologyPronunciationId = "Cách_phát_âm",
                InnerSectionAntonymId = "Trái_nghĩa",
                InnerSectionSynonymId = "Đồng_nghĩa",
                AntonymTextQuery = "Trái nghĩa",
                SynonymTextQuery = "Đồng nghĩa",
            };
        }

        protected override IEnumerable<HtmlNode> GetRawEtymologySections(HtmlNode languageSection)
            => new HtmlNode[] { languageSection }; //vi.wiktionary definitions only have one etymolgy

        protected override IEnumerable<HtmlNode> GetRawEtymologyTexts(HtmlNode rawEtymologySection)
            => rawEtymologySection.QuerySelector("[id='Từ_nguyên']")?.ParentNode.Elements("p") ?? Array.Empty<HtmlNode>();

        protected override IEnumerable<HtmlNode> GetRawEtymologyInnerSections(HtmlNode rawEtymologySection)
        {
            var rawInnerSections = base.GetRawEtymologyInnerSections(rawEtymologySection);
            //in vi.wikitionary, an etymology innersection can contain other innersections (Đồng nghĩa, Trái nghĩa).
            var others = rawInnerSections.SelectMany(elem => elem.Elements("section"));
            return rawInnerSections.Concat(others);
        }

        protected override IEnumerable<HtmlNode> GetRawDefinitionSections(HtmlNode rawInnerSection)
        {
            var rawDefinitionSection = base.GetRawDefinitionSections(rawInnerSection);
            var others = Helper.QuerySelectorAllDirectChildren(rawInnerSection, "ul > li");
            return rawDefinitionSection.Concat(others);
        }
    }
}