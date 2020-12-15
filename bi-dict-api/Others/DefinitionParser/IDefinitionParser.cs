using bi_dict_api.Models;

namespace bi_dict_api.Others.DefinitionParser {
    public interface IDefinitionParser {
        public Definition ParseFromWikitionaryHtml(string html);
    }
}