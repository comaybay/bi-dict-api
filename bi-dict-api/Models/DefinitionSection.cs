using System.Collections.Generic;

namespace bi_dict_api.Models
{

    public class DefinitionSection
    {
        public string Definition { get; set; } = default!;
        public IEnumerable<string> Examples { get; set; } = default!;
        public IEnumerable<string> Synonyms { get; set; } = default!;
        public IEnumerable<string> Antonyms { get; set; } = default!;

        public IEnumerable<DefinitionSection> SubDefinitions { get; set; } = default!;
    }
}