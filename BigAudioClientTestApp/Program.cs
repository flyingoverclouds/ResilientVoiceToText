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
            string filename;
            //filename= @"c:\record\tardis_nc-voixderoutante-PCM16.wav";
            filename = @"c:\record\tardis-nc.wav";
            //filename = @"c:\record\audio_1_mono_16kpcm.wav";
            using (var audioStream = File.OpenRead(filename))
            {
                var t = new LongRecognitionTester();
                var result = t.RecognizeFile(audioStream).GetAwaiter().GetResult();
                Console.WriteLine($"RECOGNITION RESULT : {result}");
            }
            Console.ReadLine();
        }
      
    }
}
