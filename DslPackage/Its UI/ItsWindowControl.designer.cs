/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Luminis.Its.Workbench.DslPackage
{
	partial class ItsWindowControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Forms.ColumnHeader.set_Text(System.String)")]
        private void InitializeComponent()
        {
            this.LoadObjectModelButton = new System.Windows.Forms.Button();
            this.ITSServerTextBox = new System.Windows.Forms.TextBox();
            this.serverPanel = new System.Windows.Forms.Panel();
            this.ItsServerLabel = new System.Windows.Forms.Label();
            this.ObjectModelListBox = new System.Windows.Forms.ComboBox();
            this.ModelPanel = new System.Windows.Forms.Panel();
            this.ObjectModelLabel = new System.Windows.Forms.Label();
            this.CommandPanel = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SpecsTree = new System.Windows.Forms.TreeView();
            this.SpecsContainter = new System.Windows.Forms.SplitContainer();
            this.PreviewButton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.serverPanel.SuspendLayout();
            this.ModelPanel.SuspendLayout();
            this.CommandPanel.SuspendLayout();
            this.SpecsContainter.Panel1.SuspendLayout();
            this.SpecsContainter.Panel2.SuspendLayout();
            this.SpecsContainter.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadObjectModelButton
            // 
            this.LoadObjectModelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.LoadObjectModelButton.Location = new System.Drawing.Point(311, 0);
            this.LoadObjectModelButton.Name = "LoadObjectModelButton";
            this.LoadObjectModelButton.Size = new System.Drawing.Size(36, 28);
            this.LoadObjectModelButton.TabIndex = 1;
            this.LoadObjectModelButton.Text = ">";
            this.LoadObjectModelButton.UseVisualStyleBackColor = true;
            this.LoadObjectModelButton.Click += new System.EventHandler(this.LoadObjectModelButton_Click);
            // 
            // ITSServerTextBox
            // 
            this.ITSServerTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ITSServerTextBox.Location = new System.Drawing.Point(60, 0);
            this.ITSServerTextBox.Name = "ITSServerTextBox";
            this.ITSServerTextBox.Size = new System.Drawing.Size(251, 22);
            this.ITSServerTextBox.TabIndex = 0;
            this.ITSServerTextBox.Text = "http://localhost:8080/its";
            // 
            // serverPanel
            // 
            this.serverPanel.Controls.Add(this.ITSServerTextBox);
            this.serverPanel.Controls.Add(this.ItsServerLabel);
            this.serverPanel.Controls.Add(this.LoadObjectModelButton);
            this.serverPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.serverPanel.Location = new System.Drawing.Point(0, 0);
            this.serverPanel.Name = "serverPanel";
            this.serverPanel.Size = new System.Drawing.Size(347, 28);
            this.serverPanel.TabIndex = 3;
            // 
            // ItsServerLabel
            // 
            this.ItsServerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ItsServerLabel.Location = new System.Drawing.Point(0, 0);
            this.ItsServerLabel.Name = "ItsServerLabel";
            this.ItsServerLabel.Size = new System.Drawing.Size(60, 28);
            this.ItsServerLabel.TabIndex = 2;
            this.ItsServerLabel.Text = "Server";
            // 
            // ObjectModelListBox
            // 
            this.ObjectModelListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectModelListBox.FormattingEnabled = true;
            this.ObjectModelListBox.Location = new System.Drawing.Point(60, 0);
            this.ObjectModelListBox.Name = "ObjectModelListBox";
            this.ObjectModelListBox.Size = new System.Drawing.Size(287, 24);
            this.ObjectModelListBox.TabIndex = 4;
            this.ObjectModelListBox.SelectedIndexChanged += new System.EventHandler(this.ObjectModelListBox_SelectedIndexChanged);
            // 
            // ModelPanel
            // 
            this.ModelPanel.Controls.Add(this.ObjectModelListBox);
            this.ModelPanel.Controls.Add(this.ObjectModelLabel);
            this.ModelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ModelPanel.Location = new System.Drawing.Point(0, 28);
            this.ModelPanel.Name = "ModelPanel";
            this.ModelPanel.Size = new System.Drawing.Size(347, 27);
            this.ModelPanel.TabIndex = 5;
            // 
            // ObjectModelLabel
            // 
            this.ObjectModelLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ObjectModelLabel.Location = new System.Drawing.Point(0, 0);
            this.ObjectModelLabel.Name = "ObjectModelLabel";
            this.ObjectModelLabel.Size = new System.Drawing.Size(60, 27);
            this.ObjectModelLabel.TabIndex = 5;
            this.ObjectModelLabel.Text = "Model";
            // 
            // CommandPanel
            // 
            this.CommandPanel.Controls.Add(this.PreviewButton);
            this.CommandPanel.Controls.Add(this.CancelButton);
            this.CommandPanel.Controls.Add(this.SaveButton);
            this.CommandPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.CommandPanel.Location = new System.Drawing.Point(0, 174);
            this.CommandPanel.Name = "CommandPanel";
            this.CommandPanel.Size = new System.Drawing.Size(347, 33);
            this.CommandPanel.TabIndex = 6;
            // 
            // CancelButton
            // 
            this.CancelButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.CancelButton.Location = new System.Drawing.Point(75, 0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 33);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            this.SaveButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.SaveButton.Location = new System.Drawing.Point(0, 0);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 33);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // SpecsTree
            // 
            this.SpecsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpecsTree.Location = new System.Drawing.Point(0, 0);
            this.SpecsTree.Name = "SpecsTree";
            this.SpecsTree.Size = new System.Drawing.Size(347, 56);
            this.SpecsTree.TabIndex = 1;
            this.SpecsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SpecsTree_NodeMouseClick);
            // 
            // SpecsContainter
            // 
            this.SpecsContainter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpecsContainter.Location = new System.Drawing.Point(0, 55);
            this.SpecsContainter.Name = "SpecsContainter";
            this.SpecsContainter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SpecsContainter.Panel1
            // 
            this.SpecsContainter.Panel1.Controls.Add(this.SpecsTree);
            // 
            // SpecsContainter.Panel2
            // 
            this.SpecsContainter.Panel2.Controls.Add(this.richTextBox1);
            this.SpecsContainter.Size = new System.Drawing.Size(347, 119);
            this.SpecsContainter.SplitterDistance = 56;
            this.SpecsContainter.TabIndex = 8;
            // 
            // PreviewButton
            // 
            this.PreviewButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.PreviewButton.Location = new System.Drawing.Point(150, 0);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(75, 33);
            this.PreviewButton.TabIndex = 2;
            this.PreviewButton.Text = "Preview";
            this.PreviewButton.UseVisualStyleBackColor = true;
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(347, 59);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // ItsWindowControl
            // 
            this.Controls.Add(this.SpecsContainter);
            this.Controls.Add(this.CommandPanel);
            this.Controls.Add(this.ModelPanel);
            this.Controls.Add(this.serverPanel);
            this.Name = "ItsWindowControl";
            this.Size = new System.Drawing.Size(347, 207);
            this.serverPanel.ResumeLayout(false);
            this.serverPanel.PerformLayout();
            this.ModelPanel.ResumeLayout(false);
            this.CommandPanel.ResumeLayout(false);
            this.SpecsContainter.Panel1.ResumeLayout(false);
            this.SpecsContainter.Panel2.ResumeLayout(false);
            this.SpecsContainter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel serverPanel;
        private System.Windows.Forms.TextBox ITSServerTextBox;
        private System.Windows.Forms.Button LoadObjectModelButton;
        private System.Windows.Forms.ComboBox ObjectModelListBox;
        private System.Windows.Forms.Label ItsServerLabel;
        private System.Windows.Forms.Panel ModelPanel;
        private System.Windows.Forms.Label ObjectModelLabel;
        private System.Windows.Forms.Panel CommandPanel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TreeView SpecsTree;
        private System.Windows.Forms.SplitContainer SpecsContainter;
        private System.Windows.Forms.Button PreviewButton;
        private System.Windows.Forms.RichTextBox richTextBox1;

    }
}
