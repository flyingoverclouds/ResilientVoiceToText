using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using System.Configuration;
using System.Net.Http;


public static async Task Run(Stream myBlob, string name, TraceWriter log)
{
    log.Info($"STARTED at {DateTime.Now}");
    Stopwatch chrono = Stopwatch.StartNew();
    log.Info($"Processing blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
    try
    {
        var apiKey = ConfigurationManager.AppSettings["bingSpeechToTextApiKey"];
        var audioContainerSasUrl = ConfigurationManager.AppSettings["audioContainerSasUrl"];

        var storageSasUri = new Uri(audioContainerSasUrl, UriKind.Absolute);
        var audioContainer = new CloudBlobContainer(storageSasUri);
        var audioBlob = audioContainer.GetBlockBlobReference(name);
        try { await audioBlob.FetchAttributesAsync(); } catch { } // try to get metadata, ignoring error if blob does not exist 
        if (audioBlob.Metadata.ContainsKey("recognitionResult"))// AVOIDING A INFINITE LOOP IF VoiceToText METADATA ALREADY SET
        {
            log.Info("VoiceToText metadata found. EXITING.");
            return;
        }

        //*** retrieving a authentication bearer using the cognitive api key
        string bearerToken = null;
        using (var authTokenClient = new System.Net.Http.HttpClient())
        {
            authTokenClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var response = authTokenClient.PostAsync("https://api.cognitive.microsoft.com/sts/v1.0/issueToken", null).Result;
            bearerToken = response.Content.ReadAsStringAsync().Result;
        }
        //log.Info($"BEARER= {bearerToken}");

        //*** converting stream content to byte[]
        byte[] audioContent;
        using (MemoryStream ms = new MemoryStream())
        {
            myBlob.CopyTo(ms);
            audioContent = ms.ToArray();
        }
        log.Info($"audioContent byte array size : {audioContent.Length}");

        //*** calling cognitive service Bing SpeechToText api
        using (var client = new HttpClient())
        {
            // for recognize options, see https://docs.microsoft.com/en-us/azure/cognitive-services/speech/api-reference-rest/bingvoicerecognition
            string recognizeScenarios = "smd"; // smd, ulm ou websearch ???
            string recognizeReqId = Guid.NewGuid().ToString();
            string recognizeLocale = "fr-FR"; // for supported langage , see  https://docs.microsoft.com/en-us/azure/cognitive-services/speech/home#SpeechRecognition
            string recognizeFormat = "json";
            string recognizeInstanceId = "5E8B7743-D341-4B46-BFB6-3DC7F3D58843"; //unique by device
            var requestUri = $"https://speech.platform.bing.com/recognize?scenarios={recognizeScenarios}&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale={recognizeLocale}&device.os=AzureFunction&version=3.0&format={recognizeFormat}&instanceid={recognizeInstanceId}&requestid={recognizeReqId}";


            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);

            using (var binaryContent = new ByteArrayContent(audioContent))
            {
                //*** Sending audio content to api 
                binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                var response = await client.PostAsync(requestUri, binaryContent);
                var responseString = await response.Content.ReadAsStringAsync();

                //*** parsing api response
                log.Info($"RESULT: HttpStatus : {(int)(response.StatusCode)} / {response.StatusCode}");
                string recognitionResultMetadata = "";
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Info($"RESPONSE CONTENT : {responseString}");
                    recognitionResultMetadata = $"FAILED : HttpStatus : {(int)(response.StatusCode)} / {response.StatusCode} \n {responseString}";
                }
                else
                {
                    recognitionResultMetadata = responseString;
                }

                //*** Attaching result metadata (base64 encoding) to original blob : 
                audioBlob.Metadata["recognitionResult"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(recognitionResultMetadata));
                await audioBlob.SetMetadataAsync();
                log.Info("Audio metadata attached to blob.");
            }
        }
    }
    catch (Exception ex)
    {
        log.Error(ex.ToString());
    }
    finally
    {
        chrono.Stop();
        log.Info($"TERMINATED at {DateTime.Now}");
        log.Info($"ELAPSED TIME : {chrono.ElapsedMilliseconds}ms");
    }
}