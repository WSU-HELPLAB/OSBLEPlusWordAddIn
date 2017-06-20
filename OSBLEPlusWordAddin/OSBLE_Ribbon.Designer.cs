namespace OSBLEPlusWordAddin
{
    partial class OSBLE_Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public OSBLE_Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OSBLE = this.Factory.CreateRibbonTab();
            this.OSBLE_Login = this.Factory.CreateRibbonGroup();
            this.btnLogin = this.Factory.CreateRibbonButton();
            this.btnLogout = this.Factory.CreateRibbonButton();
            this.grpOSBLEOptions = this.Factory.CreateRibbonGroup();
            this.btnUpload = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.dropDownCourse = this.Factory.CreateRibbonDropDown();
            this.lblLastSave = this.Factory.CreateRibbonLabel();
            this.OSBLE.SuspendLayout();
            this.OSBLE_Login.SuspendLayout();
            this.grpOSBLEOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // OSBLE
            // 
            this.OSBLE.Groups.Add(this.OSBLE_Login);
            this.OSBLE.Groups.Add(this.grpOSBLEOptions);
            this.OSBLE.Label = "OSBLE+";
            this.OSBLE.Name = "OSBLE";
            // 
            // OSBLE_Login
            // 
            this.OSBLE_Login.Items.Add(this.btnLogin);
            this.OSBLE_Login.Items.Add(this.btnLogout);
            this.OSBLE_Login.Label = "OSBLE+ Account";
            this.OSBLE_Login.Name = "OSBLE_Login";
            // 
            // btnLogin
            // 
            this.btnLogin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLogin.Label = "Sign in...";
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.OfficeImageId = "AdpPrimaryKey";
            this.btnLogin.ScreenTip = "Prompts you for a user name and password to sign into OSBLE+";
            this.btnLogin.ShowImage = true;
            this.btnLogin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLogout.Label = "Sign out...";
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.OfficeImageId = "Delete";
            this.btnLogout.ScreenTip = "Sign out of OSBLE+";
            this.btnLogout.ShowImage = true;
            this.btnLogout.Visible = false;
            this.btnLogout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLogout_Click);
            // 
            // grpOSBLEOptions
            // 
            this.grpOSBLEOptions.Items.Add(this.btnUpload);
            this.grpOSBLEOptions.Items.Add(this.separator1);
            this.grpOSBLEOptions.Items.Add(this.dropDownCourse);
            this.grpOSBLEOptions.Items.Add(this.lblLastSave);
            this.grpOSBLEOptions.Label = "OSBLE+ Course Options";
            this.grpOSBLEOptions.Name = "grpOSBLEOptions";
            this.grpOSBLEOptions.Visible = false;
            // 
            // btnUpload
            // 
            this.btnUpload.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnUpload.Label = "Upload to OSBLE+";
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.OfficeImageId = "UpgradeDocument";
            this.btnUpload.ShowImage = true;
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // dropDownCourse
            // 
            this.dropDownCourse.Label = "Course: ";
            this.dropDownCourse.Name = "dropDownCourse";
            this.dropDownCourse.SizeString = "12/31/20XX 11:59:59 PM";
            // 
            // lblLastSave
            // 
            this.lblLastSave.Label = "Last save: ";
            this.lblLastSave.Name = "lblLastSave";
            // 
            // OSBLE_Ribbon
            // 
            this.Name = "OSBLE_Ribbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.OSBLE);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.OSBLE_Ribbon_Load);
            this.OSBLE.ResumeLayout(false);
            this.OSBLE.PerformLayout();
            this.OSBLE_Login.ResumeLayout(false);
            this.OSBLE_Login.PerformLayout();
            this.grpOSBLEOptions.ResumeLayout(false);
            this.grpOSBLEOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab OSBLE;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup OSBLE_Login;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLogin;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpOSBLEOptions;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnUpload;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown dropDownCourse;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblLastSave;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLogout;
    }

    partial class ThisRibbonCollection
    {
        internal OSBLE_Ribbon OSBLE_Ribbon
        {
            get { return this.GetRibbon<OSBLE_Ribbon>(); }
        }
    }
}
