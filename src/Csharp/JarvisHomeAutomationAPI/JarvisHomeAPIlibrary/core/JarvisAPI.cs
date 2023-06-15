using JarvisHomeAPIlibrary.Models.Request;
using JarvisHomeCloud.Server.Logic;
using JarvisMainSite.Models.JarvisAPI;
using NAudio.Utils;
using Newtonsoft.Json;
using PAD_Console_2024.Models.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JarvisHomeAPIlibrary.core
{
    public class JarvisAPI
    {
        private string baseUrl;
        private string apiKey;
        private string apiSecret;

        private HttpClient client;
        public JarvisAPI()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
        }

        public void setBaseUrl(string url)
        {
            baseUrl = url;
            client.BaseAddress = new Uri(baseUrl);
        }

        /// <summary>
        /// Set your api key that is generated on the jarvis home automation site within your account.
        /// </summary>
        /// <param name="key">api key</param>
        public void setApiKey(string key)
        {
            apiKey = key;
        }

        /// <summary>
        /// set your api secret that is generated on the jarvis home automation site within your account.
        /// </summary>
        /// <param name="secret">api secret</param>
        public void setApiSecret(string secret)
        {
            apiSecret = secret;
        }

        /// <summary>
        /// get stored api key
        /// </summary>
        /// <returns>api key</returns>
        public string getApiKey()
        {
            return apiKey;
        }

        /// <summary>
        /// get the stored api secret
        /// </summary>
        /// <returns>api secret</returns>
        public string getApiSecret()
        {
            return apiSecret;
        }

        /// <summary>
        /// Get generated code from a prompt you give to PAD with your choise of language
        /// </summary>
        /// <param name="language">lanague type (pythin, javascript...)</param>
        /// <param name="prompt">The prompt to convert to code.</param>
        /// <returns>General Respone with type mainChatResponseModel</returns>
        public GeneralResponse<mainChatResponseModel> getCodeCall(string language, string prompt)
        {
            try
            {
                GenericEncryptedRequestModel request = new GenericEncryptedRequestModel();
                request.apiKey = apiKey;

                codeRequestModel code = new codeRequestModel();
                code.language = language;
                code.prompt = prompt;

                request.payload = Cipher.Encrypt(JsonConvert.SerializeObject(code), apiSecret);
                var response = Call("jarvis/v1/GetCode", HttpMethod.Post, JsonConvert.SerializeObject(request), null).Result;
                if (response != null)
                {
                    return JsonConvert.DeserializeObject<GeneralResponse<mainChatResponseModel>>(response);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        /// <summary>
        /// Get a chat response to the prompt you give the PAD language model.
        /// </summary>
        /// <param name="primer">To setup how PAD will react to your prompt</param>
        /// <param name="prompt">The question, statement or whatever you want to send it.</param>
        /// <returns>A general response with the type mainChatResponseModel</returns>
        public GeneralResponse<mainChatResponseModel> getChatCall(string primer, string prompt)
        {
            try
            {
                GenericEncryptedRequestModel request = new GenericEncryptedRequestModel();
                request.apiKey = apiKey;

                chatRequestModelAPI code = new chatRequestModelAPI();
                code.primer = primer;
                code.prompt = prompt;

                request.payload = Cipher.Encrypt(JsonConvert.SerializeObject(code), apiSecret);
                var response = Call("jarvis/v1/Chat", HttpMethod.Post, JsonConvert.SerializeObject(request), null).Result;
                if (response != null)
                {
                    return JsonConvert.DeserializeObject<GeneralResponse<mainChatResponseModel>>(response);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        /// <summary>
        /// To convert the string prompt to audio data and store a temp file to play the audio.
        /// </summary>
        /// <param name="prompt">What you want the voice to say</param>
        public async void getTTSAndSpeak(string prompt)
        {
            try
            {
                GenericEncryptedRequestModel request = new GenericEncryptedRequestModel();
                request.apiKey = apiKey;

                chatRequestModelAPI code = new chatRequestModelAPI();
                //code.primer = primer;
                code.prompt = prompt;
                request.payload = Cipher.Encrypt(JsonConvert.SerializeObject(code), apiSecret);
                MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");

                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, contentType);

                using (HttpResponseMessage response = await client.PostAsync("/jarvis/v1/TTS", content))
                {
                    var aud_data = await response.Content.ReadAsByteArrayAsync();
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        string tempFile = Path.GetTempFileName();
                        string genFile = Environment.CurrentDirectory + "\\main.wav";
                        using (Stream streamToWriteTo = File.Open(tempFile, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }



                        PAD_Console_2024.util.Speaker.SavePcmToMp3(aud_data, 44100, 2, "main.mp3");
                        PAD_Console_2024.util.Speaker.PlayAudio(tempFile);
                    }
                }
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// Get the audo byte data from send a prompt as a string
        /// </summary>
        /// <param name="prompt">What you want the voice to say</param>
        /// <returns>Task byte[] data of the audio</returns>
        public async Task<byte[]> getTTS(string prompt)
        {
            try
            {
                GenericEncryptedRequestModel request = new GenericEncryptedRequestModel();
                request.apiKey = apiKey;

                chatRequestModelAPI code = new chatRequestModelAPI();
                //code.primer = primer;
                code.prompt = prompt;
                request.payload = Cipher.Encrypt(JsonConvert.SerializeObject(code), apiSecret);
                MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");

                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, contentType);

                using (HttpResponseMessage response = await client.PostAsync("/jarvis/v1/TTS", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var aud_data = await response.Content.ReadAsByteArrayAsync();
                        return aud_data;
                    } else
                    {
                        return null;
                    }
                    
                }
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        /// <summary>
        /// Get a sentiment rating based on the prompt you give the API call.
        /// </summary>
        /// <param name="prompt">The general phrase you want to be rated.</param>
        /// <returns>General Response with type SentimentResponseModel</returns>
        public GeneralResponse<SentimentResponseModel> getSentiment(string prompt)
        {
            try
            {
                GenericEncryptedRequestModel request = new GenericEncryptedRequestModel();
                request.apiKey = apiKey;
                SentimentRequestModel code = new SentimentRequestModel();
                code.phrase = prompt;
                request.payload = Cipher.Encrypt(JsonConvert.SerializeObject(code), apiSecret);
                var response = Call("jarvis/v1/GetSentiment", HttpMethod.Post, JsonConvert.SerializeObject(request), null).Result;
                if (response != null)
                {
                    return JsonConvert.DeserializeObject<GeneralResponse<SentimentResponseModel>>(response);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }


        private async Task<string> Call(string endpoint, HttpMethod method, string body, List<Tuple<string, string>> headers)
        {
            try
            {
                HttpResponseMessage response = null;

                if (headers != null)
                {
                    foreach (var head in headers)
                    {
                        client.DefaultRequestHeaders.Add(head.Item1, head.Item2);
                    }
                }

                MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");

                HttpContent content = new StringContent(body, Encoding.UTF8, contentType);
                if (method == HttpMethod.Get)
                {
                    response = await client.GetAsync(endpoint);
                }
                else if (method == HttpMethod.Post)
                {
                    response = await client.PostAsync(endpoint, content);
                }
                else if (method == HttpMethod.Put)
                {
                    response = await client.PutAsync(endpoint, content);
                }
                else if (method == HttpMethod.Delete)
                {
                    response = await client.DeleteAsync(endpoint);
                }
                else
                {
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Debug.WriteLine(await response.Content.ReadAsStringAsync());
                    return null;
                }

                return null;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }
    }
}
