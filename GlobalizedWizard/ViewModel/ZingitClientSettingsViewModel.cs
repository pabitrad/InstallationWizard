using System.Windows.Forms;

using ZingitWizard.Model;

namespace ZingitWizard.ViewModel
{
    public class ZingitClientSettingsViewModel : ZingitWizardPageViewModelBase
    {
        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Client Settings"; }
        }

        public override bool CanMoveToNextPage()
        {
            if (string.IsNullOrEmpty(AppConfig.AccountNumber.Trim()) == true)
            {
                MessageBox.Show("Please enter Account Number.");
                return false;
            }

            if (string.IsNullOrEmpty(AppConfig.ClientName.Trim()) == true)
            {
                MessageBox.Show("Please enter Client Name.");
                return false;
            }

            return AppConfig.EncryptAndSaveData();
        }
    }
}
