using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.UI;
using Unity;

[assembly: ExtensionApplication(typeof(Jpp.Ironstone.DocumentManagement.DocumentManagementExtensionApplication))]

namespace Jpp.Ironstone.DocumentManagement
{
    class DocumentManagementExtensionApplication : IIronstoneExtensionApplication
    {
        public void Initialize()
        {
            CoreExtensionApplication._current.RegisterExtension(this);
        }

        public void Terminate()
        {
        }

        public void InjectContainer(IUnityContainer container)
        {
        }

        public void CreateUI()
        {
            RibbonControl rc = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab primaryTab = rc.FindTab(Jpp.Ironstone.Core.Constants.IRONSTONE_TAB_ID);

            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            source.Title = Properties.Resources.ExtensionApplication_UI_PanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;
            RibbonButton addRevision = UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_RevisionButton,
                Properties.Resources.Revise_Small, RibbonItemSize.Standard, String.Empty);

            RibbonButton finaliseRevisions = UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_FinaliseRevisionButton,
                Properties.Resources.Revise_Small, RibbonItemSize.Standard, String.Empty);

            RibbonSplitButton revisionSplitButton = new RibbonSplitButton();
            /*revisionSplitButton.Text = "RibbonSplit";*/
            revisionSplitButton.ShowText = true;
            revisionSplitButton.Items.Add(addRevision);
            revisionSplitButton.Items.Add(finaliseRevisions);
            //TODO: Enable once the backing code is in place
            revisionSplitButton.IsEnabled = false;

            column1.Items.Add(revisionSplitButton);
            column1.Items.Add(new RibbonRowBreak());
            column1.Items.Add(UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_ImportDrawing,
                Properties.Resources.DocumentType, RibbonItemSize.Standard, "DM_ImportDrawing"));

            //Build the UI hierarchy
            source.Items.Add(column1);

            Panel.Source = source;

            primaryTab.Panels.Add(Panel);
        }
    }
}
