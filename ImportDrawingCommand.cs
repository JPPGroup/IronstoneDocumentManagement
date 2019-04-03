using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.DocumentManagement.Views;

namespace Jpp.Ironstone.DocumentManagement
{
    public class ImportDrawingCommand
    {
        [CommandMethod("DM_ImportDrawing")]
        public static void ImportDrawing()
        {
            DocumentTypeSelectorView selectorView = new DocumentTypeSelectorView();
            selectorView.ShowDialog();
        }
    }
}
