namespace bi_dict_api.Utils.DefinitionProvider.TratuSoha
{
    using bi_dict_api.Models;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TratuSohaParserENToVN : TratuSohaParser
    {
        public TratuSohaParserENToVN()
        {
            Config = new TratuSohaParserOptions()
            {
                DefinitionLanguage = "vi",
                WordLanguage = "en",
                IgnoreSectionsQuery = "hình thái từ"
            };
        }

        protected override IEnumerable<HtmlNode> GetRawEtymologies(HtmlNode bodyContent)
            => bodyContent.SelectNodes("div[@id='show-alter']") ?? new HtmlNodeCollection(dummyElement);

        protected override Subsense ParseDefinitionSection(HtmlNode rawDefinitionSection)
        {
            int exampleCount = rawDefinitionSection.SelectNodes("dl/dd/dl/dd")?.Count ?? 0;
            return exampleCount == 1 ? SpecialCase() : NormalCase();

            Subsense NormalCase() //Example: Danh từ section in http://tratu.soha.vn/dict/en_vn/smile
            {
                var rawDefinitionText = GetRawDefinitionText(rawDefinitionSection);
                var rawExampleSection = GetRawExampleSection(rawDefinitionSection);
                return new Subsense()
                {
                    Meaning = ParseDefinitionText(rawDefinitionText),
                    Examples = ParseExamples(rawExampleSection),
                    Antonyms = Array.Empty<string>(),
                    Synonyms = Array.Empty<string>(),
                    SubSenses = Array.Empty<Subsense>()
                };
            }

            Subsense SpecialCase() //Example: Cấu trúc trừ section in http://tratu.soha.vn/dict/en_vn/smile
            {
                var rawExampleSection = GetRawExampleSectionSpecialCase(rawDefinitionSection);
                return new Subsense()
                {
                    Meaning = ParseDefinitionTextSpecialCase(rawDefinitionSection),
                    Examples = ParseExamples(rawExampleSection),
                    Antonyms = Array.Empty<string>(),
                    Synonyms = Array.Empty<string>(),
                    SubSenses = Array.Empty<Subsense>()
                };
            }
        }

        private static HtmlNode GetRawExampleSectionSpecialCase(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.SelectSingleNode("dl/dd/dl/dd/dl") ?? dummyElement;

        private static HtmlNode GetRawExampleSection(HtmlNode rawDefinitionSection)
            => rawDefinitionSection.SelectSingleNode("dl/dd/dl") ?? dummyElement;

        private static IEnumerable<string> ParseExamples(HtmlNode rawExampleSection)
        {
            var exampleParts = rawExampleSection.Elements("dd")
                                                .Select(dd => dd.InnerText.Trim())
                                                .ToArray();
            var examples = new Stack<string>();
            for (int i = 1; i < exampleParts.Length; i += 2)
            {
                examples.Push($"{exampleParts[i - 1]} - {exampleParts[i]}");
            }
            return examples;
        }

        private static string ParseDefinitionTextSpecialCase(HtmlNode rawDefinitionSection)
        {
            string? vnText = rawDefinitionSection.SelectSingleNode("h5")?.InnerText.Trim();
            var rawDef2 = rawDefinitionSection.SelectSingleNode("dl/dd/dl/dd") ?? dummyElement;
            int enTextEnd = rawDef2.InnerText.IndexOf('\n');

            return (vnText, enTextEnd) switch
            {
                (null, -1) => "",
                (null, _) => EnText(),
                (_, _) => $"{vnText}: {EnText()}",
            };

            string EnText() => rawDef2.InnerText[..enTextEnd];
        }
    }
}
