using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
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
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
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
                RefreshModels();
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
                RefreshModels();
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
                RefreshModels();
            }
        }

        public ICommand PdfGenerate { get; }
        public ICommand TitleCorrect { get; }        


        public ObservableCollection<LayoutSheetViewModel> Sheets { get; set; }

        private ProjectController _projectController;
        private LayoutSheetController _layoutController;

        private ILogger<CoreExtensionApplication> _logger;

        public ProjectManagerViewModel(IServiceProvider container, IConfiguration settings, ILogger<CoreExtensionApplication> logger)
        {
            _logger = logger;

            Sheets = new ObservableCollection<LayoutSheetViewModel>();

            PdfGenerate = new DelegateCommand(() =>
            {               
              PlotToPdf();                
            });

            TitleCorrect = new DelegateCommand(() =>
            {
                UpdateTitles();
            });            

            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;

                using (doc.LockDocument())
                using (Transaction trans = doc.TransactionManager.StartTransaction())
                {                    
                    if (doc.IsNamedDrawing)
                    {
                        string workDirectoryName = Path.GetDirectoryName(doc.Database.Filename);
                        //TODO: Consider caching controllers for performance?
                        _projectController = new ProjectController(container, logger, settings, workDirectoryName);

                        BuildSheetModels();                                                
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Unknown exception: {e.Message}");
            }
        }

        private void RefreshModels()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                BuildSheetModels();
            }
        }

        private void BuildSheetModels()
        {
            Sheets.Clear();

            foreach (LayoutSheetController sheetController in _projectController.SheetControllers.Values)
            {
                foreach (LayoutSheet sheet in sheetController.Sheets.Values)
                {
                    LayoutSheetViewModel vm = new LayoutSheetViewModel()
                    {
                        DrawingNumber = sheet.TitleBlock?.DrawingNumber,
                        DrawingTitle = sheet.TitleBlock?.Title,
                        BackingSheet = sheet,
                        Selected = false,
                        ClientDiffers = sheet.TitleBlock?.Client != _projectController.Client,
                        ProjectDiffers = sheet.TitleBlock?.Project != _projectController.ProjectName,
                        ProjectNumberDiffers = sheet.TitleBlock?.ProjectNumber != _projectController.ProjectNumber
                    };

                    Sheets.Add(vm);
                }
            }
        }

        private void UpdateTitles()
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction trans = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
                {
                    foreach (LayoutSheetViewModel vm in Sheets.Where(s => s.Selected))
                    {
                        vm.BackingSheet.TitleBlock.ProjectNumber = _projectController.ProjectNumber;
                        vm.BackingSheet.TitleBlock.Project = _projectController.ProjectName;
                        vm.BackingSheet.TitleBlock.Client = _projectController.Client;

                        vm.ClientDiffers = vm.BackingSheet.TitleBlock?.Client != _projectController.Client;
                        vm.ProjectDiffers = vm.BackingSheet.TitleBlock?.Project != _projectController.ProjectName;
                        vm.ProjectNumberDiffers = vm.BackingSheet.TitleBlock?.ProjectNumber != _projectController.ProjectNumber;
                    }                   

                    trans.Commit();
                }
            }            
        }

        private void PlotToPdf()
        {
            int bpValue = Convert.ToInt32(Application.GetSystemVariable("BACKGROUNDPLOT"));

            try
            {
                IEnumerable<LayoutSheetViewModel> sheetsToPlot = Sheets.Where(s => s.Selected);
                                
                Application.SetSystemVariable("BACKGROUNDPLOT", 0);
                _logger.LogTrace($"BACKGROUNDPLOT set to {Convert.ToInt32(Application.GetSystemVariable("BACKGROUNDPLOT"))}");

                using (PlotEngine pe = PlotFactory.CreatePublishEngine())
                {
                    using (PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true))
                    {

                        ppd.OnBeginPlot();
                        ppd.IsVisible = false;
                        pe.BeginPlot(ppd, null);

                        List<string> expectedFiles = new List<string>();

                        using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                        {

                            foreach (LayoutSheetViewModel sheet in sheetsToPlot)
                            {
                                string fileName = Path.Combine(_projectController.PdfDirectory, sheet.BackingSheet.GetPDFName());
                                _logger.LogTrace($"Plotting {fileName}");

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

                        Process.Start(_projectController.PdfDirectory);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown plot error");
                string messageBoxText = "An unkown plot error has occurred, please see logs for details";
                string caption = "Plot Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
            finally
            {
                Application.SetSystemVariable("BACKGROUNDPLOT", bpValue);
                _logger.LogTrace($"BACKGROUNDPLOT set to {Convert.ToInt32(Application.GetSystemVariable("BACKGROUNDPLOT"))}");
            }
        }
    }
}
