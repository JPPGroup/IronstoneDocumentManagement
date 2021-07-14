using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.DocumentManagement.ObjectModel;
using Jpp.Ironstone.DocumentManagement.ViewModels;
using Jpp.Ironstone.DocumentManagement.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jpp.Ironstone.DocumentManagement
{
    public static class ProjectCommands
    {
        [IronstoneCommand]
        [CommandMethod("DM_Project_AddSheet")]
        public static void AddSheet()
        {
            ILogger<CoreExtensionApplication> logger = DocumentManagementExtensionApplication._container.GetRequiredService<ILogger<CoreExtensionApplication>>();
            IConfiguration settings = DocumentManagementExtensionApplication._container.GetRequiredService<IConfiguration>();

            using (Transaction trans = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
            {
                LayoutSheetController lsc = new LayoutSheetController(logger, Application.DocumentManager.MdiActiveDocument.Database, settings);
                NewLayoutViewModel viewModel = new NewLayoutViewModel();
                bool? result = Application.ShowModalWindow(new NewLayout(viewModel));

                if (result.HasValue && result.Value)
                {
                    lsc.AddLayout(viewModel.LayoutName, viewModel.PaperSize);
                }

                trans.Commit();
            }

            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }
    }
}
