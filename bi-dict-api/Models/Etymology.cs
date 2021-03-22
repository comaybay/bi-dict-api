using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class Etymology
    {
        public IEnumerable<string> Origin { get; set; } = default!;
        public IEnumerable<string> Pronunciations { get; set; } = default!;
        public IEnumerable<EtymologyInnerSection> InnerSections { get; set; } = default!;
    }
}