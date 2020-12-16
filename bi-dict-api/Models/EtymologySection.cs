using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models {

    public class EtymologySection {
        public string Etymology { get; set; }
        public IList<string> Pronunciations { get; set; }
        public IList<EtymologyInnerSection> InnerSections { get; set; }
    }
}