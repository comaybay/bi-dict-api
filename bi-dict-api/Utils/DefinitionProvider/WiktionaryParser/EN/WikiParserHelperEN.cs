namespace bi_dict_api.Utils.DefinitionProvider.WiktionaryParser.EN
{
    using Base;

    internal class WikiParserHelperEN : WikiParserHelperBase
    {

        public override string GetLanguageSectionId(string language)
            => language switch
            {
                "en" => "English",
                "vi" => "Vietnamese",
                "ja" => "Japanese",
                _ => throw LanguageIDNotImplementedException(language),
            };
    }
}