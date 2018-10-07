using System.Windows;
using System.Windows.Controls;

namespace ZingitWizard.View
{
    /// <summary>
    /// Interaction logic for DropAndGoMapping.xaml
    /// </summary>
    public partial class ZingitDropAndGoMappingView : UserControl
    {
        public ZingitDropAndGoMappingView()
        {
            InitializeComponent();
        }

        public void Button_Click_ViewInstruction(object sender, RoutedEventArgs e)
        {
            ZingitDropAndGoMappingInstruction dropAndGoInstructionWindow = new ZingitDropAndGoMappingInstruction();

            dropAndGoInstructionWindow.Show();
        }
    }
}
