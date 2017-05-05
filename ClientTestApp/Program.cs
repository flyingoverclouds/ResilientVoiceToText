﻿using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //SimulateClient();
            SimulateFunction();
            Console.ReadLine();
        }


        static void SimulateClient()
        {
            Console.WriteLine("Resilient VoiceToText architecture implementation.");
            Console.WriteLine("upload target: " + ConfigurationManager.AppSettings["uploadSasUrl"]);
            Console.WriteLine("apiUrl: " + ConfigurationManager.AppSettings["apiUrl"]);

            var storageSasUri = new Uri(ConfigurationManager.AppSettings["uploadSasUrl"], UriKind.Absolute);
            var resilientVttApiUri = new Uri(ConfigurationManager.AppSettings["apiUrl"], UriKind.Absolute);

            try
            {
                var t = new ResilientVttTester();
                t.RunTest(storageSasUri, resilientVttApiUri, @"C:\record\audio_1_partial.wav").GetAwaiter().GetResult();


            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION : " + ex.ToString());
            }
        }

        static void SimulateFunction()
        {
            VoiceToTextOnBlob_Function function = new VoiceToTextOnBlob_Function();
            function.Simulate(@"C:\record\audio_1_p_mono16k.wav");


            var storageSasUri = new Uri(ConfigurationManager.AppSettings["audioContainerSasUrl"], UriKind.Absolute);
            var audioContainer = new CloudBlobContainer(storageSasUri);
            var audioBlob = audioContainer.GetBlockBlobReference("1.wav");
            audioBlob.FetchAttributes();

            Console.WriteLine("METADATA : " + audioBlob.Metadata["recognitionResult"]);

        }
    }
}
