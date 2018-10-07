using System;
using System.Windows;
using CoffeeLibrary;
using ZingitWizard.ViewModel;

namespace ZingitWizard
{
    public partial class CoffeeWizardDialog : Window
    {
        readonly CoffeeWizardViewModel _coffeeWizardViewModel;

        public CoffeeWizardDialog()
        {
            InitializeComponent();
            
            _coffeeWizardViewModel = new CoffeeWizardViewModel();
            _coffeeWizardViewModel.RequestClose += this.OnViewModelRequestClose;
            base.DataContext = _coffeeWizardViewModel;            
        }

        /// <summary>
        /// Returns the cup of coffee ordered by the user, 
        /// or null if the user cancelled the order.
        /// </summary>
        public CupOfCoffee Result
        {
           get { return _coffeeWizardViewModel.CupOfCoffee; }
        }

        void OnViewModelRequestClose(object sender, EventArgs e)
        {
            base.DialogResult = this.Result != null;
        }
    }
}