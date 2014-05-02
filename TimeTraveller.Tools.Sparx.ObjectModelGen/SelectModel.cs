using System;
using System.Windows.Forms;

using ProjectSettings = TimeTraveller.Tools.Sparx.ObjectModelGen.Properties.Settings;
using System.IO;

namespace TimeTraveller.Tools.Sparx.ObjectModelGen
{
    public partial class SelectModel : Form
    {
        public SelectModel()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtRepositoryName.Text = ProjectSettings.Default.RepositoryName;
            this.txtOutputDirectory.Text = ProjectSettings.Default.OutputDirectory;
        }

        private void cmdGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                ProjectSettings.Default.RepositoryName = this.txtRepositoryName.Text;
                ProjectSettings.Default.OutputDirectory = this.txtOutputDirectory.Text;
                RunGenerator runGenerator = new RunGenerator();
                runGenerator.Show();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(String.Format("Fatal error detected.\r\nText of the error:{0}\r\nSource of the error:{1}\r\nStack trace:\r\n{2}",
                                              ex.Message, ex.Source, ex.StackTrace));
            }
        }

        private void cmdChooseModelFile_Click(object sender, EventArgs e)
        {
            string modelFileName = SelectModelFile(ProjectSettings.Default.RepositoryName);
            if (modelFileName != null)
            {
                ProjectSettings.Default.RepositoryName = modelFileName;
                this.txtRepositoryName.Text = ProjectSettings.Default.RepositoryName;
            }
        }
        
        private void cmdChooseOutputDirectory_Click(object sender, EventArgs e)
        {
            string outputDirectory = SelectOutputDirectory(ProjectSettings.Default.OutputDirectory);
            if (outputDirectory != null)
            {
                ProjectSettings.Default.OutputDirectory = outputDirectory;
                this.txtOutputDirectory.Text = ProjectSettings.Default.OutputDirectory;
            }
        }

        private string SelectModelFile(string initialModel)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Model files (*.eap)|*.eap|All files (*.*)|*.*";
            dialog.InitialDirectory = Path.GetDirectoryName(initialModel);
            dialog.Title = "Select a model file";
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.FileName : null;
        }

        private string SelectOutputDirectory(string initialPath)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = initialPath;
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.SelectedPath : null;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProjectSettings.Default.Save();
        }
   }
}