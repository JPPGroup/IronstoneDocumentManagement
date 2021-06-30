using System;
using Autodesk.AutoCAD.ApplicationServices;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.DocumentManagement.ObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Jpp.Ironstone.DocumentManagement.ViewModels
{
    class ProjectManagerViewModel
    {
        public string Project {
            get
            {
                return _projectController.ProjectName;
            }
            set
            {
                _projectController.ProjectName = value;
            }
        }

        private ProjectController _projectController;
        private LayoutSheetController _layoutController;

        public ProjectManagerViewModel(IServiceProvider container, IConfiguration settings, ILogger<CoreExtensionApplication> logger)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            string workDirectoryName = Path.GetDirectoryName(doc.Database.Filename);

            _projectController = new ProjectController(container, workDirectoryName);
            _layoutController = new LayoutSheetController(logger, doc, settings);
        }
    }
}
