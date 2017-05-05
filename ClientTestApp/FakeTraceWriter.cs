using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    /// <summary>
    /// Fake tracewriter (log to console). Based on https://github.com/Azure/azure-webjobs-sdk/blob/663a508e8a851629c26a51e7de3af36629dfd120/src/Microsoft.Azure.WebJobs.Host/TraceWriter.cs
    /// </summary>
    class TraceWriter
    {
        public TraceWriter()
        {
      
        }


        public void Verbose(string message, string source = null)
        {
            Console.WriteLine($"VERBOSE: {message}");
        }


        public void Info(string message, string source = null)
        {
            Console.WriteLine($"INFO: {message}");
        }

        public void Warning(string message, string source = null)
        {
            Console.WriteLine($"WARNING: {message}");
        }

        public void Error(string message, Exception ex = null, string source = null)
        {
            Console.WriteLine($"Error: {message}");
        }


        public virtual void Flush()
        {
        }
    }
}
