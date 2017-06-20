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
                //if m_state equals the value then return to avoid infinite loop
                if (m_state != value)
                {
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
            m_state = loginForm.DoPrompt();

            //if the state is not null that the user has successfully logged in
            if (m_state != null)
            {
                //show OSBLE options and switch login and logout visibility
                grpOSBLEOptions.Visible = true;
                btnLogin.Visible = false;
                btnLogout.Visible = true;
                lblLastSave.Label = "Last Save: ";

                dropDownCourse.Items.Clear();

                if (m_state.Courses.Length > 0)
                {
                    foreach (OSBLEServices.Course c in m_state.Courses)
                    {
                        RibbonDropDownItem item
                                = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                        item.Label = c.Name + ", " + c.Semester.Substring(0, 2).ToUpper() + ", " + c.Year;
                        item.Tag = c;
                        dropDownCourse.Items.Add(item);
                    }

                    //debug only
                    /*RibbonDropDownItem debug_item
                                = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                    debug_item.Label = "No Courses Found";
                    debug_item.Tag = null;
                    dropDownCourse.Items.Add(debug_item);
                    */
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

            else
            {
                //hide OSBLE options and switch login and logout visibility
                grpOSBLEOptions.Visible = false;
                btnLogin.Visible = true; ;
                btnLogout.Visible = false;
            }
        }

        private void btnLogout_Click(object sender, RibbonControlEventArgs e)
        {
            m_state = null;

            //hide OSBLE options and switch login and logout visibility
            grpOSBLEOptions.Visible = false;
            btnLogin.Visible = true; ;
            btnLogout.Visible = false;
        }
    }
}
