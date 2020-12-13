using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class EtymologyInnerSectionEN {
        public string PartOfSpeech { get; set; }
        public string Inflection { get; set; }
        public IList<string> Synonyms { get; set; }
        public IList<string> Antonyms { get; set; }
        public IList<DefinitionSectionEN> DefinitionSections { get; set; }
    }
}