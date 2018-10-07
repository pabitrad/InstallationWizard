using System.ComponentModel;
using ZingitWizard.Model;

namespace ZingitWizard.ViewModel
{
    /// <summary>
    /// Abstract base class for all pages shown in the wizard.
    /// </summary>
    public abstract class ZingitWizardPageViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        bool _isCurrentPage;
        bool _isPropertyChanged = false;

        public AppConfigModel AppConfig { get; set; }

        #endregion // Fields

        #region Constructor

        protected ZingitWizardPageViewModelBase()
        {
            AppConfig = AppConfigModel.Instance;
        }

        #endregion // Constructor


        #region Properties

        public abstract string DisplayName { get; }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set 
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                this.OnPropertyChanged("IsCurrentPage");
            }
        }
 
        protected bool IsPropertyChanged
        {
            get { return _isPropertyChanged; }

            set
            {
                _isPropertyChanged = value;
            }
        }

        public virtual bool CanMoveToNextPage()
        {
            return true;
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        #endregion // Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            IsPropertyChanged = true;
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}