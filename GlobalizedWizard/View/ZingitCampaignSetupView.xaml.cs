using System.Windows;
using System.Windows.Controls;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for ZingitCampaignSetupView.xaml
    /// </summary>
    public partial class ZingitCampaignSetupView : UserControl
    {
        public ZingitCampaignSetupView()
        {
            InitializeComponent();
        }

        public void Button_Click_ViewInstruction(object sender, RoutedEventArgs e)
        {
            ZingitCampaignSetupInstructions campaignSetupInstructionWindow = new ZingitCampaignSetupInstructions();

            campaignSetupInstructionWindow.Show();
        }
    }
}