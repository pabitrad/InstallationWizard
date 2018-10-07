using ZingitWizard.Resources;

namespace ZingitWizard.ViewModel
{
    /// <summary>
    /// The first wizard page in the workflow.  
    /// This page has no functionality.
    /// </summary>
    public class WelcomePageViewModel : CoffeeWizardPageViewModelBase
    {
        public WelcomePageViewModel()
            : base(null)
        {
        }

        public override string DisplayName
        {
            get { return Strings.PageDisplayName_Welcome; }
        }

        internal override bool IsValid()
        {
            return true;
        }
    }
}