using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Windows.Forms;
using Osble.Model;
using Osble.Client.AsyncServiceClient;
using OSBLEPlus.Services.Models;
using OSBLEPlusWordAddin.Rubric;

namespace OSBLEPlusWordAddin
{
    public partial class ThisAddIn
    {
        WordSaveHandler saveHandler = null;

        RubricContainerForm rubricContainerForm;
        private Microsoft.Office.Tools.CustomTaskPane rubricTaskPane;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //initialize the save handler
            saveHandler = new WordSaveHandler(Application);
            //attach the save handler
            saveHandler.AfterAutoSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterAutoSaveEvent);
            saveHandler.AfterSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterSaveEvent);
            saveHandler.AfterUiSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterUiSaveEvent);

            rubricContainerForm = new RubricContainerForm();
            rubricTaskPane = this.CustomTaskPanes.Add(rubricContainerForm, "Osble +Rubric");
            rubricTaskPane.Visible = true;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {

        }

        void saveHandler_AfterUiSaveEvent(Word.Document doc, bool isClosed)
        {
            //if (!isClosed)
            //    MessageBox.Show("After SaveAs Event");
            //else
            //    MessageBox.Show("After Close and SaveAs Event. The filname was: " + saveHandler.ClosedFilename);
        }

        void saveHandler_AfterSaveEvent(Word.Document doc, bool isClosed)
        {
            //if (!isClosed)
            //    MessageBox.Show("After Save Event");
            //else
            //    MessageBox.Show("After Close and Save Event. The filname was: " + saveHandler.ClosedFilename);

            string authToken = null;
            try
            {
                authToken = Globals.Ribbons.OSBLE_Ribbon.mState.AuthToken;
                var response = AsyncServiceClient.SubmitStatistics(authToken, doc);
            }

            catch(Exception e)
            {

            }
        }

        void saveHandler_AfterAutoSaveEvent(Word.Document doc, bool isClosed)
        {
            //MessageBox.Show("After AutoSave Event");
        }

        public Word.Document GetActiveDocument()
        {
            return (Word.Document)Application.ActiveDocument;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
