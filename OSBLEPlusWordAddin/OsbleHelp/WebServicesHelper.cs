using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using OSBLEStructures;

namespace OSBLEPlusWordAddin
{
    static class AuthenticationHelper
    {
        /// <summary>
        /// Asynchronous web api call to authenticate the submitted username and password
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        /// <returns>authentication token </returns>
        public static string Login(string userName, string password)
        {
            using (var client = AsyncServiceClient.GetClient())
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

    static class ServicesHelper
    {
        /// <summary>
        /// Asynchronous web api call to retrieve the submission assignments for a course.
        /// </summary>
        /// <param name="courseId">course ID</param>
        /// <param name="authToken">authorization token</param>
        /// <returns>task with a list of submission assignments as the result</returns>
        public static async Task<List<SubmisionAssignment>> GetAssignmentsForCourse(int courseId, string authToken)
        {
            using (var client = AsyncServiceClient.GetClient())
            {
                var task = client.GetAsync(string.Format("api/course/getassignmentsforcourse?id={0}&a={1}", courseId, authToken));
                await task;
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new AssignmentJsonConverter() }
                };
                return JsonConvert.DeserializeObject<List<SubmisionAssignment>>(task.Result.Content.ReadAsStringAsync().Result);
            }
        }

        /// <summary>
        /// Asynchronous web api call to retrieve the courses for a user given their authentication token.
        /// </summary>
        /// <param name="authToken">authorization token</param>
        /// <returns>task with a list of courses as the result</returns>
        public static async Task<List<ProfileCourse>> GetCoursesForUser(string authToken)
        {
            using (var client = AsyncServiceClient.GetClient())
            {
                var task = client.GetAsync(string.Format("api/userprofiles/getcoursesforuser?a={0}", authToken));
                await task;

                return JsonConvert.DeserializeObject<List<ProfileCourse>>(task.Result.Content.ReadAsStringAsync().Result);
            }
        }
    }

    public class AssignmentJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ICourse));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(ICourse))
                return JObject.Load(reader).ToObject<ProfileCourse>(serializer);

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class AsyncServiceClient
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
