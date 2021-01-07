using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Utils.WordSuggestions.Models {

    public class JdictSuggestions {
        public JdictSuggestion[] List { get; set; }
    }

    public class JdictSuggestion {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Word { get; set; }
        public string Kana { get; set; }
        public string SuggestMean { get; set; }
        public JdictType Type { get; set; }
    }

    public class JdictType {
        public string Name { get; set; }
        public string Tag { get; set; }
    }
}