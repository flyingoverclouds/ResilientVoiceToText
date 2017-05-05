using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    /// <summary>
    /// Used to test/simulate functions code locally for DEV/TREST
    /// </summary>
    class ResilientVttFuntions
    {
        // http://stackoverflow.com/questions/36411536/how-can-i-use-nuget-packages-in-my-azure-functions
        // https://blogs.msdn.microsoft.com/appserviceteam/2017/03/16/publishing-a-net-class-library-as-a-function-app/


        public void LaunchVoiceToTExtOnFunctionSimulationSimulate(string audioFileNAme)
        {
            using (var audioStream = File.OpenRead(audioFileNAme))
            {

            }
        }


        //public static void Run(Stream myBlob, string name, TraceWriter log)
        public void LaunchVoiceToTExtOnFunctionSimulation_Run(Stream myBlob, string name, object log)
        {

        }
    }
}
