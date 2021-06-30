using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.DocumentManagement.Objectmodel.DrawingTypes;
using Jpp.Ironstone.DocumentManagement.Views;
using Microsoft.Win32;

namespace Jpp.Ironstone.DocumentManagement
{
    public class ImportDrawingCommand
    {
        [CommandMethod("DM_ImportDrawing", CommandFlags.Session)]
        public static void ImportDrawing()
        {
            DocumentTypeSelectorView selectorView = new DocumentTypeSelectorView();
            selectorView.ShowDialog();

            CivilXrefDrawingType xrefDrawingType = new CivilXrefDrawingType();
            Document activeDocument =
                Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            xrefDrawingType.SetDrawing(activeDocument);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Drawing File|*.dwg";
            sfd.Title = "Save drawing as";
            sfd.ShowDialog();
            bool Aborted;
            if (sfd.FileName != "")
            {
                Aborted = false;
            }
            else
            {
                Aborted = true;
            }

            if (!Aborted)
            {
                activeDocument.Database.SaveAs(sfd.FileName, Autodesk.AutoCAD.DatabaseServices.DwgVersion.Current);

                //Close the original file as its no longer needed
                activeDocument.CloseAndDiscard();
            }
        }

    }           
}
