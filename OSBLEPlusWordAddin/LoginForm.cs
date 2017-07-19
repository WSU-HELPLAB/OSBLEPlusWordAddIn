using OSBLEPlusWordAddin;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSBLEPlusWordAddin
{
    internal partial class frmOSBLELogin : Form
    {
        /// <summary>
        /// Name of the file that contains the encrypted password (full path and name)
        /// </summary>
        private string m_credPFileName;

        /// <summary>
        /// Name of the file that contains the encrypted user name (full path and name)
        /// </summary>
        private string m_credUFileName;

        /// <summary>
        /// Name of the directory used to store local files related to the OSBLE Plus Word Addin
        /// </summary>
        private string m_localDirectory;

        private OSBLEState m_state = null;

        private static byte[] s_key = new byte[]{
            185,204,123,80,94,243,206,55,111,36,53,197,43,58,87,197,244,6,181,81,235,
            249,34,106,119,93,36,29,195,106,237,113
        };

        private static byte[] s_iv = new byte[]{
            240,218,75,92,242,171,14,143,97,50,227,197,9,242,206,202
        };

        /// <summary>
        /// Assign paths to OSBLE directory and credential files
        /// </summary>
        public frmOSBLELogin()
        {
            InitializeComponent();

            try
            {
                //set local directory to the local app data directory and append the name of the OSBLE directory
                m_localDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                m_localDirectory = Path.Combine(m_localDirectory, StringConstants.LocalFolder);

                //create local directory if it does not currently exist
                if (!Directory.Exists(m_localDirectory))
                    Directory.CreateDirectory(m_localDirectory);

                //set paths to credential files
                m_credUFileName = Path.Combine(m_localDirectory, "OSBLE_Word_u.dat");
                m_credPFileName = Path.Combine(m_localDirectory, "OSBLE_Word_p.dat");
            }

            catch(Exception) { }
        }

        /// <summary>
        /// The event called when a login has been submitted. The event will
        /// default the user interface of the form and deactivate the
        /// buttons. Updates will take place on another thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Ignore the click if we have an empty user name or password
            if (0 == txtUsername.TextLength)
            {
                txtUsername.Focus();
                return;
            }
            if (0 == txtPassword.TextLength)
            {
                txtPassword.Focus();
                return;
            }

            //reset error text box and size of the form
            txtErrors.Text = "";
            txtErrors.Visible = false;
            this.Size = new System.Drawing.Size(300, 194);

            // Disable the buttons while we're processing
            btnSubmit.Enabled = false;
            btnCancel.Enabled = false;
            // Also show the progress bar
            pbLogin.Visible = true;

            m_state = new OSBLEState(txtUsername.Text, txtPassword.Text);
            m_state.RefreshAsync(this.LoginAttemptCompleted_CT);
        }

        /// <summary>
        /// Simply checks if the login was successful before returning the login state
        /// </summary>
        /// <returns>OSBLE state of the logged in user</returns>
        public OSBLEState DoPrompt()
        {
            if (this.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return m_state;
        }

        /// <summary>
        /// Returns an decrypted string from an encrypted file
        /// </summary>
        /// <param name="fileName">Name of file containing encrypted string</param>
        /// <returns></returns>
        private static string LoadEncrypted(string fileName)
        {
            // Load encrypted file
            try
            {
                byte[] fileBytes = File.ReadAllBytes(fileName);
                if (null == fileBytes || fileBytes.Length < 6)
                {
                    File.Delete(fileName);
                    return null;
                }

                return DecryptStringFromBytes(fileBytes, s_key, s_iv);
            }
            catch (Exception ex) { return null; }
        }

        /// <summary>
        /// The event called after a login attempt has been completed. The event
        /// will first determine if the login was successful, if there was an error
        /// then a error message will be displayed on the form. Otherwise, the form
        /// will be closed and the credentials will be updated in the local app data
        /// according to the selected form settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginAttemptCompleted(object sender, EventArgs e)
        {
            pbLogin.Visible = false;
            btnSubmit.Enabled = true;
            btnCancel.Enabled = true;

            OSBLEStateEventArgs oe = e as OSBLEStateEventArgs;
            if (!oe.Success)
            {
                //insert message on new line if errors already exist
                if (!String.IsNullOrEmpty(txtErrors.Text))
                    txtErrors.Text += "\r\n";

                txtErrors.Text += oe.Message;

                //adjust size of the form
                if (!String.IsNullOrEmpty(txtErrors.Text))
                {
                    txtErrors.Visible = true;

                    using (Graphics g = CreateGraphics())
                    {
                        //determine height of the error text box to determine new height of the form
                        SizeF size = g.MeasureString(txtErrors.Text, txtErrors.Font);
                        txtErrors.Height = (int)Math.Ceiling(size.Height - 12 + size.Width / txtErrors.Font.Height);
                    }

                    this.Size = new System.Drawing.Size(this.Width, this.Height + txtErrors.Height);
                }

                return;
            }

            this.DialogResult = DialogResult.OK;

            // Save user name and password if login was successful and box is checked
            if (chkRememberCredentials.Checked)
            {
                byte[] encUser = EncryptStringToBytes(txtUsername.Text, s_key, s_iv);
                byte[] encPass = EncryptStringToBytes(txtPassword.Text, s_key, s_iv);
                try
                {
                    File.WriteAllBytes(m_credUFileName, encUser);
                    File.WriteAllBytes(m_credPFileName, encPass);
                }
                catch (Exception) { }
            }

            // Otherwise delete save files
            else
            {
                if (File.Exists(m_credUFileName)) { File.Delete(m_credUFileName); }
                if (File.Exists(m_credPFileName)) { File.Delete(m_credPFileName); }
            }
        }

        /// <summary>
        /// Process the login attempt on another thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginAttemptCompleted_CT(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(LoginAttemptCompleted), sender, e);
        }

        #region Cryptography methods from http://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
        #endregion

        /// <summary>
        /// The event called when the login form is displayed. The event will
        /// first determine if the credentials exist and load them into the
        /// login form if found. Otherwise, the login form will be left blank.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOSBLELogin_Load(object sender, EventArgs e)
        {
            if (!File.Exists(m_credUFileName) || !File.Exists(m_credPFileName))
            {
                // No credentials file means that they didn't save their user 
                // name and password last time.
                chkRememberCredentials.Checked = false;
                return;
            }

            string userName = LoadEncrypted(m_credUFileName);
            string password = LoadEncrypted(m_credPFileName);

            if (string.IsNullOrEmpty(userName) || (string.IsNullOrEmpty(password)))
            {
                // Empty credential files means a file was not saved correctly
                // and we cannot import an empty username or password.
                chkRememberCredentials.Checked = false;
                return;
            }


            txtUsername.Text = userName;
            txtPassword.Text = password;
            chkRememberCredentials.Checked = true;
        }

        /// <summary>
        /// Simply closes the login form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmOSBLELogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            //TODO: Remove when done testing
            //Document currentDocument = Globals.ThisAddIn.GetActiveDocument();
            //var paragraphs = currentDocument.Paragraphs;
            //var temp = currentDocument.Content.Text;
            //var temp2 = currentDocument.Content.Underline;
            //var temp3 = currentDocument.Content.SpellingErrors;
            //MessageBox.Show("TEST");
        }
    }
}
