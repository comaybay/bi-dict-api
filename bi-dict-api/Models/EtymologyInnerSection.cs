using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class EtymologyInnerSection
    {
        public string PartOfSpeech { get; set; }
        public string Inflection { get; set; }
        public IEnumerable<string> Synonyms { get; set; }
        public IEnumerable<string> Antonyms { get; set; }
        public IEnumerable<DefinitionSection> DefinitionSections { get; set; }
    }
}