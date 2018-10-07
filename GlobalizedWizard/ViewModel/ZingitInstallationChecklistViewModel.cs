using System;
using System.Windows.Forms;

namespace ZingitWizard.ViewModel
{
    class ZingitInstallationChecklistViewModel : ZingitWizardPageViewModelBase
    {
        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Installation Checklist"; }
        }

        public bool GoAndMapping
        {
            get; set;
        }

        public bool CampaignSetup
        {
            get; set;
        }

        public bool SMSContentCustomization
        {
            get; set;
        }

        public bool RemoveOmitBouncebackMessage
        {
            get; set;
        }

        public bool ExecutionModeSetToPRODUCTION
        {
            get; set;
        }

        public override bool CanMoveToNextPage()
        {
            if (string.IsNullOrEmpty(AppConfig.SMSContent) == true)
            {
                MessageBox.Show("Please enter SMS Content.");
                return false;
            }
 
            CheckTasks();

            return true;
        }

        private void CheckTasks()
        {
            if (GoAndMapping & CampaignSetup & SMSContentCustomization & RemoveOmitBouncebackMessage & ExecutionModeSetToPRODUCTION)
            {
                return;
            }
            MessageBox.Show("From check list looks like you forgot to perform some tasks.");
        }
    }
}
