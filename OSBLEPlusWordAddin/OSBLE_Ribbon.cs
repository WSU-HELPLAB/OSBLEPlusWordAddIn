using System;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Osble.Model.ProfileCourse;
using Osble.Model.SubmissionAssignment;

namespace OSBLEPlusWordAddin
{
    public partial class OSBLE_Ribbon
    {
        public OSBLEState m_state = null;

        /// <summary>
        /// Property used to get and set the m_state member. The setter will update
        /// the user interface by showing or hiding the OSBLE+ ribbon options
        /// depending on the OSBLEState of the value.
        /// </summary>
        public OSBLEState mState
        {
            get { return m_state; }
            private set
            {
                //avoid further steps if m_state is equivalent to the value
                if(m_state != value)
                {
                    m_state = value;

                    //check value of state when it is changed to show or hide the OSBLE options
                    if (m_state == null)
                        logoutActions();
                    else
                        loginActions();

                }
            }
        }

        /// <summary>
        /// The function called when the login button is clicked. The event will show the
        /// login form and update mState with its return value. The value of mState will
        /// then be used to update the ribbon user interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RibbonControlEventArgs e)
        {
            //display login form and retrieve OSBLE state
            frmOSBLELogin loginForm = new frmOSBLELogin();
            mState = loginForm.DoPrompt();
        }

        /// <summary>
        /// The function called when the logout button is clicked. The event will hide
        /// the OSBLE+ ribbon options and logout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RibbonControlEventArgs e)
        {
            logoutActions();
        }

        /// <summary>
        /// The function called when the submit button is clicked. If the submission
        /// is successful then the last save label will be updated, otherwise an
        /// error will appear explainin why the submission failed or what steps
        /// must be taken to submit properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, RibbonControlEventArgs e)
        {
            //if state is null then logout
            if (mState == null)
                return;
            
            //check if the selected drop down menu has a valid tag
            if (dropDownCourse.SelectedItem.Tag != null && dropDownAssignment.SelectedItem.Tag != null)
            {
                var course = dropDownCourse.SelectedItem.Tag as ProfileCourse;
                var assignment = dropDownAssignment.SelectedItem.Tag as SubmisionAssignment;
                var document = Globals.ThisAddIn.Application.ActiveDocument;

                //submit the document and return submission status
                OSBLEDocumentSaver.SaveResult sr = OSBLEDocumentSaver.Save(mState, course.Id, assignment.Id, document);

                //update last save label if the submission was successful otherwise display the returned error message
                if(sr.Success)
                    lblLastSave.Label = "Last Save: " + DateTime.Now.ToString();
                else
                    showMessage(sr.ErrorMessage);
            }

            else
                showMessage("OSBLE+ could not submit to the selected course.");
        }
        
        /// <summary>
        /// Displays a message in a box that will prevent any further action
        /// or Word window overlap before being handled with. Using this function
        /// will help to maintain consistent message box titles.
        /// </summary>
        /// <param name="msg">alert message to be shown</param>
        private void showMessage(string msg)
        {
            MessageBox.Show(msg, "OSBLE+ Word Add-in");
        }

        /// <summary>
        /// Updates the ribbon user interface to give a user who has logged in
        /// successful the option to submit documents and logout. The drop down
        /// menu will be populated with the courses they are allowed to submit
        /// documents.
        /// </summary>
        private void loginActions()
        {
            //show OSBLE options and switch login and logout visibility
            grpOSBLEOptions.Visible = true;
            btnLogin.Visible = false;
            btnLogout.Visible = true;

            //reset last save label
            lblLastSave.Label = "Last Save: ";

            //clear drop down menus
            dropDownCourse.Items.Clear();
            dropDownAssignment.Items.Clear();

            if (mState.Courses.Length > 0)
            {
                //display message in drop down menus
                RibbonDropDownItem courseBlank = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                courseBlank.Label = "Select a course...";
                courseBlank.Tag = null;
                dropDownCourse.Items.Add(courseBlank);

                RibbonDropDownItem assignmentBlank = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                dropDownAssignment.Items.Clear();
                assignmentBlank.Label = "Select a course to populate assignments...";
                assignmentBlank.Tag = null;
                dropDownAssignment.Items.Add(assignmentBlank);

                //populate drop down course menu
                foreach (ProfileCourse c in mState.Courses)
                {
                    RibbonDropDownItem item = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                    item.Label = c.Name;
                    item.Tag = c;
                    dropDownCourse.Items.Add(item);
                }
            }

            else
            {
                //display no items found error
                RibbonDropDownItem courseNone = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                courseNone.Label = "No Courses Found";
                courseNone.Tag = null;
                dropDownCourse.Items.Add(courseNone);

                RibbonDropDownItem assignmentNone = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                dropDownAssignment.Items.Clear();
                assignmentNone.Label = "No Assignments Found";
                assignmentNone.Tag = null;
                dropDownAssignment.Items.Add(assignmentNone);
            }
        }

        /// <summary>
        /// Logs out a user and updates the ribbon user interface to prevent
        /// the option of submitting documents to courses. The drop down menu
        /// is emptied.
        /// </summary>
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

            //clear drop down course menu
            dropDownCourse.Items.Clear();
            RibbonDropDownItem item = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
            item.Label = "No Courses Found";
            item.Tag = null;
            dropDownCourse.Items.Add(item);

            //clear drop down assignment menu
            dropDownAssignment.Items.Clear();
            item.Label = "No Assignments Found";
            item.Tag = null;
            dropDownAssignment.Items.Add(item);
        }

        /// <summary>
        /// The function called when a course is selected from the drop down menu.
        /// The function will determine the selected course and use it to collect
        /// the submission assignment for the course to populate the drop down
        /// assignment menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropDownCourse_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            //create course object for the selected course
            ProfileCourse c = dropDownCourse.SelectedItem.Tag as ProfileCourse;

            //clear drop down assignment menu
            dropDownAssignment.Items.Clear();

            //update collection of assignments for the selected course
            mState.UpdateAssignments(c);

            if (mState.Assignments != null && mState.Assignments.Length > 0)
            {
                //display message in drop down assignment menu
                RibbonDropDownItem assignmentBlank = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                assignmentBlank.Label = "Select an assignment...";
                assignmentBlank.Tag = null;
                dropDownAssignment.Items.Add(assignmentBlank);

                //populate drop down assignment menu
                foreach (SubmisionAssignment a in mState.Assignments)
                {
                    RibbonDropDownItem item = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                    item.Label = a.Name;
                    item.Tag = a;
                    dropDownAssignment.Items.Add(item);
                }
            }

            else
            {
                //display no items found error
                RibbonDropDownItem assignmentNone = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                dropDownAssignment.Items.Clear();
                assignmentNone.Label = "No Assignments Found";
                assignmentNone.Tag = null;
                dropDownAssignment.Items.Add(assignmentNone);
            }
        }
    }
}
