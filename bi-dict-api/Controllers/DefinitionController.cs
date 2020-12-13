using bi_dict_api.Models;
using bi_dict_api.Models.DefinitionEN;
using bi_dict_api.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bi_dict_api.Controllers {

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefinitionController : ControllerBase {
        private readonly IHttpClientFactory _clientFactory;

        public DefinitionController(IHttpClientFactory clientFactory) {
            _clientFactory = clientFactory;
        }

        /// <summary>Gets definition of a given word in English.</summary>
        /// <response code="200"> Returns definition successfully</response>
        /// <response code="404">Unkown word or language</response>
        [HttpGet("EN/{word}/{wordLanguage}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DefinitionEN>> GetEN(string word, string wordLanguage) {
            var definition = await GetOrNull(word, wordLanguage, "EN");
            return DefinitionResponse(definition);
        }

        /// <summary>Gets definition of a given word in Vietnamese.</summary>
        /// <response code="200"> Returns definition successfully</response>
        /// <response code="404">Unkown word or language</response>
        [HttpGet("VN/{word}/{wordLanguage}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DefinitionEN>> GetVN(string word, string wordLanguage) {
            var definition = await GetOrNull(word, wordLanguage, "VN");
            return DefinitionResponse(definition);
        }

        private ActionResult DefinitionResponse(Definition definition) => definition is null ? NotFound() : Ok(definition);

        private async Task<Definition> GetOrNull(string word, string wordLanguage, string definitionLanguage) {
            try {
                //if definition is null => can't parse wikitionary page (page doesn't contain section of wordLanguage)
                return await Get(word, wordLanguage, definitionLanguage);
            }
            catch (ArgumentException) {
                //given wordLanguage is not implemented or unknown
                return null;
            }
        }

        private async Task<Definition> Get(string word, string wordLanguage, string definitionLanguage) {
            var response = await GetWikitionaryHTML(word, definitionLanguage);
            //word not found on wikitionary
            if (!response.IsSuccessStatusCode)
                return null;

            var html = await response.Content.ReadAsStringAsync();
            var definitionParser = DefinitionParserFactory.Create(definitionLanguage);
            Definition definition = definitionParser.ParseFromWikitionaryHtml(html, wordLanguage);
            return definition;
        }

        private async Task<HttpResponseMessage> GetWikitionaryHTML(string word, string definitionLanguage) {
            string URL = GetWikitionaryURL(word, definitionLanguage);
            var request = new HttpRequestMessage(HttpMethod.Get, URL);
            request.Headers.Add("Api-User-Agent", "thaichibao@gmail.com");

            var client = _clientFactory.CreateClient();
            return await client.SendAsync(request);
        }

        private static string GetWikitionaryURL(string word, string definitionLanguage) {
            var code = definitionLanguage switch {
                "EN" => "en",
                "VN" => "vi",
                "JP" => "ja",
                _ => "NULL"
            };

            return $"https://{code}.wiktionary.org/api/rest_v1/page/html/{word}";
        }
    }
}