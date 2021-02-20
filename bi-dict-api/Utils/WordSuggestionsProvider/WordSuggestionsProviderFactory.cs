namespace bi_dict_api.Utils.WordSuggestionsProvider
{
    using System;
    using System.Net.Http;

    //NOTE: definition language is Vietnamese.
    public class WordSuggestionsProviderFactory
    {
        static public IWordSuggestionsProvider Create(IHttpClientFactory clientFactory, string wordLanguage)
            => wordLanguage switch
            {
                "vi" => new WordSuggestionsProviderTratuSoha(clientFactory, "vn_vn"),
                "en" => new WordSuggestionsProviderTratuSoha(clientFactory, "en_vn"),
                "ja" => new WordSuggestionsProviderJdict(clientFactory),
                _ => new WordSuggestionsProviderWiki(clientFactory, wordLanguage),
            };
    }
}