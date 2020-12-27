namespace htaccess2Webconfig
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txt_configurationFile = new System.Windows.Forms.TextBox();
            this.btn_selectFile = new System.Windows.Forms.Button();
            this.btn_Import = new System.Windows.Forms.Button();
            this.txt_rewriteRules = new System.Windows.Forms.RichTextBox();
            this.lbl_rewriteRulesTextBox = new System.Windows.Forms.Label();
            this.tab_Control = new System.Windows.Forms.TabControl();
            this.tab_treeView = new System.Windows.Forms.TabPage();
            this._treeView = new System.Windows.Forms.TreeView();
            this.tab_xmlView = new System.Windows.Forms.TabPage();
            this.txt_xmlView = new System.Windows.Forms.RichTextBox();
            this.rightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl_summary = new System.Windows.Forms.Label();
            this.tab_Control.SuspendLayout();
            this.tab_treeView.SuspendLayout();
            this.tab_xmlView.SuspendLayout();
            this.rightClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_configurationFile
            // 
            this.txt_configurationFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_configurationFile.Location = new System.Drawing.Point(12, 12);
            this.txt_configurationFile.Name = "txt_configurationFile";
            this.txt_configurationFile.Size = new System.Drawing.Size(714, 22);
            this.txt_configurationFile.TabIndex = 0;
            this.txt_configurationFile.TextChanged += new System.EventHandler(this.Txt_configurationFile_TextChanged);
            // 
            // btn_selectFile
            // 
            this.btn_selectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_selectFile.Location = new System.Drawing.Point(730, 9);
            this.btn_selectFile.Name = "btn_selectFile";
            this.btn_selectFile.Size = new System.Drawing.Size(40, 29);
            this.btn_selectFile.TabIndex = 1;
            this.btn_selectFile.Text = "...";
            this.btn_selectFile.UseVisualStyleBackColor = true;
            this.btn_selectFile.Click += new System.EventHandler(this.Btn_SelectFile_Click);
            // 
            // btn_Import
            // 
            this.btn_Import.Location = new System.Drawing.Point(12, 40);
            this.btn_Import.Name = "btn_Import";
            this.btn_Import.Size = new System.Drawing.Size(80, 35);
            this.btn_Import.TabIndex = 2;
            this.btn_Import.Text = "Import";
            this.btn_Import.UseVisualStyleBackColor = true;
            this.btn_Import.Click += new System.EventHandler(this.Btn_Import_Click);
            // 
            // txt_rewriteRules
            // 
            this.txt_rewriteRules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_rewriteRules.Location = new System.Drawing.Point(12, 107);
            this.txt_rewriteRules.Name = "txt_rewriteRules";
            this.txt_rewriteRules.Size = new System.Drawing.Size(754, 203);
            this.txt_rewriteRules.TabIndex = 3;
            this.txt_rewriteRules.Text = "";
            this.txt_rewriteRules.TextChanged += new System.EventHandler(this.Txt_rewriteRules_TextChanged);
            // 
            // lbl_rewriteRulesTextBox
            // 
            this.lbl_rewriteRulesTextBox.AutoSize = true;
            this.lbl_rewriteRulesTextBox.Location = new System.Drawing.Point(12, 87);
            this.lbl_rewriteRulesTextBox.Name = "lbl_rewriteRulesTextBox";
            this.lbl_rewriteRulesTextBox.Size = new System.Drawing.Size(94, 17);
            this.lbl_rewriteRulesTextBox.TabIndex = 4;
            this.lbl_rewriteRulesTextBox.Text = "Rewrite rules:";
            // 
            // tab_Control
            // 
            this.tab_Control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tab_Control.Controls.Add(this.tab_treeView);
            this.tab_Control.Controls.Add(this.tab_xmlView);
            this.tab_Control.Location = new System.Drawing.Point(15, 328);
            this.tab_Control.Name = "tab_Control";
            this.tab_Control.SelectedIndex = 0;
            this.tab_Control.Size = new System.Drawing.Size(751, 294);
            this.tab_Control.TabIndex = 5;
            // 
            // tab_treeView
            // 
            this.tab_treeView.Controls.Add(this._treeView);
            this.tab_treeView.Location = new System.Drawing.Point(4, 25);
            this.tab_treeView.Name = "tab_treeView";
            this.tab_treeView.Padding = new System.Windows.Forms.Padding(3);
            this.tab_treeView.Size = new System.Drawing.Size(743, 265);
            this.tab_treeView.TabIndex = 0;
            this.tab_treeView.Text = "Tree View";
            this.tab_treeView.UseVisualStyleBackColor = true;
            // 
            // _treeView
            // 
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.Location = new System.Drawing.Point(3, 3);
            this._treeView.Name = "_treeView";
            this._treeView.Size = new System.Drawing.Size(737, 259);
            this._treeView.TabIndex = 8;
            // 
            // tab_xmlView
            // 
            this.tab_xmlView.Controls.Add(this.txt_xmlView);
            this.tab_xmlView.Location = new System.Drawing.Point(4, 25);
            this.tab_xmlView.Name = "tab_xmlView";
            this.tab_xmlView.Padding = new System.Windows.Forms.Padding(3);
            this.tab_xmlView.Size = new System.Drawing.Size(743, 265);
            this.tab_xmlView.TabIndex = 1;
            this.tab_xmlView.Text = "Xml View";
            this.tab_xmlView.UseVisualStyleBackColor = true;
            // 
            // txt_xmlView
            // 
            this.txt_xmlView.ContextMenuStrip = this.rightClick;
            this.txt_xmlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_xmlView.Location = new System.Drawing.Point(3, 3);
            this.txt_xmlView.Name = "txt_xmlView";
            this.txt_xmlView.Size = new System.Drawing.Size(737, 259);
            this.txt_xmlView.TabIndex = 0;
            this.txt_xmlView.Text = "";
            // 
            // rightClick
            // 
            this.rightClick.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.rightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Copy});
            this.rightClick.Name = "rightClick";
            this.rightClick.Size = new System.Drawing.Size(113, 28);
            this.rightClick.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.RightClick_ItemClicked);
            // 
            // Copy
            // 
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(112, 24);
            this.Copy.Text = "Copy";
            // 
            // lbl_summary
            // 
            this.lbl_summary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_summary.AutoSize = true;
            this.lbl_summary.Location = new System.Drawing.Point(19, 627);
            this.lbl_summary.Name = "lbl_summary";
            this.lbl_summary.Size = new System.Drawing.Size(0, 17);
            this.lbl_summary.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 653);
            this.Controls.Add(this.lbl_summary);
            this.Controls.Add(this.tab_Control);
            this.Controls.Add(this.lbl_rewriteRulesTextBox);
            this.Controls.Add(this.txt_rewriteRules);
            this.Controls.Add(this.btn_Import);
            this.Controls.Add(this.btn_selectFile);
            this.Controls.Add(this.txt_configurationFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "htaccess2Webconfig";
            this.tab_Control.ResumeLayout(false);
            this.tab_treeView.ResumeLayout(false);
            this.tab_xmlView.ResumeLayout(false);
            this.rightClick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_configurationFile;
        private System.Windows.Forms.Button btn_selectFile;
        private System.Windows.Forms.Button btn_Import;
        private System.Windows.Forms.RichTextBox txt_rewriteRules;
        private System.Windows.Forms.Label lbl_rewriteRulesTextBox;
        private System.Windows.Forms.TabControl tab_Control;
        private System.Windows.Forms.TabPage tab_treeView;
        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.TabPage tab_xmlView;
        private System.Windows.Forms.Label lbl_summary;
        private System.Windows.Forms.RichTextBox txt_xmlView;
        private System.Windows.Forms.ContextMenuStrip rightClick;
        private System.Windows.Forms.ToolStripMenuItem Copy;
    }
}

