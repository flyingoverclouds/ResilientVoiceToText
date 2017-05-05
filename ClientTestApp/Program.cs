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
            Console.ReadLine();
        }

    }
}
