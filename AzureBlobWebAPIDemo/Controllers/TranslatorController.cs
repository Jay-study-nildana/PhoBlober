using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobWebAPIDemo.DTO;
using AzureBlobWebAPIDemo.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static Azure.Core.HttpHeader;

namespace AzureBlobWebAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslatorController : ControllerBase
    {
        private const string Prefix = "";
        protected ResponseDto _response;
        private readonly ITranslatorStuff _translatorStuff1;

        public TranslatorController(
            ITranslatorStuff translatorStuff
            )
        {
            this._response = new ResponseDto();
            this._translatorStuff1 = translatorStuff;
        }

        //// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        public class TranslationResponse
        {
            public List<Translation>? translations { get; set; }
        }

        public class Translation
        {
            public string text { get; set; } //TranslatedText
            public string to { get; set; } //Language
        }

        [HttpPost("SendTextForTranslation")]
        public async Task<ResponseDto> SendTextForTranslation(TextDTO textDTO)
        {
            int count_of_characters = textDTO.OriginalText.Length;
            int azure_translator_limit = 50000;
            int total_languages = 3;
            

            //https://learn.microsoft.com/en-us/azure/ai-services/translator/service-limits
            if (count_of_characters*total_languages> azure_translator_limit)
            {
                _response.IsSuccess = false;
                _response.Message = "text is too long. please use a shorter sentence";

                //here, can I break it up, you know, make a looping request and stitch it up together.

                return _response;
            }

            try
            {
                var translatorinfo = _translatorStuff1.GetMeTranslatorSettings();
                string key = translatorinfo.key;
                string endpoint = translatorinfo.endpoint;
                TextTranslatedDTO textTranslatedDTO = new TextTranslatedDTO();

                textTranslatedDTO.OriginalText = textDTO.OriginalText;

                //as of now, only english text is allowed as input
                //as of now, translating to kannada, hindi and french
                //note: there is a collection called, TargetLanguages, which can be used to acquire custom languages.
                //loop through it, and build the route.
                string route = "/translate?api-version=3.0&from=en&to=kn&to=hi&to=fr";
                string textToTranslate = textDTO.OriginalText;
                object[] body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();
                    try
                    {
                        List<TranslationResponse> result_object = JsonConvert.DeserializeObject<List<TranslationResponse>>(result);
                        foreach (var translation in result_object[0].translations)
                        {
                            TranslatedTextUnit translatedTextUnit = new TranslatedTextUnit();
                            translatedTextUnit.TranslatedText = translation.text;
                            translatedTextUnit.Language = translation.to;

                            textTranslatedDTO.TextUnits.Add(translatedTextUnit);
                        }
                    }
                    catch (Exception ex)
                    {
                        _response.Message = ex.Message;
                        _response.IsSuccess = false;
                        return _response;
                    }
                }

                _response.Result = textTranslatedDTO;
                _response.Message = "Text Translated Successfully";

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            return _response;
        }
    }
}
