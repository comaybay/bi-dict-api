using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models {

    public class Definition {
        public string Word { get; set; }
        public string DefinitionLanguage { get; set; }
        public IList<string> GlobalPronunciations { get; set; }
    }
}