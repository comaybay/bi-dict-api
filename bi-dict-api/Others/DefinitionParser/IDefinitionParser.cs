using bi_dict_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bi_dict_api.Others {

    public interface IDefinitionParser {

        public Definition ParseFromWikitionaryHtml(string html, string wordLanguage);
    }
}