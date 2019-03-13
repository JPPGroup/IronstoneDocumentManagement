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
            RibbonButton revision = UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_RevisionButton,
                Properties.Resources.Revise_Small, RibbonItemSize.Standard, String.Empty);
            revision.IsEnabled = false;
            column1.Items.Add(revision);
            column1.Items.Add(new RibbonRowBreak());;

            //Build the UI hierarchy
            source.Items.Add(column1);

            Panel.Source = source;

            primaryTab.Panels.Add(Panel);
        }
    }
}
