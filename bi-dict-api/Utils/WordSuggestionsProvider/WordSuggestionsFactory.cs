namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using System;
    using System.Net.Http;

    //NOTE: definition language is Vietnamese.
    public class WordSuggestionsFactory
    {
        static public IWordSuggestions Create(IHttpClientFactory clientFactory, string wordLanguage)
            => wordLanguage switch
            {
                "vi" => new WordSuggestionsTratuSoha(clientFactory, "vn_vn"),
                "en" => new WordSuggestionsTratuSoha(clientFactory, "en_vn"),
                "ja" => new WordSuggestionsJdict(clientFactory),
                _ => throw new NotImplementedException($"language \"{wordLanguage}\" not implemented or invalid"),
            };
    }
}