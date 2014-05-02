namespace TimeTraveller.Tools.Sparx.ObjectModelGen
{
    partial class SelectModel
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtRepositoryName = new System.Windows.Forms.TextBox();
            this.cmdModelGenerate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.cmdChooseModelFile = new System.Windows.Forms.Button();
            this.cmdChooseOutputDirectory = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "EA Model :";
            // 
            // txtRepositoryName
            // 
            this.txtRepositoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRepositoryName.Location = new System.Drawing.Point(106, 12);
            this.txtRepositoryName.Name = "txtRepositoryName";
            this.txtRepositoryName.Size = new System.Drawing.Size(430, 20);
            this.txtRepositoryName.TabIndex = 1;
            this.txtRepositoryName.Text = "C:\\usr\\try\\EAObjectModelGenerator\\TestRepository.eap";
            // 
            // cmdModelGenerate
            // 
            this.cmdModelGenerate.Location = new System.Drawing.Point(106, 66);
            this.cmdModelGenerate.Name = "cmdModelGenerate";
            this.cmdModelGenerate.Size = new System.Drawing.Size(164, 23);
            this.cmdModelGenerate.TabIndex = 4;
            this.cmdModelGenerate.Text = "&Generate models";
            this.cmdModelGenerate.UseVisualStyleBackColor = true;
            this.cmdModelGenerate.Click += new System.EventHandler(this.cmdGenerate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Output directory :";
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDirectory.Location = new System.Drawing.Point(106, 40);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.Size = new System.Drawing.Size(430, 20);
            this.txtOutputDirectory.TabIndex = 6;
            this.txtOutputDirectory.Text = "C:\\usr\\try\\EAObjectModelGenerator\\GeneratedInterfaces";
            // 
            // cmdChooseModelFile
            // 
            this.cmdChooseModelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseModelFile.Location = new System.Drawing.Point(542, 12);
            this.cmdChooseModelFile.Name = "cmdChooseModelFile";
            this.cmdChooseModelFile.Size = new System.Drawing.Size(39, 23);
            this.cmdChooseModelFile.TabIndex = 7;
            this.cmdChooseModelFile.Text = "...";
            this.cmdChooseModelFile.UseVisualStyleBackColor = true;
            this.cmdChooseModelFile.Click += new System.EventHandler(this.cmdChooseModelFile_Click);
            // 
            // cmdChooseOutputDirectory
            // 
            this.cmdChooseOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChooseOutputDirectory.Location = new System.Drawing.Point(542, 40);
            this.cmdChooseOutputDirectory.Name = "cmdChooseOutputDirectory";
            this.cmdChooseOutputDirectory.Size = new System.Drawing.Size(39, 23);
            this.cmdChooseOutputDirectory.TabIndex = 8;
            this.cmdChooseOutputDirectory.Text = "...";
            this.cmdChooseOutputDirectory.UseVisualStyleBackColor = true;
            this.cmdChooseOutputDirectory.Click += new System.EventHandler(this.cmdChooseOutputDirectory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 106);
            this.Controls.Add(this.cmdChooseOutputDirectory);
            this.Controls.Add(this.cmdChooseModelFile);
            this.Controls.Add(this.txtOutputDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdModelGenerate);
            this.Controls.Add(this.txtRepositoryName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "TimeTraveller Object Model Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRepositoryName;
        private System.Windows.Forms.Button cmdModelGenerate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOutputDirectory;
        private System.Windows.Forms.Button cmdChooseModelFile;
        private System.Windows.Forms.Button cmdChooseOutputDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

