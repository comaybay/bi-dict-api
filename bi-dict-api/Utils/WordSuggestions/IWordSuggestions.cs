using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Utils.WordSuggestions {

    public interface IWordSuggestions {

        public Task<IEnumerable<WordSuggestion>> Get(string word);
    }
}