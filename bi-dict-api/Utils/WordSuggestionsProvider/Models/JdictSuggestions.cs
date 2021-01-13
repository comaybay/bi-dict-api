namespace bi_dict_api.Utils.WordSuggestionsProvider.Models
{

    public class JdictSuggestions
    {
        public JdictSuggestion[] List { get; set; } = default!;
    }

    public class JdictSuggestion
    {
        public int Id { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string Word { get; set; } = default!;
        public string Kana { get; set; } = default!;
        public string SuggestMean { get; set; } = default!;
        public JdictType Type { get; set; } = default!;
    }

    public class JdictType
    {
        public string Name { get; set; } = default!;
        public string Tag { get; set; } = default!;
    }
}