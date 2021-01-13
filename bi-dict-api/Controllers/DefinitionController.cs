using bi_dict_api.Models;
using bi_dict_api.Utils.DefinitionProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bi_dict_api.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefinitionController : ControllerBase
    {
        private readonly IHttpClientFactory clientFactory;

        public DefinitionController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        /// <summary>Gets definition of a given word in English.</summary>
        /// <response code="200"> Returns definition successfully</response>
        /// <response code="404">Language not implemented or invalid</response>
        [HttpGet("en/{word}/{wordLanguage}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Definition>> GetEN(string word, string wordLanguage)
           => await CreateResponse("en", word, wordLanguage);

        /// <summary>Gets definition of a given word in Vietnamese.</summary>
        /// <response code="200"> Returns definition successfully</response>
        /// <response code="404">Language not implemented or invalid</response>
        [HttpGet("vi/{word}/{wordLanguage}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Definition>> GetVN(string word, string wordLanguage)
            => await CreateResponse("vi", word, wordLanguage);

        private async Task<ActionResult> CreateResponse(string definitionLanguage, string word, string wordLanguage)
        {
            try
            {
                var definition = await Get(definitionLanguage, word, wordLanguage);
                return Ok(definition);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return NotFound();
            }
        }

        private async Task<Definition> Get(string definitionLanguage, string word, string wordLanguage)
        {
            word = FormatWord(word);
            var definitionParser = DefinitionProviderFactory.Create(clientFactory, definitionLanguage, wordLanguage);
            var definition = await definitionParser.Get(word);
            return definition;
        }

        private static string FormatWord(string word) => word.Trim().ToLower().Replace(' ', '_');
    }
}