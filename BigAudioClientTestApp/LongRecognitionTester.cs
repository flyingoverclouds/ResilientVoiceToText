﻿using Microsoft.Bing.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigAudioClientTestApp
{
    // NUGET  ADD : Microsoft.Bing.Speech

    // https://docs.microsoft.com/en-us/azure/cognitive-services/speech/api-reference-rest/bingvoicerecognition
    // https://github.com/Azure-Samples/Cognitive-Speech-STT-ServiceLibrary


    class LongRecognitionTester
    {
        
        public async Task<string> RecognizeFile(Stream audio,string stsUrl,string authKey, Uri longDictationUrl)
        {          var cts = new CancellationTokenSource();

            // create the preferences object
            var preferences = new Preferences("en-US", longDictationUrl, 
                new CognitiveServicesAuthorizationProvider(stsUrl,authKey));

            List<RecognitionResult> results = new List<RecognitionResult>();

            using (var speechClient = new SpeechClient(preferences))
            { 
                speechClient.SubscribeToRecognitionResult(async (rr) =>
                {
                    await Task.Run(() =>
                    {
                        results.Add(rr);
                        Console.WriteLine("--------------------------------------------");
                        Console.WriteLine($"RecognitionStatus = ${rr.RecognitionStatus}");
                        foreach(var p in rr.Phrases)
                        {
                            Console.WriteLine($"  {p.Confidence} {p.DisplayText}");
                        }
                    });
                });

                var deviceMetadata = new DeviceMetadata(DeviceType.Near, 
                    DeviceFamily.Desktop, 
                    NetworkType.Unknown, 
                    OsName.Unknown, "1607", "Azure", "Function");
                var applicationMetadata = new ApplicationMetadata("ResilientLongVtt", "1.0.0");
                var requestMetadata = new RequestMetadata(Guid.NewGuid(), deviceMetadata, applicationMetadata, "SampleAppService");
                
                 await speechClient.RecognizeAsync(new SpeechInput(audio, requestMetadata), cts.Token);

                Console.Write($"TOTAL RESULTS : {results.Count}");
                
            }

            return "rien";
        }

      

    }
}
