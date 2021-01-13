using bi_dict_api.Models;
using System.Threading.Tasks;

namespace bi_dict_api.Others.DefinitionParser
{

    public interface IDefinitionProvider
    {

        public Task<Definition> Get(string word);
    }
}