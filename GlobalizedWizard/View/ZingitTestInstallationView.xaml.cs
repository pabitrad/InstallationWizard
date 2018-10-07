using System.Windows;
using System.Windows.Controls;

using ZingitWizard.ViewModel;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitTestInstallationView.xaml
    /// </summary>
    public partial class ZingitTestInstallationView : UserControl
    {
        public ZingitTestInstallationView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(TestInstallation_Loaded);
        }

        void TestInstallation_Loaded(object sender, RoutedEventArgs e)
        {
            ZingitTestInstallationViewModel csViewModel = this.DataContext as ZingitTestInstallationViewModel;
            if (csViewModel != null)
            {
                csViewModel.LoadTaskParams();
            }
        }
    }
}
