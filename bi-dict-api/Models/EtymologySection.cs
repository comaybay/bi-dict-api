using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class EtymologySection
    {
        public IEnumerable<string> EtymologyTexts { get; set; } = default!;
        public IEnumerable<string> Pronunciations { get; set; } = default!;
        public IEnumerable<EtymologyInnerSection> InnerSections { get; set; } = default!;
    }
}