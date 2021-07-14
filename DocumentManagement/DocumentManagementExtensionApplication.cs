using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.DocumentManagement.Properties;
using Jpp.Ironstone.DocumentManagement.ViewModels;
using Jpp.Ironstone.DocumentManagement.Views;
using Microsoft.Extensions.DependencyInjection;

[assembly: ExtensionApplication(typeof(Jpp.Ironstone.DocumentManagement.DocumentManagementExtensionApplication))]

namespace Jpp.Ironstone.DocumentManagement
{
    class DocumentManagementExtensionApplication : IIronstoneExtensionApplication
    {
        internal static IServiceProvider _container;
        
        public void Initialize()
        {
            CoreExtensionApplication._current.RegisterExtension(this);
        }

        public void Terminate()
        {
        }

        public void RegisterServices(IServiceCollection container)
        {
            container.AddTransient<ProjectManager>();
            container.AddTransient<ProjectManagerViewModel>();

            //TODO: Consider caching controllers for performance?
        }

        public void InjectContainer(IServiceProvider provider)
        {
            _container = provider;
        }

        public void CreateUI()
        {
            RibbonControl rc = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab primaryTab = rc.FindTab(Jpp.Ironstone.Core.Constants.IronstoneGeneralTabId);

            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            source.Title = Properties.Resources.ExtensionApplication_UI_PanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;

            RibbonButton addSheet = UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_AddSheetButton,
                Properties.Resources.AddNewSheet_Small, RibbonItemSize.Standard, UIHelper.GetCommandGlobalName(typeof(ProjectCommands), nameof(ProjectCommands.AddSheet)));

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

            column1.Items.Add(addSheet);
            column1.Items.Add(revisionSplitButton);
            column1.Items.Add(new RibbonRowBreak());
            column1.Items.Add(UIHelper.CreateButton(Properties.Resources.ExtensionApplication_UI_ImportDrawing,
                Properties.Resources.DocumentType, RibbonItemSize.Standard, "DM_ImportDrawing"));
            
            RibbonToggleButton projectButton = UIHelper.CreateWindowToggle(Resources.ExtensionApplication_UI_ProjectManagementToggleButton, Resources.File, RibbonItemSize.Large, _container.GetRequiredService<ProjectManager>(), "e11bf768-89ed-42b7-a68f-166d1b4b60e8");

            //Build the UI hierarchy
            source.Items.Add(projectButton);
            source.Items.Add(column1);
            

            Panel.Source = source;

            primaryTab.Panels.Add(Panel);
        }
    }
}
