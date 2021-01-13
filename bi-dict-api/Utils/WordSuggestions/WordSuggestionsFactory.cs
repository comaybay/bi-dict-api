using System;
using System.Net.Http;

namespace bi_dict_api.Utils.WordSuggestions
{
    public class WordSuggestionsFactory
    {
        static public IWordSuggestions Create(IHttpClientFactory clientFactory, string wordLanguage)
            => wordLanguage switch
            {
                "vi" => new WordSuggestionsTratuSoha(clientFactory, "vn_vn"),
                "ja" => new WordSuggestionsJdict(clientFactory),

                _ => throw new NotImplementedException($"language \"{wordLanguage}\" not implemented or invalid"),
            };
    }
}