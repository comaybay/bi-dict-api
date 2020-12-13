using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class Definition {
        public string Word { get; set; }
        public string WordLanguage { get; set; }
        public string DefinitionLanguage { get; set; }
        public IList<string> GlobalPronunciations { get; set; }
        public IList<EtymologySection> Etymologys { get; set; }
    }
}