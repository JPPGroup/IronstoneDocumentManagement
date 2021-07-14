using System;
using Autodesk.AutoCAD.ApplicationServices;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.DocumentManagement.ObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Jpp.Ironstone.DocumentManagement.ViewModels
{
    public class ProjectManagerViewModel
    {
        public string ProjectName {
            get
            {
                return _projectController.ProjectName;
            }
            set
            {
                _projectController.ProjectName = value;
            }
        }

        public string ProjectNumber
        {
            get
            {
                return _projectController.ProjectNumber;
            }
            set
            {
                _projectController.ProjectNumber = value;
            }
        }

        public string Client
        {
            get
            {
                return _projectController.Client;
            }
            set
            {
                _projectController.Client = value;
            }
        }

        private ProjectController _projectController;
        private LayoutSheetController _layoutController;

        public ProjectManagerViewModel(IServiceProvider container, IConfiguration settings, ILogger<CoreExtensionApplication> logger)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc.IsNamedDrawing)
            {
                string workDirectoryName = Path.GetDirectoryName(doc.Database.Filename);
                //TODO: Consider caching controllers for performance?
                _projectController = new ProjectController(container, logger, settings, workDirectoryName);
            }
        }
    }
}
