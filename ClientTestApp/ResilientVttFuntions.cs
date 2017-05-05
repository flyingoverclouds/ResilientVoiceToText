using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    /// <summary>
    /// Used to test/simulate functions code locally for DEV/TREST
    /// </summary>
    class ResilientVttFunctions
    {
        // http://stackoverflow.com/questions/36411536/how-can-i-use-nuget-packages-in-my-azure-functions
        // https://blogs.msdn.microsoft.com/appserviceteam/2017/03/16/publishing-a-net-class-library-as-a-function-app/

        /// <summary>
        /// This method simulate (on code level) the call of Run method made by the Azure Function runtime.
        /// </summary>
        /// <param name="audioFileNAme"></param>
        public void LaunchVoiceToTExtOnFunctionSimulationSimulate(string audioFileNAme)
        {
            string filename = Path.GetFileName(audioFileNAme);
            TraceWriter consoleTraceWriter = new TraceWriter();
            using (var audioStream = File.OpenRead(audioFileNAme))
            {
                LaunchVoiceToTExtOnFunctionSimulation_Run(audioStream, filename, consoleTraceWriter);
            }
        }


        //public static void Run(Stream myBlob, string name, TraceWriter log)
        public void LaunchVoiceToTExtOnFunctionSimulation_Run(Stream myBlob, string name, TraceWriter log)
        {

        }
    }

  
}
