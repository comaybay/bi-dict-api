using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Models.DefinitionEN {

    public class EtymologyInnerSection {
        public string PartOfSpeech { get; set; }
        public string Inflection { get; set; }
        public IList<string> Synonyms { get; set; }
        public IList<string> Antonyms { get; set; }
        public IList<DefinitionSection> DefinitionSections { get; set; }
    }
}