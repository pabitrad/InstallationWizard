using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZingitWizard.ViewModel
{
    class ZingitCampaignSetupViewModel : ZingitWizardPageViewModelBase
    {
        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return true;
        }

        public override string DisplayName
        {
            get { return "Campaign Setup"; }
        }
    }
}
