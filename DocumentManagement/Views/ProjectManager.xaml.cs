using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.DocumentManagement.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Jpp.Ironstone.DocumentManagement.Views
{
    /// <summary>
    /// Interaction logic for ProjectManager.xaml
    /// </summary>
    public partial class ProjectManager : HostedUserControl
    {
        public ProjectManager()
        {
            InitializeComponent();
        }

        public override void Show()
        {
            this.DataContext = DocumentManagementExtensionApplication._container.GetRequiredService<ProjectManagerViewModel>();
        }

        public override void Hide()
        {
        }
    }
}
