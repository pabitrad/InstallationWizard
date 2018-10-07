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
using System.Windows.Shapes;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitCampaignSetupInstructions.xaml
    /// </summary>
    public partial class ZingitCampaignSetupInstructions : Window
    {
        public ZingitCampaignSetupInstructions()
        {
            InitializeComponent();

            this.Height = SystemParameters.PrimaryScreenHeight * 0.9;
        }
    }
}
