using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSBLEPlusWordAddin
{
    public partial class frmOSBLELogin : Form
    {        
        public frmOSBLELogin()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {            
            string username = txtUsername.Text;            
            string password = txtPassword.Text;            
            bool rememberCredentials = chkRememberCredentials.Checked;

            //authenticate the username and password
            string authkey = OSBLEAuthentication.Login(username, password);
            //var authkey = await task; //authkey stored locally and used for authentication

            if(String.IsNullOrEmpty(authkey)) //user credentials were not authenticated
            {
                //TODO: make this feedback show on the form instead of messagebox
                MessageBox.Show("Invalid user name or password.");
            }
            else if(authkey == "ConnectionFailure")
            {
                //TODO: make this feedback show on the form instead of messagebox
                MessageBox.Show("Unable to connect to the OSBLE+ server. Please check your connection and try again.");
            }
            else //authenticated! this is the users's authkey...
            {
                if (rememberCredentials)
                {
                    //TODO: handle the local storage of the user credentials
                }

                //TODO: change logged in icon

                //TODO: show the course options group (currently visible, needs to default hidden)

                //TODO: store the authkey locally for use in submitting data to the server
                //MessageBox.Show(authkey);                 
                this.Close();
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmOSBLELogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
