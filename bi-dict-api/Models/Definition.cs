using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class Definition
    {
        public string Word { get; set; } = default!;
        public string WordLanguage { get; set; } = default!;
        public string DefinitionLanguage { get; set; } = default!;
        public IEnumerable<string> GlobalPronunciations { get; set; } = default!;
        public IEnumerable<EtymologySection> Etymologies { get; set; } = default!;
        public string DefinitionSource { get; set; } = default!;
        public string DefinitionSourceLink { get; set; } = default!;
    }
}