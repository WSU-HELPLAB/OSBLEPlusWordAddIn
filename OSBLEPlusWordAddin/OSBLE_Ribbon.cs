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
        private void OSBLE_Ribbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RibbonControlEventArgs e)
        {
            //MessageBox.Show("Click works");
            Form loginForm = new frmOSBLELogin();
            loginForm.Show();
        }
    }
}
