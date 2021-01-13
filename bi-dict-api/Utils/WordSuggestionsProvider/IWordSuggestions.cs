namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using bi_dict_api.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWordSuggestions
    {
        public Task<IEnumerable<WordSuggestion>> Get(string word);
    }
}