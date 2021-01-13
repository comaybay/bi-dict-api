using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class Definition
    {
        public string Word { get; set; }
        public string WordLanguage { get; set; }
        public string DefinitionLanguage { get; set; }
        public IEnumerable<string> GlobalPronunciations { get; set; }
        public IEnumerable<EtymologySection> Etymologies { get; set; }
    }
}