using System.Windows;
using System.Windows.Controls;
using ZingitWizard.ViewModel;
using ZingitWizard.Model;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitClientSettingsView.xaml
    /// </summary>
    public partial class ZingitClientSettingsView : UserControl
    {
        public ZingitClientSettingsView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ClientSettings_Loaded);
        }

        void ClientSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ZingitClientSettingsViewModel csViewModel = this.DataContext as ZingitClientSettingsViewModel;
            string installDir = App.Current.Properties["InstallDir"] as string;

            if (string.IsNullOrEmpty(installDir) == false)
            {
                AppConfigModel config = csViewModel.AppConfig;
                if (config.DecryptData(installDir) == false)
                {
                    return;
                }

                config.SetClientSettings();
            }
        }
    }
}
