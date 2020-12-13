using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class EtymologySection {
        public IList<string> Pronunciations { get; set; }
        public IList<EtymologyInnerSection> InnerSections { get; set; }
    }
}