using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Ionic.Zip;
using System.IO;
using Microsoft.Office.Interop.Word;

//Note: Marjority of the following code is copied from the OSBLEExcelPlugin.sln
//  OSBLEWorkBookSaver.cs


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
        /// Saves the active word document and zips the file in the local directory.
        /// </summary>
        /// <param name="userName">Username of the user currently logged in</param>
        /// <param name="password">Password of the user currently logged in</param>
        /// <param name="courseID">Course ID of the selected course in the ribbon drop down menu</param>
        /// <param name="d">Active document opened in Word to be submitted</param>
        /// <returns></returns>
        public static SaveResult Save(string userName, string password, int courseID, Document d)
        {
            //if the document contains changes then request the user to save
            if(!d.Saved)
            {
                return new SaveResult(false, "Please save the current document before submitting.");
            }

            //determine the string value of the local directory
            string tempSaveDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //adjust local directory appropiatetly with slashes
            if (!tempSaveDir.EndsWith("\\") && !tempSaveDir.EndsWith("/"))
            {
                tempSaveDir += "\\";
            }

            //add the saved document to a ZipFile object and then use the save method to save to the local directory
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(d.FullName, "");
                string s = String.Format(tempSaveDir + "OSBLE_Word_Upload.zip");
                zip.Save(s);
            }



            return new SaveResult(true, null);
        }
    }
}

