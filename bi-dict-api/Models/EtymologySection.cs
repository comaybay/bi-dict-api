using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models {

    public class EtymologySection {
        public IEnumerable<string> EtymologyTexts { get; set; }
        public IEnumerable<string> Pronunciations { get; set; }
        public IEnumerable<EtymologyInnerSection> InnerSections { get; set; }
    }
}