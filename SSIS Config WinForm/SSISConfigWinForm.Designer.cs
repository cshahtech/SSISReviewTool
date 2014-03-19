namespace SSIS_Config_WinForm
{
    partial class SSISConfigWinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlBackGround = new System.Windows.Forms.Panel();
            this.btnCreatePkg = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnModifyPackage = new System.Windows.Forms.Button();
            this.tabPath = new System.Windows.Forms.TabControl();
            this.tabFilePath = new System.Windows.Forms.TabPage();
            this.grpFileUpload = new System.Windows.Forms.GroupBox();
            this.txtFileUpload = new System.Windows.Forms.TextBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.tabDirectoryPath = new System.Windows.Forms.TabPage();
            this.grpFolderUpload = new System.Windows.Forms.GroupBox();
            this.btnBrowseUploadFolder = new System.Windows.Forms.Button();
            this.txtDirectoryPath = new System.Windows.Forms.TextBox();
            this.btnAnalysePkg = new System.Windows.Forms.Button();
            this.pnlBackGround.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPath.SuspendLayout();
            this.tabFilePath.SuspendLayout();
            this.grpFileUpload.SuspendLayout();
            this.tabDirectoryPath.SuspendLayout();
            this.grpFolderUpload.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBackGround
            // 
            this.pnlBackGround.BackColor = System.Drawing.Color.Azure;
            this.pnlBackGround.Controls.Add(this.btnCreatePkg);
            this.pnlBackGround.Controls.Add(this.lblResult);
            this.pnlBackGround.Controls.Add(this.dataGridView1);
            this.pnlBackGround.Controls.Add(this.btnModifyPackage);
            this.pnlBackGround.Controls.Add(this.tabPath);
            this.pnlBackGround.Controls.Add(this.btnAnalysePkg);
            this.pnlBackGround.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackGround.Location = new System.Drawing.Point(0, 0);
            this.pnlBackGround.Name = "pnlBackGround";
            this.pnlBackGround.Size = new System.Drawing.Size(988, 493);
            this.pnlBackGround.TabIndex = 0;
            // 
            // btnCreatePkg
            // 
            this.btnCreatePkg.BackColor = System.Drawing.Color.Green;
            this.btnCreatePkg.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreatePkg.ForeColor = System.Drawing.Color.White;
            this.btnCreatePkg.Location = new System.Drawing.Point(745, 3);
            this.btnCreatePkg.Name = "btnCreatePkg";
            this.btnCreatePkg.Size = new System.Drawing.Size(140, 35);
            this.btnCreatePkg.TabIndex = 10;
            this.btnCreatePkg.Text = "Create Package";
            this.btnCreatePkg.UseVisualStyleBackColor = false;
            this.btnCreatePkg.Visible = false;
            this.btnCreatePkg.Click += new System.EventHandler(this.btnCreatePkg_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(37, 150);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(125, 19);
            this.lblResult.TabIndex = 9;
            this.lblResult.Text = "Result of Analysis";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Azure;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(24, 174);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(936, 307);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // btnModifyPackage
            // 
            this.btnModifyPackage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnModifyPackage.Enabled = false;
            this.btnModifyPackage.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModifyPackage.ForeColor = System.Drawing.Color.DimGray;
            this.btnModifyPackage.Location = new System.Drawing.Point(745, 97);
            this.btnModifyPackage.Name = "btnModifyPackage";
            this.btnModifyPackage.Size = new System.Drawing.Size(140, 35);
            this.btnModifyPackage.TabIndex = 7;
            this.btnModifyPackage.Text = "Modify Package";
            this.btnModifyPackage.UseVisualStyleBackColor = false;
            this.btnModifyPackage.Visible = false;
            this.btnModifyPackage.Click += new System.EventHandler(this.btnModifyPackage_Click);
            // 
            // tabPath
            // 
            this.tabPath.Controls.Add(this.tabFilePath);
            this.tabPath.Controls.Add(this.tabDirectoryPath);
            this.tabPath.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPath.Location = new System.Drawing.Point(159, 34);
            this.tabPath.Name = "tabPath";
            this.tabPath.SelectedIndex = 0;
            this.tabPath.Size = new System.Drawing.Size(541, 96);
            this.tabPath.TabIndex = 6;
            // 
            // tabFilePath
            // 
            this.tabFilePath.Controls.Add(this.grpFileUpload);
            this.tabFilePath.Location = new System.Drawing.Point(4, 24);
            this.tabFilePath.Name = "tabFilePath";
            this.tabFilePath.Padding = new System.Windows.Forms.Padding(3);
            this.tabFilePath.Size = new System.Drawing.Size(533, 68);
            this.tabFilePath.TabIndex = 0;
            this.tabFilePath.Text = "File Path";
            this.tabFilePath.UseVisualStyleBackColor = true;
            // 
            // grpFileUpload
            // 
            this.grpFileUpload.Controls.Add(this.txtFileUpload);
            this.grpFileUpload.Controls.Add(this.btnBrowseFile);
            this.grpFileUpload.Location = new System.Drawing.Point(-4, 9);
            this.grpFileUpload.Name = "grpFileUpload";
            this.grpFileUpload.Size = new System.Drawing.Size(531, 52);
            this.grpFileUpload.TabIndex = 1;
            this.grpFileUpload.TabStop = false;
            this.grpFileUpload.Text = "File path of the SSIS Package";
            // 
            // txtFileUpload
            // 
            this.txtFileUpload.Location = new System.Drawing.Point(11, 16);
            this.txtFileUpload.Name = "txtFileUpload";
            this.txtFileUpload.Size = new System.Drawing.Size(420, 23);
            this.txtFileUpload.TabIndex = 1;
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.BackColor = System.Drawing.Color.White;
            this.btnBrowseFile.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseFile.ForeColor = System.Drawing.Color.Black;
            this.btnBrowseFile.Location = new System.Drawing.Point(435, 16);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(80, 23);
            this.btnBrowseFile.TabIndex = 0;
            this.btnBrowseFile.Text = "Browse";
            this.btnBrowseFile.UseVisualStyleBackColor = false;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // tabDirectoryPath
            // 
            this.tabDirectoryPath.Controls.Add(this.grpFolderUpload);
            this.tabDirectoryPath.Location = new System.Drawing.Point(4, 24);
            this.tabDirectoryPath.Name = "tabDirectoryPath";
            this.tabDirectoryPath.Padding = new System.Windows.Forms.Padding(3);
            this.tabDirectoryPath.Size = new System.Drawing.Size(533, 68);
            this.tabDirectoryPath.TabIndex = 1;
            this.tabDirectoryPath.Text = "Directory Path";
            this.tabDirectoryPath.UseVisualStyleBackColor = true;
            // 
            // grpFolderUpload
            // 
            this.grpFolderUpload.Controls.Add(this.btnBrowseUploadFolder);
            this.grpFolderUpload.Controls.Add(this.txtDirectoryPath);
            this.grpFolderUpload.Location = new System.Drawing.Point(-4, 9);
            this.grpFolderUpload.Name = "grpFolderUpload";
            this.grpFolderUpload.Size = new System.Drawing.Size(531, 52);
            this.grpFolderUpload.TabIndex = 3;
            this.grpFolderUpload.TabStop = false;
            this.grpFolderUpload.Text = "Directory path of the SSIS Package(s)";
            // 
            // btnBrowseUploadFolder
            // 
            this.btnBrowseUploadFolder.Location = new System.Drawing.Point(435, 16);
            this.btnBrowseUploadFolder.Name = "btnBrowseUploadFolder";
            this.btnBrowseUploadFolder.Size = new System.Drawing.Size(80, 23);
            this.btnBrowseUploadFolder.TabIndex = 1;
            this.btnBrowseUploadFolder.Text = "Browse";
            this.btnBrowseUploadFolder.UseVisualStyleBackColor = true;
            this.btnBrowseUploadFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtDirectoryPath
            // 
            this.txtDirectoryPath.Location = new System.Drawing.Point(11, 16);
            this.txtDirectoryPath.Name = "txtDirectoryPath";
            this.txtDirectoryPath.Size = new System.Drawing.Size(420, 23);
            this.txtDirectoryPath.TabIndex = 0;
            // 
            // btnAnalysePkg
            // 
            this.btnAnalysePkg.BackColor = System.Drawing.Color.White;
            this.btnAnalysePkg.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalysePkg.ForeColor = System.Drawing.Color.Black;
            this.btnAnalysePkg.Location = new System.Drawing.Point(745, 53);
            this.btnAnalysePkg.Name = "btnAnalysePkg";
            this.btnAnalysePkg.Size = new System.Drawing.Size(140, 35);
            this.btnAnalysePkg.TabIndex = 5;
            this.btnAnalysePkg.Text = "Analyze Package";
            this.btnAnalysePkg.UseVisualStyleBackColor = false;
            this.btnAnalysePkg.Click += new System.EventHandler(this.btnAnalysePkg_Click);
            // 
            // SSISConfigWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(988, 493);
            this.Controls.Add(this.pnlBackGround);
            this.MaximizeBox = false;
            this.Name = "SSISConfigWinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SSIS Configuration Review";
            this.Load += new System.EventHandler(this.SSISConfigWinForm_Load);
            this.pnlBackGround.ResumeLayout(false);
            this.pnlBackGround.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPath.ResumeLayout(false);
            this.tabFilePath.ResumeLayout(false);
            this.grpFileUpload.ResumeLayout(false);
            this.grpFileUpload.PerformLayout();
            this.tabDirectoryPath.ResumeLayout(false);
            this.grpFolderUpload.ResumeLayout(false);
            this.grpFolderUpload.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBackGround;
        private System.Windows.Forms.Button btnAnalysePkg;
        private System.Windows.Forms.TabControl tabPath;
        private System.Windows.Forms.TabPage tabFilePath;
        private System.Windows.Forms.GroupBox grpFileUpload;
        private System.Windows.Forms.TextBox txtFileUpload;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TabPage tabDirectoryPath;
        private System.Windows.Forms.GroupBox grpFolderUpload;
        private System.Windows.Forms.Button btnBrowseUploadFolder;
        private System.Windows.Forms.TextBox txtDirectoryPath;
        private System.Windows.Forms.Button btnModifyPackage;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnCreatePkg;

    }
}