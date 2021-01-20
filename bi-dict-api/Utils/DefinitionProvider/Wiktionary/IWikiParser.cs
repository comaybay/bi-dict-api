namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser
{
    using Models;

    public interface IWikiParser
    {
        public string PageLanguage { get; }

        public Definition Parse(string html);
    }
}