using System;
using System.Windows;
using ZingitWizard.Resources;

namespace ZingitWizard
{
    public partial class ApplicationMainWindow : Window
    {
        public ApplicationMainWindow()
        {
            InitializeComponent();
        }

        void btnRunWizard_Click(object sender, RoutedEventArgs e)
        {
            CoffeeWizardDialog dlg = new CoffeeWizardDialog();
            if (dlg.ShowDialog() == true)
            {
                this.txtOrderResult.Text = String.Format(
                    Strings.ApplicationMainWindow_OrderComplete_Formatted,
                    dlg.Result.Price.ToString("c"));
            }
            else
            {
                this.txtOrderResult.Text = Strings.ApplicationMainWindow_OrderCancelled;
            }
        }
    }
}