using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    class ResilientVttTester
    {
        public async Task RunTest(Uri sasAudioContainerUri, Uri apiUri, string fileToTest)
        {
            var rvtt = new ResilientVttServiceProxy(sasAudioContainerUri, apiUri);

            Stopwatch uploadChrono = Stopwatch.StartNew();
            var reqId = await rvtt.UploadFileForVTT(fileToTest);
            uploadChrono.Stop();
            Console.WriteLine($"Elapsed time for upload : {uploadChrono.ElapsedMilliseconds}ms");

            Console.WriteLine($"Updloaded file ID is: {reqId}");
            if (reqId != Guid.Empty)
            {
                Console.WriteLine("Waiting for result ... ");
                var storageSasUri = new Uri(ConfigurationManager.AppSettings["audioContainerSasUrl"], UriKind.Absolute);
                var audioContainer = new CloudBlobContainer(storageSasUri);
                var audioBlob = audioContainer.GetBlockBlobReference(reqId.ToString()+".wav");

                string recognitionResult = null;

                int counter = 0;
                do
                {
                    if (counter>60) // if more than 20try to read metadata -> terminate wait
                    {
                        break;
                    }
                    await Task.Delay(1000); // pause between eaCh try
                    Console.Write($"\rAttempt #{counter++}");
                    audioBlob.FetchAttributes();
                    if (audioBlob.Metadata.ContainsKey("recognitionResult"))
                    { // result metadata found attached on the blob !!
                        recognitionResult = audioBlob.Metadata["recognitionResult"];
                        break;
                    }

                    var result = await rvtt.GetVttResult(reqId);
                    if (!string.IsNullOrEmpty(result))
                        break;
                } while (true);
                Console.WriteLine();
                if (!string.IsNullOrEmpty(recognitionResult))
                {
                    Console.WriteLine("RECOGNITION RESULT : " + recognitionResult);
                }
                else
                {
                    Console.WriteLine(" WAIT TOOL LONG OR ERROR. VERIFY MANUALLY");
                }
            }
            else
            {
                Console.WriteLine("No ID returned --> something failed :(");
            }
            
        }
    }
}
