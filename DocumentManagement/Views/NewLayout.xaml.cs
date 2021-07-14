using System.Windows;
using Jpp.Ironstone.DocumentManagement.ViewModels;

namespace Jpp.Ironstone.DocumentManagement.Views
{
    /// <summary>
    /// Interaction logic for NewLayout.xaml
    /// </summary>
    public partial class NewLayout : Window
    {
        public NewLayout(NewLayoutViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }

        private void okButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = true;

        private void cancelButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;
    }
}
