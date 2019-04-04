using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jpp.Ironstone.DocumentManagement.ViewModels;

namespace Jpp.Ironstone.DocumentManagement.Views
{
    /// <summary>
    /// Interaction logic for DocumentTypeSelectorView.xaml
    /// </summary>
    public partial class DocumentTypeSelectorView : Window
    {
        public DocumentTypeSelectorView()
        {
            InitializeComponent();
            this.DataContext = new DocumentTypeSelectorViewModel(this);
        }
    }
}
