/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml.Linq;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;

using MsVsShell = Microsoft.VisualStudio.Shell;
using VsConstants = Microsoft.VisualStudio.VSConstants;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using System.Net;

using Luminis.Its.Client;
using Luminis.Its.Workbench;
using System.Xml;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Luminis.Its.Client.Model;

namespace Luminis.Its.Workbench.DslPackage
{
    /// <summary>
    /// PersistedWindowControl is the control that will be hosted in the
    /// PersistedWindowPane. It consists of a list view that will display
    /// the tool windows that have already been created.
    /// 
    /// </summary>
    public partial class ItsWindowControl : UserControl
    {

        // Cached Selection Tracking service used to expose properties
        private ITrackSelection trackSelection = null;
        // Object holding the current selection properties
        private MsVsShell.SelectionContainer selectionContainer = new MsVsShell.SelectionContainer();

        /// <summary>
        /// This constructor is the default for a user control
        /// </summary>
        public ItsWindowControl()
        {
            // normal control initialization
            InitializeComponent();

        }
        WorkbenchPackage _package;
        internal WorkbenchPackage Package
        {
            get
            {
                return _package;
            }

            set
            {
                _package = value;
            }
        }


        /// <summary>
        /// Track selection service for the tool window.
        /// This should be set by the tool window pane as soon as the tool
        /// window is created.
        /// </summary>
        internal ITrackSelection TrackSelection
        {
            get { return (ITrackSelection)trackSelection; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("TrackSelection");
                trackSelection = value;
                // Inititalize with an empty selection
                // Failure to do this would result in our later calls to 
                // OnSelectChange to be ignored (unless focus is lost
                // and regained).
                selectionContainer.SelectableObjects = null;
                selectionContainer.SelectedObjects = null;
                trackSelection.OnSelectChange(selectionContainer);
                selectionContainer.SelectedObjectsChanged += new EventHandler(selectionContainer_SelectedObjectsChanged);
            }
        }

        /// <summary>
        /// Handle change to the current selection is done throught the properties window
        /// drop down list.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void selectionContainer_SelectedObjectsChanged(object sender, EventArgs e)
        {
        }

        private void LoadObjectModelButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> models = ItsClient.GetObjectModelList(ITSServerTextBox.Text);

                ObjectModelListBox.Items.AddRange(models.ToArray());

                if (models.Count > 1)
                {
                    ObjectModelListBox.SelectedIndex = 1;

                }
            }
            catch (Exception exepction)
            {
                PackageUtility.ShowError(Package, String.Format("Its error: {0}", exepction.Message));
            }

        }

        WorkbenchDomainModel DomainModel
        {
            get
            {
                return null;
            }
        }

        private void ObjectModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // change model
            ObjectModel om = ItsClient.GetObjectModel(ITSServerTextBox.Text, ObjectModelListBox.Text);

            Its2Dsl.loadObjectModelInDsl(Package.RootElement, om);

            // change casefile spec list
            List<string> caseFileSpecs = ItsClient.GetCaseFileSpecList(ITSServerTextBox.Text, ObjectModelListBox.Text);

            SpecsTree.Nodes.Clear();
            var caseFileNode = SpecsTree.Nodes.Add("Case File Specifications");
            foreach (string s in caseFileSpecs)
            {
                caseFileNode.Nodes.Add( "CaseFileSpec", s);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ObjectModel om = Dsl2Its.LoadObjectModel(Package.RootElement);
            ItsClient.PutObjectModel(ITSServerTextBox.Text, ObjectModelListBox.Text, om);

            foreach (var caseFileSpec in Package.RootElement.CaseFileModelSpecs)
            {
                CaseFileSpecification spec = Dsl2Its.LoadCaseFileSpecification( caseFileSpec);

                ItsClient.PutCaseFileSpecification(ITSServerTextBox.Text, ObjectModelListBox.Text, spec);
            }
        }


        private void PreviewButton_Click(object sender, EventArgs e)
        {
            ObjectModel om = Dsl2Its.LoadObjectModel(Package.RootElement);
            richTextBox1.Text = ObjectModelService.GetXML(om);
            foreach (CaseFileModelSpec casefileModel in Package.RootElement.CaseFileModelSpecs)
            {
                CaseFileSpecification casefileSpec = Dsl2Its.LoadCaseFileSpecification(casefileModel);
                richTextBox1.Text += CaseFileSpecificationService.GetXML(casefileSpec);
            }
        }

        private void SpecsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            richTextBox1.Text = e.Node.Text;
            if (e.Node.Name == "CaseFileSpec")
            {
                var caseFileSpec = ItsClient.GetCaseFileSpec(ITSServerTextBox.Text, ObjectModelListBox.Text, e.Node.Text);

                var caseFileModelSpec = Its2Dsl.LoadCaseFileSpecInDsl(Package.RootElement, caseFileSpec);

                var lane = PresentationViewsSubject.GetPresentation(caseFileModelSpec).Where(x => x is CaseFileSpecLane).Select(x=>x as CaseFileSpecLane).FirstOrDefault();
                foreach (CaseFileModelSpec c in Package.RootElement.CaseFileModelSpecs)
                {
                    foreach (CaseFileSpecLane l in PresentationViewsSubject.GetPresentation(c).Where(x => x is CaseFileSpecLane).Select(x => x as CaseFileSpecLane)) 
                    {
                        if (l != lane)
                        {
                            //l.Hide();
                        }
                    }
                }
                bool col = lane.CanExpandAndCollapse;
                lane.Show();
            }
        }

    }
}
