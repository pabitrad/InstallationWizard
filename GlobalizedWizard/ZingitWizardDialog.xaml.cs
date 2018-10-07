using System;
using System.Windows;
using ZingitWizard.ViewModel;

namespace ZingitWizard
{
    public partial class ZingitWizardDialog : Window
    {
        readonly ZingitWizardViewModel _zingitWizardViewModel;

        public ZingitWizardDialog()
        {
            InitializeComponent();

            _zingitWizardViewModel = new ZingitWizardViewModel();
            _zingitWizardViewModel.RequestClose += this.OnViewModelRequestClose;
            base.DataContext = _zingitWizardViewModel;            
        }

        void OnViewModelRequestClose(object sender, EventArgs e)
        {
            //base.DialogResult = this.Result != null;
            base.DialogResult = false;
        }
    }
}