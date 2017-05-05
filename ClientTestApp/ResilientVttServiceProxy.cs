using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTestApp
{
    public class ResilientVttServiceProxy
    {
        Uri containerSasUri = null;
        Uri resilientApiUri = null;

        public ResilientVttServiceProxy(Uri containerSasUri,Uri resilientApiUri)
        {
            this.containerSasUri = containerSasUri;
            this.resilientApiUri = resilientApiUri;
        }

        /// <summary>
        /// Upload a file to a Azure Storage Blob. return the request GUID.
        /// </summary>
        /// <param name="fullFilename">full fill name with drive and folder path of file to upload</param>
        /// <returns>requestion id to use for requesting result. If Guid.Empty : something went wrong</returns>
        public async Task<Guid> UploadFileForVTT(string fullFilename)
        {
            if (!File.Exists(fullFilename))
            {
                throw new System.IO.FileNotFoundException($"{fullFilename} is not found.");
            }
            Guid requestId = Guid.NewGuid();
            string newblobName = requestId.ToString() +  System.IO.Path.GetExtension(fullFilename);
            var audioContainer = new CloudBlobContainer(containerSasUri);
            var audioBlob = audioContainer.GetBlockBlobReference(newblobName);

            await audioBlob.UploadFromFileAsync(fullFilename);

            // Use storage SDK to upload 
            return requestId;
        }


        /// <summary>
        /// call resilient VTT api to check if result is available. 
        /// return null if no result, or json result.
        /// throw exception if something went wrong.
        /// </summary>
        /// <param name="vttReqId">Id of the request (returned by the upload method)</param>
        /// <returns>null if not result, json if result available</returns>
        public async Task<string> GetVttResult(Guid vttReqId)
        {
            await Task.Delay(100); //simulate api call
            return null;
        }


    }
}
