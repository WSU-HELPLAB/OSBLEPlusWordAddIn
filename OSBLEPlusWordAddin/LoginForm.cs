﻿using OSBLEPlusWordAddin;
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

        private OSBLEState m_state = null;

        private static byte[] s_key = new byte[]{
            185,204,123,80,94,243,206,55,111,36,53,197,43,58,87,197,244,6,181,81,235,
            249,34,106,119,93,36,29,195,106,237,113
        };

        private static byte[] s_iv = new byte[]{
            240,218,75,92,242,171,14,143,97,50,227,197,9,242,206,202
        };
        public frmOSBLELogin()
        {
            InitializeComponent();

            m_credUFileName = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            m_credUFileName = Path.Combine(
                m_credUFileName, "OSBLE_Word_u.dat");
            m_credPFileName = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            m_credPFileName = Path.Combine(
                m_credPFileName, "OSBLE_Word_u.dat");
        }

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

            // Disable the buttons while we're processing
            btnSubmit.Enabled = false;
            btnCancel.Enabled = false;
            // Also show the progress bar
            pbLogin.Visible = true;

            // Save user name and password if needed
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
            else // Otherwise delete save files
            {
                if (File.Exists(m_credUFileName)) { File.Delete(m_credUFileName); }
                if (File.Exists(m_credPFileName)) { File.Delete(m_credPFileName); }
            }

            m_state = new OSBLEState(txtUsername.Text, txtPassword.Text);
            m_state.RefreshAsync(this.LoginAttemptCompleted_CT);
        }

        public OSBLEState DoPrompt()
        {
            if (this.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return m_state;
        }

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

        private void LoginAttemptCompleted(object sender, EventArgs e)
        {
            pbLogin.Visible = false;
            btnSubmit.Enabled = true;
            btnCancel.Enabled = true;

            OSBLEStateEventArgs oe = e as OSBLEStateEventArgs;
            if (!oe.Success)
            {
                MessageBox.Show(this, oe.Message, "OSBLE Login");
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        // CT = cross-thread
        private void LoginAttemptCompleted_CT(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(LoginAttemptCompleted), sender, e);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists(m_credUFileName))
            {
                // No credentials file means that they didn't save their user 
                // name and password last time.
                chkRememberCredentials.Checked = false;
                return;
            }

            chkRememberCredentials.Checked = true;

            string userName = LoadEncrypted(m_credUFileName);
            if (string.IsNullOrEmpty(userName))
            {
                return;
            }
            txtUsername.Text = userName;

            // Now the password
            if (File.Exists(m_credPFileName))
            {
                string password = LoadEncrypted(m_credPFileName);
                if (string.IsNullOrEmpty(password))
                {
                    return;
                }
                txtPassword.Text = password;
            }
            else
            {
                txtPassword.Text = string.Empty;
            }
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

        private void frmOSBLELogin_Load(object sender, EventArgs e)
        {

        }
    }
}
