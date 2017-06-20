using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Tools.Ribbon;
using Word = Microsoft.Office.Interop.Word;

//testing only
using System.Windows.Forms;

namespace OSBLEPlusWordAddin
{
    public partial class OSBLE_Ribbon
    {
        private OSBLEState m_state = null;

        private OSBLEState mState
        {
            get { return m_state; }
            set
            {
                //avoid further steps if m_state is equivalent to the value
                if(m_state != value)
                {
                    m_state = value;

                    //check value of state when it is changed to show or hide the OSBLE options
                    if (m_state == null)
                        grpOSBLEOptions.Visible = false;
                    else
                        grpOSBLEOptions.Visible = true;

                }
            }
        }
        

        private void OSBLE_Ribbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RibbonControlEventArgs e)
        {
            //display login form and retrieve OSBLE state
            frmOSBLELogin loginForm = new frmOSBLELogin();
            mState = loginForm.DoPrompt();

            //if the state is not null then the user has successfully logged in
            if (mState != null)
                loginActions();
            else
                logoutActions();
        }

        private void btnLogout_Click(object sender, RibbonControlEventArgs e)
        {
            logoutActions();
        }

        private void btnUpload_Click(object sender, RibbonControlEventArgs e)
        {
            //if state is null then logout and return
            if (mState == null)
            {
                logoutActions();
                return;
            }
            
            if (dropDownCourse.SelectedItem.Tag != null)
            {
                WordSaveHandler wsh = new WordSaveHandler(Globals.ThisAddIn.Application);

                //TODO: check submission is successful before updating last save
                lblLastSave.Label = "Last Save: " + DateTime.Now.ToString();
            }

            else
            {
                showMessage("Oops! OSBLE+ could not submit to the selected course.");
            }
        }

        private void showMessage(string msg)
        {
            MessageBox.Show(msg, "OSBLE+ Word Add-in");
        }

        private void loginActions()
        {
            //show OSBLE options and switch login and logout visibility
            grpOSBLEOptions.Visible = true;
            btnLogin.Visible = false;
            btnLogout.Visible = true;

            //reset last save label
            lblLastSave.Label = "Last Save: ";

            //clear drop down menu and populate with new items
            dropDownCourse.Items.Clear();
            
            if (mState.Courses.Length > 0)
            {
                foreach (OSBLEServices.Course c in mState.Courses)
                {
                    RibbonDropDownItem item
                            = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                    item.Label = c.Name + ", " + c.Semester.Substring(0, 2).ToUpper() + ", " + c.Year;
                    item.Tag = c;
                    dropDownCourse.Items.Add(item);
                }
            }

            else
            {
                RibbonDropDownItem item
                            = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                item.Label = "No Courses Found";
                item.Tag = null;
                dropDownCourse.Items.Add(item);
            }
        }

        private void logoutActions()
        {
            //set state to null
            mState = null;

            //hide OSBLE options and switch login and logout visibility
            grpOSBLEOptions.Visible = false;
            btnLogin.Visible = true; ;
            btnLogout.Visible = false;

            //reset last save label
            lblLastSave.Label = "Last Save: ";

            //clear drop down menu
            dropDownCourse.Items.Clear();
            RibbonDropDownItem item
                                = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
            item.Label = "No Courses Found";
            item.Tag = null;
            dropDownCourse.Items.Add(item);
        }
    }
}
