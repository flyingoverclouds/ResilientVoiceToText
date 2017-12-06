using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigAudioClientTestApp
{
    class Program
    {

        static void Main(string[] args)
        {
            // for CRIS : (retrive the right URL from cris.ai portal : WebSocket with the .NET service Library)
            Uri crisLongDictationUrl = new Uri(@"wss://REPLACE_BY_YOUR_DEPLOYMENT_URL.api.cris.ai/ws/cris/speech/recognize/continuous");

            // for CRIS : retrieved from azure portal
            string crisAuthKey = "PLACE_HERE_YOUR_COGNITIVESERVICe_API_KEY";

            // for CRIS : retieved from portal, the CSS Cog Svc instance, tab 'overview' 
            string crisStsEndpoint = "https://westus.api.cognitive.microsoft.com/sts/v1.0"; 


            string filename;
            //filename= @"c:\record\tardis_nc-voixderoutante-PCM16.wav";
            //filename = @"c:\record\tardis-nc.wav";
            //filename = @"c:\record\audio_1_mono_16kpcm.wav";

            filename = @"c:\record\Shakespeare-TheTempest-4.wav";

            using (var audioStream = File.OpenRead(filename))
            {
                var t = new LongRecognitionTester();
                var result = t.RecognizeFile(audioStream, crisStsEndpoint, crisAuthKey,crisLongDictationUrl).GetAwaiter().GetResult();
                Console.WriteLine($"RECOGNITION RESULT : {result}");
            }
            Console.ReadLine();
        }
      
    }
}
