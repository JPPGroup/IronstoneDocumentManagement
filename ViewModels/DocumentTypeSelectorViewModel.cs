using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.AutoCAD;

namespace Jpp.Ironstone.DocumentManagement.ViewModels
{
    public class DocumentTypeSelectorViewModel
    {
        public ICommand OkCommand { get; set; }
        public List<string> DrawingTypes { get; set; }
        public string SelectedType { get; set; }

        public DocumentTypeSelectorViewModel()
        {
            DrawingTypes = new List<string>();
            DrawingTypes.Add("Civil Xref");
            /*OkCommand = new DelegateCommand(() =>
            {
                switch (SelectedType)
                {
                    case "Civil Xref":
                        int i = 1;
                        break;
                }
            });*/
        }

    }
}
