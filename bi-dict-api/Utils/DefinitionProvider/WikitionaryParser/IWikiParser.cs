using bi_dict_api.Models;

namespace bi_dict_api.Others.DefinitionParser
{

    public interface IWikiParser
    {
        public string PageLanguage { get; }

        public Definition Parse(string html);
    }
}