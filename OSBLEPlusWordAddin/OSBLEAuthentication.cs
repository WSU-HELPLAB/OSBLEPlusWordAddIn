using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;


namespace OSBLEPlusWordAddin
{
    public class OSBLEAuthentication
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
        public static string Login(string userName, string password)
        {
            using (var client = GetClient())
            {
                // Send post data via key/value pairs in a dictionary to be deserialized into json
                // for the new HttpPost method.
                Dictionary<string, string> values = new Dictionary<string, string>()
                {
                    {"e", userName},
                    {"hp", GetPasswordHash(password)}
                };

                var content = new FormUrlEncodedContent(values);

                var task = client.PostAsync ("api/userprofiles/login", content);

                //await task;

                try
                {
                    return task.Result.StatusCode == HttpStatusCode.OK
                                               ? task.Result.Content.ReadAsStringAsync().Result
                                               : string.Empty;
                }
                catch (System.AggregateException) //failed to connect to the OSBLE server
                {
                    return "ConnectionFailure";
                }
            }
        }

        /// <summary>
        /// Converts a clear-text password into its encrypted form
        /// </summary>
        /// <param name="rawPassword"></param>
        /// <returns></returns>
        public static string GetPasswordHash(string rawPassword)
        {
            SHA256Managed algorithm = new SHA256Managed();

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] unhashedPassword = System.Text.Encoding.UTF8.GetBytes(rawPassword);
            byte[] hashedBytes = sha.ComputeHash(unhashedPassword);
            string hashedPassword = System.Text.Encoding.UTF8.GetString(hashedBytes);
            return hashedPassword;
        }
    }
}
