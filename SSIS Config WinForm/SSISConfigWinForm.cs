using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SSIS_Config_BLL;

namespace SSIS_Config_WinForm
{
    public partial class SSISConfigWinForm : Form
    {
        public string filepath = string.Empty;
        public string SSISVarXMLPath = string.Empty;
        public SSISConfigWinForm()
        {
            InitializeComponent();
        }
        private void SSISConfigWinForm_Load(object sender, EventArgs e)
        {

        }

        #region Button Events
        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                txtDirectoryPath.Text = op.SelectedPath;
                filepath = op.SelectedPath;
            }

        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = @"C:\";
            op.Filter = "dtsx Package | *.dtsx";
            op.Multiselect = false;

            if (op.ShowDialog() == DialogResult.OK)
            {
                txtFileUpload.Text = op.FileName;
                filepath = op.FileName;

            }
        }

        private void btnModifyPackage_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            if (tabPath.SelectedTab == tabFilePath)
            {
                FileOperation(true);
            }
            else
            {
                DirectoryOperation(true);
            }
        }

        private void btnAnalysePkg_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Show();

            if (tabPath.SelectedTab == tabFilePath)
            {
                filepath = txtFileUpload.Text;
                FileOperation(false);
            }
            else
            {
                filepath = txtDirectoryPath.Text;
                DirectoryOperation(false);
            }
        }

        private void btnCreatePkg_Click(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region File-Directory Operation

        private void FileOperation(bool modifySSIS)
        {
            try
            {

                List<OutputEntity> ListOutputObj = new List<OutputEntity>();
                CheckPkgPreRequisite(ref ListOutputObj, modifySSIS);

                //try
                {
                    PerformSSISReview(filepath, ref ListOutputObj, modifySSIS,SSISVarXMLPath );
                    displayGrid(ref ListOutputObj);
                }


                

            }
            catch (Exception e)
            {
                MessageBox.Show("Error in the application \n" + e.Message.ToString(), "Application Error");
            }
        }

        private void DirectoryOperation(bool modifySSIS)
        {
            try
            {
                StringBuilder errorList=new StringBuilder();
                DirectoryInfo di = null;
                di = new DirectoryInfo(txtDirectoryPath.Text.ToString());
                var directories = di.GetFiles("*.dtsx", SearchOption.AllDirectories);

                List<OutputEntity> ListOutputObj = new List<OutputEntity>();
                CheckPkgPreRequisite(ref ListOutputObj, modifySSIS);

                foreach (FileInfo file in directories)
                {
                    filepath = file.FullName;
                    try
                    {
                        PerformSSISReview(filepath, ref ListOutputObj, modifySSIS,SSISVarXMLPath );
                    }
                    catch(Exception)
                    {
                        errorList.AppendLine(filepath);
                    }
                }

                displayGrid(ref ListOutputObj);

                if (errorList.Length>0)
                {
                    string errorMsg="Unable to review following package(s):\n" + errorList.ToString();
                    MessageBox.Show(errorMsg ,"Error in package review/modification");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in the application \n" + e.Message.ToString(), "Application Error");
            }
        }

        private static string GetPkgPassword(string pkgFilePath)
        {
            string[] pkgFilePathSplit = pkgFilePath.Split('\\');
            string pkgName = pkgFilePathSplit.LastOrDefault().ToString();
            frmPkgPassword frmPwd = new frmPkgPassword();
            frmPwd.LoadData(pkgName);
            frmPwd.ShowDialog();

            return frmPwd.pkgPassword.ToString();
        }

        #endregion

        private void CheckPkgPreRequisite(ref List<OutputEntity> ListOutputObj, bool modifySSIS)
        {
            OutputEntity mOutputObj = new OutputEntity();
            LogEntity mLogEntity = new LogEntity();


            mOutputObj.pkgFilePath = filepath;
            mOutputObj.modifySSIS = modifySSIS;
            
            SSISConfigLib objSSISConfigLib = new SSISConfigLib();
            mOutputObj.logEnt.Add(objSSISConfigLib.CheckEnvVar(modifySSIS));
            mOutputObj.logEnt.Add(objSSISConfigLib.CheckDTSConfigPath());

            //FETCH XML file Path
            mLogEntity = objSSISConfigLib.CheckSSISVarXMLPath();
            SSISVarXMLPath = mLogEntity.ComponentValue;
            mOutputObj.logEnt.Add(mLogEntity);


            mOutputObj.isSuccess = true;
            ListOutputObj.Add(mOutputObj);
        }

        private void PerformSSISReview(string filepath, ref List<OutputEntity> ListOutputObj, bool modifySSIS, string SSISVarXMLPath)
        {
            
            OutputEntity mOutputObj = null;
            if ((filepath != "" && filepath.EndsWith(".dtsx")))
            {
                //try
                //{
                    mOutputObj = new OutputEntity();
                    mOutputObj.pkgFilePath = filepath;
                    mOutputObj.modifySSIS = modifySSIS;

                    SSISConfigureBO objSSISConfigure = new SSISConfigureBO();
                    objSSISConfigure.CheckModifySSIS(ref mOutputObj, SSISVarXMLPath);

                    if (mOutputObj.isPkgPwdProtected)
                    {
                        mOutputObj.pkgPassword = GetPkgPassword(mOutputObj.pkgFilePath);
                        objSSISConfigure.CheckModifySSIS(ref mOutputObj, SSISVarXMLPath);
                    }
                    ListOutputObj.Add(mOutputObj);
                    mOutputObj = null;
                //}
                //catch (Exception e)
                //{
                //   // MessageBox.Show("Unable to review/modify \n" + e.Message.ToString(), "Error");

                //}
            }
            else
            {
                MessageBox.Show("Invalid file format: \n" + filepath, "Invalid File format");
            }

        }

        #region Display Grid
        private void displayGrid(ref List<OutputEntity> ListOutputObj)
        {
            
            List<LogEntity> listlogEnt = new List<LogEntity>();

            var FinalListOutputObj = (from a in ListOutputObj where a.isSuccess == Convert.ToBoolean(true) 
                                        select a).Distinct();

            foreach (OutputEntity mOutputObj in FinalListOutputObj)
            {
                //if (mOutputObj.isSuccess)
                {
                    listlogEnt.AddRange(mOutputObj.logEnt);
                }
            }
            FormatDataGrid(listlogEnt);
            dataGridView1.Visible = true;

        }

        private void FormatDataGrid(List<LogEntity> logEnt)
        {
            dataGridView1.DataSource = logEnt;

            //DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
            //{
            //    column.HeaderText = "Modify";
            //    column.Name = "Modify";
            //    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //    column.FlatStyle = FlatStyle.Standard;
            //    column.ThreeState = false;
            //    column.CellTemplate = new DataGridViewCheckBoxCell();
            //}
            //dataGridView1.Columns.Insert(0, column);

            //dataGridView1.Columns[0].HeaderText = "Modify";
            dataGridView1.Columns[0].HeaderText = "Package Name";
            dataGridView1.Columns[1].HeaderText = "Component";
            dataGridView1.Columns[2].HeaderText = "Component Value";
            dataGridView1.Columns[3].HeaderText = "Test Status";

            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{
            //    dataGridView1.Rows[i].Cells["Modify"].Value = Convert.ToBoolean(logEnt[i].isModify);
            //}

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
       }
        #endregion

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                //dataGridView1.Rows[i].Cells["Modify"].Value = ;
            }

        }

       
    }
}
