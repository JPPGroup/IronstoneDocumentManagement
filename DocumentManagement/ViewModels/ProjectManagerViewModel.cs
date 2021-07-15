using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.Civil.DatabaseServices;
using Jpp.Common;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.DocumentManagement.ObjectModel;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public ICommand PdfGenerate { get; }
        

        public List<LayoutSheetViewModel> Sheets { get; set; }

        private ProjectController _projectController;
        private LayoutSheetController _layoutController;

        private ILogger<CoreExtensionApplication> _logger;

        public ProjectManagerViewModel(IServiceProvider container, IConfiguration settings, ILogger<CoreExtensionApplication> logger)
        {
            _logger = logger;

            Sheets = new List<LayoutSheetViewModel>();

            PdfGenerate = new DelegateCommand(() =>
            {
                PlotToPdf();
            });

            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;

                using (doc.LockDocument())
                {
                    if (doc.IsNamedDrawing)
                    {
                        string workDirectoryName = Path.GetDirectoryName(doc.Database.Filename);
                        //TODO: Consider caching controllers for performance?
                        _projectController = new ProjectController(container, logger, settings, workDirectoryName);

                        foreach (LayoutSheetController sheetController in _projectController.SheetControllers.Values)
                        {
                            foreach (LayoutSheet sheet in sheetController.Sheets.Values)
                            {
                                Sheets.Add(new LayoutSheetViewModel()
                                {
                                    DrawingNumber = sheet.TitleBlock.DrawingNumber,
                                    DrawingTitle = sheet.TitleBlock.Title,
                                    BackingSheet = sheet,
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Unknown exception: {e.Message}");
            }
        }

        private void PlotToPdf()
        {
            IEnumerable<LayoutSheetViewModel> sheetsToPlot = Sheets.Where(s => s.Selected);

            using (PlotEngine pe = PlotFactory.CreatePublishEngine())
            {
                using (PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true))
                {
                    int bpValue = Convert.ToInt32(Application.GetSystemVariable("BACKGROUNDPLOT"));
                    Application.SetSystemVariable("BACKGROUNDPLOT", 0);
                    _logger.LogTrace($"BACKGROUNDPLOT set to {Convert.ToInt32(Application.GetSystemVariable("BACKGROUNDPLOT"))}");

                    ppd.OnBeginPlot();
                    ppd.IsVisible = false;
                    pe.BeginPlot(ppd, null);

                    List<string> expectedFiles = new List<string>();

                    using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                    {

                        foreach (LayoutSheetViewModel sheet in sheetsToPlot)
                        {
                            string fileName = Path.Combine(_projectController.PdfDirectory, sheet.BackingSheet.GetPDFName());

                            try
                            {
                                sheet.BackingSheet.Plot(fileName, pe, ppd);
                                expectedFiles.Add(fileName);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"{fileName} failed to plot.");
                            }
                        }
                    }

                    ppd.PlotProgressPos = 100;
                    ppd.OnEndPlot();
                    pe.EndPlot(null);

                    Application.SetSystemVariable("BACKGROUNDPLOT", bpValue);
                }
            }
        }
    }

    public class LayoutSheetViewModel
    {
        public string DrawingNumber { get; set; }
        public string DrawingTitle { get; set; }
        public bool Selected { get; set; }


        internal LayoutSheet BackingSheet { get; set; }
    }
}
