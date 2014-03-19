using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSIS_Config_WinForm
{
    public partial class frmPkgPassword : Form
    {
        public string pkgPassword=String.Empty;
        public frmPkgPassword()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            pkgPassword = txtPassword.Text;
            this.Close();
        }

        public void LoadData(string pkgName)
        {
            lblPackageName.Text += pkgName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
