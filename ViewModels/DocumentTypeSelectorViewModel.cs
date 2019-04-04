using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.Windows;
using Jpp.Common;
using Window = System.Windows.Window;

namespace Jpp.Ironstone.DocumentManagement.ViewModels
{
    public class DocumentTypeSelectorViewModel
    {
        public ICommand OkCommand { get; set; }
        public List<string> DrawingTypes { get; set; }
        public string SelectedType { get; set; }

        //TODO: Remove window dependency
        public DocumentTypeSelectorViewModel(Window w)
        {
            DrawingTypes = new List<string>();
            DrawingTypes.Add("Civil Xref");
            OkCommand = new DelegateCommand(() =>
            {
                switch (SelectedType)
                {
                    case "Civil Xref":
                        int i = 1;
                        break;
                }

                w.Close();
            });
        }
    }
}
