using System.Windows;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for DropAndGoMappingInstruction.xaml
    /// </summary>
    public partial class ZingitDropAndGoMappingInstruction : Window
    {
        public ZingitDropAndGoMappingInstruction()
        {
            InitializeComponent();

            this.Height = SystemParameters.PrimaryScreenHeight * 0.9;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.9;
        }
    }
}
