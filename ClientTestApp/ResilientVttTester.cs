using System;
using System.Collections.Generic;
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
            
            var reqId = await rvtt.UploadFileForVTT(fileToTest);

            Console.WriteLine($"Updloaded file ID is: {reqId}");
            if (reqId != Guid.Empty)
            {
                Console.WriteLine("Waiting for result ... ");
                int counter = 0;
                do
                {
                    await Task.Delay(750); // pause between eaCh try
                    Console.Write($"\rAttempt #{counter++}");
                    
                    var result = await rvtt.GetVttResult(reqId);
                    if (!string.IsNullOrEmpty(result))
                        break;
                } while (true);

                // TODO : check & parse return string
            }
            else
            {
                Console.WriteLine("No ID returned --> something failed :(");
            }
            
        }
    }
}
