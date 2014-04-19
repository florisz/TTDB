using System;
using System.ComponentModel.Design;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Globalization;

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using MsVsShell = Microsoft.VisualStudio.Shell;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;



namespace Luminis.Its.Workbench.DslPackage
{
    [MsVsShell.ProvideToolWindow(typeof(ItsWindowPane), Style = MsVsShell.VsDockStyle.Tabbed, Window = "3ae79031-e1bc-11d0-8f78-00a0c9110057")]
    [MsVsShell.DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\9.0Exp")]
    [MsVsShell.PackageRegistration(UseManagedResourcesOnly = true)]

    [MsVsShell.ProvideMenuResource(1000, 1)]
    partial class WorkbenchPackage
    {
        // Cache the Menu Command Service since we will use it multiple times
        private MsVsShell.OleMenuCommandService menuService;

        protected override void Initialize()
        {
            base.Initialize();

            // Each command is uniquely identified by a Guid/integer pair.
            CommandID id = new CommandID(new Guid(Constants.WorkbenchCommandSetId), PkgCmdId.cmdidItsWindow);
            // Add the handler for the persisted window with selection tracking
            DefineCommandHandler(new EventHandler(this.ShowItsWindow), id);

            ShowItsWindow(this, null);
        }

        /// <summary>
        /// Event handler for our menu item.
        /// This results in the tool window being shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ShowItsWindow(object sender, EventArgs arguments)
        {
            // Get the 1 (index 0) and only instance of our tool window (if it does not already exist it will get created)
            MsVsShell.ToolWindowPane pane = this.FindToolWindow(typeof(ItsWindowPane), 0, true);
            if (pane == null)
            {
                throw new COMException("this.GetResourceString(\"@101\")");
            }
            IVsWindowFrame frame = pane.Frame as IVsWindowFrame;
            if (frame == null)
            {
                throw new COMException("this.GetResourceString(\"@102\")");
            }
            // Bring the tool window to the front and give it focus
            ErrorHandler.ThrowOnFailure(frame.Show());
        }


        /// <summary>
        /// This method loads a localized string based on the specified resource.
        /// </summary>
        /// <param name="resourceName">Resource to load</param>
        /// <returns>String loaded for the specified resource</returns>
        internal string GetResourceString(string resourceName)
        {
            string resourceValue;
            IVsResourceManager resourceManager = (IVsResourceManager)GetService(typeof(SVsResourceManager));
            if (resourceManager == null)
            {
                throw new InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method");
            }
            Guid packageGuid = this.GetType().GUID;
            int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            return resourceValue;
        }

        /// <summary>
        /// Define a command handler.
        /// When the user press the button corresponding to the CommandID
        /// the EventHandler will be called.
        /// </summary>
        /// <param name="id">The CommandID (Guid/ID pair) as defined in the .vsct file</param>
        /// <param name="handler">Method that should be called to implement the command</param>
        /// <returns>The menu command. This can be used to set parameter such as the default visibility once the package is loaded</returns>
        internal MsVsShell.OleMenuCommand DefineCommandHandler(EventHandler handler, CommandID id)
        {
            // if the package is zombied, we don't want to add commands
            if (this.Zombied)
                return null;

            // Make sure we have the service
            if (menuService == null)
            {
                // Get the OleCommandService object provided by the MPF; this object is the one
                // responsible for handling the collection of commands implemented by the package.
                menuService = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            }
            MsVsShell.OleMenuCommand command = null;
            if (null != menuService)
            {
                // Add the command handler
                command = new MsVsShell.OleMenuCommand(handler, id);
                menuService.AddCommand(command);
            }
            
            return command;
        }

        public ModelRoot RootElement
        {
            get
            {
                ToolWindow pane = GetToolWindow(typeof(WorkbenchExplorerToolWindow), true);

                ToolWindow v = GetToolWindow(typeof(DiagramView), true);

                
                //pane.DocData = new WorkbenchDocData(this, new Guid(Constants.WorkbenchEditorFactoryId));
                //data.InitNew(0);
                //data.InitFromITS();
                

                WorkbenchDocData data = pane.DocData as WorkbenchDocData;

                ModelRoot root = data.RootElement as ModelRoot;

                //Workbench.ClassDiagram dg;
                //dg.AutoLayoutShapeElements(dg.GetChildElements(root));
                return root;
            }
        }

    }
}
