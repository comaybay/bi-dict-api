namespace bi_dict_api.Utils.DefinitionProvider
{
    using Models;
    using System.Threading.Tasks;

    public interface IDefinitionProvider
    {

        public Task<Definition> Get(string word);
    }
}