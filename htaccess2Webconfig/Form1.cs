using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using WebConfig;

namespace htaccess2Webconfig
{
    public partial class Form1 : Form
    {
        private string ConfigurationPath = null;
        private RewriteEntry _rewrite;
        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Web Server Configuration File (*.htaccess)|*.htaccess";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txt_configurationFile.Text = openFileDialog.FileName;
                }
            }
        }

        private void Txt_configurationFile_TextChanged(object sender, EventArgs e)
        {
            btn_Import.Enabled = (txt_configurationFile.Text.Length > 0);
            ConfigurationPath = txt_configurationFile.Text;
        }

        private void Btn_Import_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ConfigurationPath))
                {
                    txt_rewriteRules.Text = File.ReadAllText(ConfigurationPath);
                }
                else
                {
                    ShowError("Cannot import the specified configuration file.", "Error openning file");
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Translate()
        {
            _treeView.BeginUpdate();
            try
            {
                _treeView.Nodes.Clear();
                _rewrite = Translator.Translate(this.txt_rewriteRules.Text, false, 0);
                AddNodesToTreeView(_rewrite);
            }
            catch (Exception ex)
            {
                TreeNode treeNode = new TreeNode(ex.Message);
                _treeView.Nodes.Add(treeNode);
            }
            finally
            {
                _treeView.EndUpdate();
                base.Update();
            }
        }

        private void Txt_rewriteRules_TextChanged(object sender, EventArgs e)
        {
            Translate();
            txt_xmlView.Text = _rewrite.GetXml();
        }

        private void AddNodesToTreeView(RewriteEntry rewrite)
        {
            this._treeView.Nodes.Add((EntryTreeNode)rewrite.TreeNode);
            int num = rewrite.Rules.Count;
            int num2 = 0;
            foreach (RuleEntry ruleEntry in rewrite.Rules)
            {
                if (ruleEntry.HasErrorInfo || ruleEntry.HasErrorInConditions)
                {
                    num2++;
                }
            }
            if (rewrite.HasUnsupportedDirectives)
            {
                num2 += rewrite.UnsupportedDirectives.Count;
                num += rewrite.UnsupportedDirectives.Count;
            }
            lbl_summary.Text = string.Format(CultureInfo.InvariantCulture, "Summary: {0} of {1} rule(s) were converted successfully. {2} rule(s) were not converted.", (num - num2), num, num2);
        }

        private void RightClick_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
             case "Copy":
                    Clipboard.Clear();
                    Clipboard.SetText(txt_xmlView.Text);
                    break;
            }
        }
    }
}
