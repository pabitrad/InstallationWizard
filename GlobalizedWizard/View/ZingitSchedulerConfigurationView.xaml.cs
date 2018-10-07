using System.Windows;
using System.Windows.Controls;

using ZingitWizard.ViewModel;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitSchedulerConfigurationView.xaml
    /// </summary>
    public partial class ZingitSchedulerConfigurationView : UserControl
    {
        public ZingitSchedulerConfigurationView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(TaskSchedulerCoofig_Loaded);
        }

        void TaskSchedulerCoofig_Loaded(object sender, RoutedEventArgs e)
        {
            ZingitSchedulerConfigurationViewModel csViewModel = this.DataContext as ZingitSchedulerConfigurationViewModel;
            if (csViewModel != null)
            {
                csViewModel.LoadTaskParams();
            }
        }
    }
}
