using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using OSBLEPlusWordAddin;

namespace Osble.Client
{
    static class ServiceClient
    {
        public static HttpClient GetClient()
        {
            // THIS IS FOR DEBUGGING ONLY, THIS TURNS OFF HTTPS VALIDATION
            // DO NOT USE ON THE LIVE SERVER
#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
#endif
            var client = new HttpClient { BaseAddress = new Uri(StringConstants.DataServiceRoot) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;

        }
    }
}
