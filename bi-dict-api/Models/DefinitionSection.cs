using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class DefinitionSection {
        public string Definition { get; set; }
        public IList<string> Examples { get; set; }
        public IList<string> Synonyms { get; set; }
        public IList<string> Antonyms { get; set; }

        public IList<DefinitionSection> SubDefinitions { get; set; }
    }
}