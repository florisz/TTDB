using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ProjectSettings = TimeTraveller.Tools.Sparx.ObjectModelGen.Properties.Settings;

namespace TimeTraveller.Tools.Sparx.ObjectModelGen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Command line parsing
            Arguments CommandLine = new Arguments(args);

            // application is started in windows mode
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // application is started in batch mode
            try
            {
                if (CommandLine.Count == 0 || CommandLine["win"] != null)
                {
                    Application.Run(new SelectModel());
                }
                else
                {
                    bool paramsOk = true;

                    if (CommandLine["o"] != null)
                        ProjectSettings.Default.OutputDirectory = CommandLine["o"];
                    else
                        paramsOk = false;

                    if (CommandLine["r"] != null)
                        ProjectSettings.Default.RepositoryName = CommandLine["r"];
                    else
                        paramsOk = false;

                    if (!paramsOk)
                    {
                        MessageBox.Show("Error in paramaters; usage: EAObjectModelGenerator /r:repositoryname /o:output-directory");
                    }
                    else
                    {
                        Application.Run(new RunGenerator());
                        Application.Exit(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Fatal error detected.\r\nText of the error:{0}\r\nSource of the error:{1}\r\nStack trace:\r\n{2}",
                                              ex.Message, ex.Source, ex.StackTrace));
            }
        }
    }
}