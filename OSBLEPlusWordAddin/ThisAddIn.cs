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

namespace OSBLEPlusWordAddin
{
    public partial class ThisAddIn
    {
        WordSaveHandler saveHandler = null;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //initialize the save handler
            saveHandler = new WordSaveHandler(Application);
            //attach the save handler
            saveHandler.AfterAutoSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterAutoSaveEvent);
            saveHandler.AfterSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterSaveEvent);
            saveHandler.AfterUiSaveEvent += new WordSaveHandler.AfterSaveDelegate(saveHandler_AfterUiSaveEvent);
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
            var response = AsyncServiceClient.SubmitStatistics(new WordStats("authtokentest", doc));
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
