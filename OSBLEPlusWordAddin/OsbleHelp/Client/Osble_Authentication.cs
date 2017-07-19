using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

namespace Osble.Client.Authentication
{
    static class AuthenticationClient
    {
        /// <summary>
        /// Asynchronous web api call to authenticate the submitted username and password
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        /// <returns>authentication token </returns>
        public static string Login(string userName, string password)
        {
            using (var client = ServiceClient.GetClient())
            {
                // Send post data via key/value pairs in a dictionary to be deserialized into json
                // for the new HttpPost method.
                Dictionary<string, string> values = new Dictionary<string, string>()
                {
                    {"e", userName},
                    {"hp", GetPasswordHash(password)}
                };

                var content = new FormUrlEncodedContent(values);

                var task = client.PostAsync("api/userprofiles/login", content);

                //await task;

                try
                {
                    return task.Result.StatusCode == HttpStatusCode.OK
                                               ? task.Result.Content.ReadAsStringAsync().Result
                                               : string.Empty;
                }
                catch (System.AggregateException) //failed to connect to the OSBLE server
                {
                    return string.Empty;
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
