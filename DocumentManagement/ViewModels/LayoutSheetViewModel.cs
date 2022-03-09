using CommunityToolkit.Mvvm.ComponentModel;
using Jpp.Ironstone.DocumentManagement.ObjectModel;

namespace Jpp.Ironstone.DocumentManagement.ViewModels
{
    public partial class LayoutSheetViewModel : ObservableObject
    {
        public string DrawingNumber
        {
            get
            {
                return _drawingNumber;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _drawingNumber = value;
                    SetProperty(ref _drawingNumber, value);
                }
            }
        }
        private string _drawingNumber = "?";

        public string DrawingTitle
        {
            get
            {
                return _drawingTitle;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _drawingTitle, value);
                }
            }
        }

        private string _drawingTitle = "Unknown";

        public string DocName { get; set; }

        public bool Selected { get; set; }

        [ObservableProperty]
        private bool _projectDiffers;

        [ObservableProperty]
        private bool _clientDiffers;

        [ObservableProperty]
        private bool _projectNumberDiffers;


        internal LayoutSheet BackingSheet { get; set; }
    }
}
