using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Osble.Model;
using Osble.Model.ProfileCourse;
using Osble.Model.SubmissionAssignment;
using OSBLEPlus.Logic.DomainObjects.ActivityFeeds;
using OSBLEPlus.Services.Models;

namespace Osble.Client.AsyncServiceClient
{
    static class AsyncServiceClient
    {
        /// <summary>
        /// Asynchronous web api call to retrieve the courses for a user given their authentication token.
        /// </summary>
        /// <param name="authToken">authorization token</param>
        /// <returns>task with a list of courses as the result</returns>
        public static async Task<List<ProfileCourse>> GetCoursesForUser(string authToken)
        {
            using (var client = ServiceClient.GetClient())
            {
                var task = client.GetAsync(string.Format("api/userprofiles/getcoursesforuser?a={0}", authToken));
                await task;

                return JsonConvert.DeserializeObject<List<ProfileCourse>>(task.Result.Content.ReadAsStringAsync().Result);
            }
        }

        /// <summary>
        /// Asynchronous web api call to retrieve the submission assignments for a course.
        /// </summary>
        /// <param name="courseId">course ID</param>
        /// <param name="authToken">authorization token</param>
        /// <returns>task with a list of submission assignments as the result</returns>
        public static async Task<List<SubmisionAssignment>> GetAssignmentsForCourse(int courseId, string authToken)
        {
            using (var client = ServiceClient.GetClient())
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
        /// Asynchronous web api call to submit an assinment to the server.
        /// </summary>
        /// <param name="submitEvent">submitevent for the assignment to be submitted</param>
        /// <param name="authToken">authorization token</param>
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> SubmitAssignment(SubmitEvent submitEvent, string authToken)
        {
            using (var client = ServiceClient.GetClient())
            {
                //package submitevent and authtoken within a SubmissionRequest
                var request = new SubmissionRequest
                {
                    AuthToken = authToken,
                    SubmitEvent = submitEvent,
                };

                var response = await client.PostAsXmlAsync("api/course/post", request);

                return response;
            }
        }

        /// <summary>
        /// Asynchronous web call to retrieve the full name of the logged in user
        /// </summary>
        /// <param name="authToken">authoruzation token</param>
        /// <returns>full name of logged in user</returns>
        public static async Task<string> GetName(string authToken)
        {
            using (var client = ServiceClient.GetClient())
            {
                var task = client.GetAsync(string.Format("api/userprofiles/getname?a={0}", authToken));
                await task;

                return JsonConvert.DeserializeObject<string>(task.Result.Content.ReadAsStringAsync().Result);
            }
        }

        /// <summary>
        /// Asynchronous web call to submit the word statistics to the server.
        /// </summary>
        /// <param name="stats">word statistics</param>
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> SubmitStatistics(WordStats stats)
        {
            using (var client = ServiceClient.GetClient())
            {
                //package submitevent and authtoken within a SubmissionRequest
                var request = new WordStats(stats);

                var response = await client.PostAsXmlAsync("api/word/post", request);

                return response;
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
                return JObject.Load(reader).ToObject<Osble.Model.ProfileCourse.ProfileCourse>(serializer);

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
}
