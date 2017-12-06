using Microsoft.Bing.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BigAudioClientTestApp
{
    public sealed class CognitiveServicesAuthorizationProvider : IAuthorizationProvider
    {
   
        /// <summary>
        /// The subscription key
        /// </summary>
        private readonly string subscriptionKey;

        /// <summary>
        /// STS url to retrieve auth token
        /// </summary>
        private readonly string stsUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CognitiveServicesAuthorizationProvider" /> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription identifier.</param>
        public CognitiveServicesAuthorizationProvider(string stsUrl,string subscriptionKey)
        {
            if (string.IsNullOrEmpty(subscriptionKey))
            {
                throw new ArgumentException(nameof(subscriptionKey));
            }
            if (string.IsNullOrEmpty(stsUrl))
            {
                throw new ArgumentException(nameof(stsUrl));
            }

            this.stsUrl = stsUrl;
            this.subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// Gets the authorization token asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of the string parameter contains the next the authorization token.
        /// </returns>
        /// <remarks>
        /// This method should always return a valid authorization token at the time it is called.
        /// </remarks>
        public Task<string> GetAuthorizationTokenAsync()
        {
            return FetchToken(stsUrl, this.subscriptionKey);
        }

        /// <summary>
        /// Fetches the token.
        /// </summary>
        /// <param name="fetchUri">The fetch URI.</param>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <returns>An access token.</returns>
        private static async Task<string> FetchToken(string fetchUri, string subscriptionKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                var uriBuilder = new UriBuilder(fetchUri);
                uriBuilder.Path += "/issueToken";

                using (var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false))
                {
                    return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
