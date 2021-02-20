namespace bi_dict_api.Utils.DefinitionProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DefinitionException : Exception
    {
        public DefinitionException(string message) : base(message)
        {
        }
    }
}
