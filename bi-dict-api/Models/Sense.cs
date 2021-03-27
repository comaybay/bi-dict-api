using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class Sense
    {
        public string Meaning { get; set; } = default!;
        public string GrammaticalNote { get; set; } = default!;
        public string SenseRegisters { get; set; } = default!;
        public string Region { get; set; } = default!;
        public IEnumerable<string> Examples { get; set; } = default!;
        public IEnumerable<string> Synonyms { get; set; } = default!;
        public IEnumerable<string> Antonyms { get; set; } = default!;
        public IEnumerable<Sense> SubSenses { get; set; } = default!;
    }
}