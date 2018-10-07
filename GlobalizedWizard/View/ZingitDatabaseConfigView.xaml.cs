using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZingitWizard.Model;
using ZingitWizard.ViewModel;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitDatabaseConfigView.xaml
    /// </summary>
    public partial class ZingitDatabaseConfigView : UserControl
    {
        public ZingitDatabaseConfigView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DatabaseConfig_Loaded);
        }

        void DatabaseConfig_Loaded(object sender, RoutedEventArgs e)
        {
            AppConfigModel config = AppConfigModel.Instance;
            config.SetDbConfigData();
        }
    }
}
