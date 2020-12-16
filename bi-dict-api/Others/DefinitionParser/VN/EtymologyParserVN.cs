using bi_dict_api.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bi_dict_api.Others.DefinitionParser.VN {

    internal class EtymologyParserVN : EtymologyParserBase {

        public EtymologyParserVN() {
            Config = new EtymologyParserOptions() {
                DefinitionSectionFilter = "Danh từ Động từ Nội động từ Tính từ Số từ Lượng từ Phó từ Đại từ Chỉ từ "
                                          + "Trợ từ Thán từ Tình thái từ Quan hệ từ Giới từ Thành ngữ Tục ngữ"
                                          + "Mạo từ hạn định Liên từ Danh từ riêng",
                EtymologySectionQuery = "section > [id^='Từ_nguyên']",
                EtymologyPronunciationQuery = "section > [id^='Cách_phát_âm']",
                InnerSectionAntonymQuery = "section > [id='Trái_nghĩa']",
                InnerSectionSynonymQuery = "section > [id='Đồng_nghĩa']",
                AntonymTextQuery = "Trái nghĩa",
                SynonymTextQuery = "Đồng nghĩa",
                Helper = new DefinitionParserHelperVN(),
            };
        }

        protected override string ParseEtymologyText(HtmlNode rawEtymology) {
            var text = base.ParseEtymologyText(rawEtymology);
            if (!String.IsNullOrEmpty(text))
                return text;

            var pTexts = rawEtymology.QuerySelectorAll("dl > dd").Select(elem => elem.InnerText);
            return pTexts.Count() switch {
                0 => null,
                1 => pTexts.First(),
                _ => pTexts.Aggregate((acc, p) => acc + "\n" + p)
            };
        }

        protected override IList<EtymologyInnerSection> ParseEtymologyInnerSections(HtmlNode rawEtymology) {
            var rawInnerSections = rawEtymology.Elements("section");
            rawInnerSections = rawInnerSections.Concat(rawInnerSections.SelectMany(elem => elem.Elements("section")));

            var innerSections = rawInnerSections?.Where(rawSection => {
                //select first elem text
                var sectionTitle = rawSection.QuerySelector("*")?.InnerText;
                //if section title contains one of these words
                return Config.DefinitionSectionFilter.Contains(sectionTitle ?? "NULL");
            })
                                                ?.Select(rawSection => ParseEtymologyInnerSection(rawSection))
                                                .ToList();

            return innerSections ?? new List<EtymologyInnerSection>();
        }

        protected override IList<DefinitionSection> ParseDefinitionSections(HtmlNode rawSection) {
            var definitionSections = base.ParseDefinitionSections(rawSection);
            var others = rawSection.ParentNode
                                    ?.QuerySelectorAll($"[id='{rawSection.Id}'] > ul > li")
                                    .Select(rawDefinitionSection => ParseDefinitionSection(rawDefinitionSection));

            return definitionSections.Concat(others).ToList();
        }
    }
}