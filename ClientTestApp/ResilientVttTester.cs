using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    class ResilientVttTester
    {
        public async Task RunTest(Uri sasAudioContainerUri, string fileToTest)
        {
            Guid requestId = Guid.NewGuid(); // unique ID used as blob named (to avoid collide)

            string newblobName = requestId.ToString() + System.IO.Path.GetExtension(fileToTest);
            var audioContainer = new CloudBlobContainer(sasAudioContainerUri);
            var audioBlob = audioContainer.GetBlockBlobReference(newblobName);


            //**** UPLOADING AUDIO FILE AS BLOB
            Stopwatch chrono = Stopwatch.StartNew();
            if (!File.Exists(fileToTest))
            {
                throw new System.IO.FileNotFoundException($"{fileToTest} is not found.");
            }
            await audioBlob.UploadFromFileAsync(fileToTest);
            chrono.Stop();
            Console.WriteLine($"Elapsed time for upload : {chrono.ElapsedMilliseconds}ms");


            //**** WAITING FOR METADATA
            Console.WriteLine("Waiting for voice recognition result ... ");
            string recognitionResult = null;
            chrono = Stopwatch.StartNew();
            int counter = 0;
            do
            {
                await Task.Delay(1000); // pause between each try
                Console.Write($"\rAttempt #{counter++}");
                    
                audioBlob.FetchAttributes();
                if (audioBlob.Metadata.ContainsKey("recognitionResult")) // if audio metadata found attached on the blob !!
                { 
                    recognitionResult = audioBlob.Metadata["recognitionResult"];
                    break;
                }
            } while (counter<=30); // if more than 30tries to read metadata -> terminate polling

            if (!string.IsNullOrEmpty(recognitionResult))
            {
                string vttResult = System.Text.Encoding.UTF8.GetString( Convert.FromBase64String(recognitionResult));
                Console.WriteLine("\nRECOGNITION RESULT : " + vttResult);
            }
            else
                Console.WriteLine("\nWAIT TOO LONG OR ERROR. VERIFY MANUALLY !!");
            chrono.Stop();
            Console.WriteLine($"Elapsed time for recognition result : {chrono.ElapsedMilliseconds}ms");

        }
    }
}
