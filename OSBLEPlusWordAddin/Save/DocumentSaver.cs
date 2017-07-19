using System;
using Ionic.Zip;
using System.IO;
using Microsoft.Office.Interop.Word;
using Osble.Client.AsyncServiceClient;
using System.Net.Http;
using System.Threading.Tasks;
using OSBLEPlus.Logic.DomainObjects.ActivityFeeds;

namespace OSBLEPlusWordAddin
{
    internal static class OSBLEDocumentSaver
    {
        public class SaveResult
        {
            private string m_errorMessage;

            private bool m_success;

            public SaveResult()
            {
                m_success = true;
                m_errorMessage = null;
            }

            public SaveResult(bool success, string errorMessage)
            {
                m_success = success;
                m_errorMessage = errorMessage;
            }

            public string ErrorMessage
            {
                get { return m_errorMessage; }
            }

            public bool Success
            {
                get { return m_success; }
            }
        }

        /// <summary>
        /// Saves a document in a zip folder within the local app data directory.
        /// </summary>
        /// <param name="d">document</param>
        /// <returns>path to zip</returns>
        public static string SaveFileToZip(Document doc)
        {
            //determine the string value of the local directory
            string tmpSaveDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            tmpSaveDir = Path.Combine(tmpSaveDir, StringConstants.LocalFolder);

            //adjust local directory appropiatetly with slashes
            if (!tmpSaveDir.EndsWith("\\") && !tmpSaveDir.EndsWith("/"))
            {
                tmpSaveDir += "\\";
            }

            //determine the name of the file without the extension
            string zipPath = doc.Name.Split('.')[0];

            //create path to zip
            zipPath = String.Format(tmpSaveDir + zipPath + ".zip");

            //add the saved document to a ZipFile object and then use the save method to save to the local directory
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(doc.FullName, "");
                zip.Save(zipPath);
            }

            return zipPath;
        }

        /// <summary>
        /// Asynchronous helper function to submit the assignment through a web api call
        /// </summary>
        /// <param name="submitEvent">submitevent for the assignment to be submitted</param>
        /// <param name="authToken">authorization token</param>
        public static async Task<HttpResponseMessage> SubmitAssignment(SubmitEvent submitevent, string authtoken)
        {
            return await AsyncServiceClient.SubmitAssignment(submitevent, authtoken);
        }

        /// <summary>
        /// Saves the active word document and zips the file in the local directory.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="course"></param>
        /// <param name="assignment"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static SaveResult Save(OSBLEState state, int courseId, int assignmentId, Document doc)
        {
            //if the document doesn't exist or contains changes then request the user to save
            if (!File.Exists(doc.FullName) || !doc.Saved)
            {
                return new SaveResult(false, "Please save the current document before submitting.");
            }

            //TODO: consider try/catch here
            //TODO: consider passing username/date/filename
            string zipPath = SaveFileToZip(doc);
            byte[] zipData = File.ReadAllBytes(zipPath);

            //package assignment information
            var submitevent = new SubmitEvent();
            submitevent.AssignmentId = assignmentId;
            submitevent.CourseId = courseId;
            submitevent.SolutionData = zipData;

            //call asynchronous web api helper function
            var result = SubmitAssignment(submitevent, state.AuthToken);

            return new SaveResult(true, null);
        }
    }
}

