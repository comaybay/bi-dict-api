using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class DefinitionEN : Definition {
        public IList<EtymologySectionEN> Etymologys { get; set; }
        public string WordLanguage { get; set; }
    }
}