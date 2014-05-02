using System;
using System.Windows.Forms;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using ProjectSettings = TimeTraveller.Tools.Sparx.ObjectModelGen.Properties.Settings;

namespace TimeTraveller.Tools.Sparx.ObjectModelGen
{
    public partial class RunGenerator : Form
    {
        public RunGenerator()
        {
            InitializeComponent();
        }

        public delegate void MyDelegate();

        public void OnProgress(object sender, ObjectModelGenEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (MyDelegate)delegate()
                {
                    this.progressBar1.Minimum = 1;
                    this.progressBar1.Value = e.Count;
                    this.progressBar1.Maximum = e.Total;
                    this.lblProgressMessage.Text = e.Message + "...";
                    this.Refresh();
                }
            );
        }

        private void RunGenerator_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            Cursor = Cursors.WaitCursor;
            ObjectModelGen objectModelGen = new ObjectModelGen();
            objectModelGen.Progress += new EventHandler<ObjectModelGenEventArgs>(OnProgress);
            objectModelGen.Generate(ProjectSettings.Default.RepositoryName,
                                            ProjectSettings.Default.OutputDirectory);
            Cursor = Cursors.Default;
            this.Close();

        }
    }
}
