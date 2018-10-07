using System.Windows;
using System.Windows.Controls;

using ZingitWizard.ViewModel;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitFinalizeSettingsView.xaml
    /// </summary>
    public partial class ZingitFinalizeSettingsView : UserControl
    {
        public ZingitFinalizeSettingsView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(FinalizeSettings_Loaded);
        }

        void FinalizeSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ZingitFinalizeSettingsViewModel csViewModel = this.DataContext as ZingitFinalizeSettingsViewModel;
            if (csViewModel != null)
            {
                csViewModel.LoadParams();
            }
        }
    }
}
