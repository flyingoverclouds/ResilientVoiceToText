using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    /// <summary>
    /// Used to test/simulate functions code locally for DEV/TREST
    /// </summary>
    class VoiceToTextOnBlob_Function
    {
        // http://stackoverflow.com/questions/36411536/how-can-i-use-nuget-packages-in-my-azure-functions
        // https://blogs.msdn.microsoft.com/appserviceteam/2017/03/16/publishing-a-net-class-library-as-a-function-app/

        /// <summary>
        /// This method simulate (on code level) the call of Run method made by the Azure Function runtime.
        /// </summary>
        /// <param name="audioFileNAme"></param>
        public void Simulate(string audioFileNAme)
        {
            string filename = Path.GetFileName(audioFileNAme);
            TraceWriter consoleTraceWriter = new TraceWriter();
            using (var audioStream = File.OpenRead(audioFileNAme))
            {
                Run(audioStream, filename, consoleTraceWriter);
            }
        }


        //public static void Run(Stream myBlob, string name, TraceWriter log)
        public async void Run(Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var apiKey = System.Configuration.ConfigurationManager.AppSettings["bingSpeedToTextApiKey"];
            log.Info($"Bing SpeechToText apiKey = {apiKey}");

            //*** retrieving a authentication bearer using the api key
            string bearerToken = null;
            using (var authTokenClient = new System.Net.Http.HttpClient())
            {
                authTokenClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                var response = authTokenClient.PostAsync("https://api.cognitive.microsoft.com/sts/v1.0/issueToken", null).Result;
                bearerToken = response.Content.ReadAsStringAsync().Result;
            }
            log.Info($"BEARER= {bearerToken}");

            //*** converting stream content to byte[]
            byte[] audioContent;
            using (MemoryStream ms = new MemoryStream())
            {
                myBlob.CopyTo(ms);
                audioContent = ms.ToArray();
            }
            log.Info($"audioContent byte array size : {audioContent.Length}");

            //*** calling cognitive service SpeechToText api
            using (var client = new System.Net.Http.HttpClient())
            {
                // for recognize options, see https://docs.microsoft.com/en-us/azure/cognitive-services/speech/api-reference-rest/bingvoicerecognition
                string recognizeScenarios = "ulm"; // smd, ulm ou websearch ???
                string recognizeReqId = Guid.NewGuid().ToString(); 
                string recognizeLocale = "fr-FR"; // for supported langage , see  https://docs.microsoft.com/en-us/azure/cognitive-services/speech/home#SpeechRecognition
                string recognizeFormat = "json";
                string recognizeInstanceId = "5E8B7743-D341-4B46-BFB6-3DC7F3D58843"; //unique by device
                var requestUri = $"https://speech.platform.bing.com/recognize?scenarios={recognizeScenarios}&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale={recognizeLocale}&device.os=AzureFunction&version=3.0&format={recognizeFormat}&instanceid={recognizeInstanceId}&requestid={recognizeReqId}";


                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);

                using (var binaryContent = new System.Net.Http.ByteArrayContent(audioContent))
                {
                    binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                    var response = await client.PostAsync(requestUri, binaryContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    log.Info($"RESULT: HttpStatus {response.StatusCode}"); 
                    log.Info($"RESPONSE CONTENT : {responseString}");
                }
            }

        }
    }


}
