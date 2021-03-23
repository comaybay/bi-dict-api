using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class Subsense
    {
        public string Meaning { get; set; } = default!;
        public string GrammaticalNote { get; set; } = default!;
        public IEnumerable<string> Examples { get; set; } = default!;
        public IEnumerable<string> Synonyms { get; set; } = default!;
        public IEnumerable<string> Antonyms { get; set; } = default!;

        public IEnumerable<Subsense> SubSenses { get; set; } = default!;
    }
}