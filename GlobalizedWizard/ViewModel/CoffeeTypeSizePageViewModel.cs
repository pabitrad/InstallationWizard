using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CoffeeLibrary;
using ZingitWizard.Resources;

namespace ZingitWizard.ViewModel
{
    /// <summary>
    /// The wizard page that allows the user to select the type of coffee bean
    /// and drink size that they want to purchase.
    /// </summary>
    public class CoffeeTypeSizePageViewModel : CoffeeWizardPageViewModelBase
    {
        #region Fields

        ReadOnlyCollection<OptionViewModel<BeanType>> _availableBeanTypes;
        ReadOnlyCollection<OptionViewModel<DrinkSize>> _availableDrinkSizes;

        #endregion // Fields

        #region Constructor

        public CoffeeTypeSizePageViewModel(CupOfCoffee cupOfCoffee)
            : base(cupOfCoffee)
        {
        }

        #endregion // Constructor

        #region Properties

        #region AvailableBeanTypes

        /// <summary>
        /// Returns a read-only collection of all bean types that the user can select.
        /// </summary>
        public ReadOnlyCollection<OptionViewModel<BeanType>> AvailableBeanTypes
        {
            get
            {
                if (_availableBeanTypes == null)
                    this.CreateAvailableBeanTypes();

                return _availableBeanTypes;
            }
        }

        void CreateAvailableBeanTypes()
        {
            List<OptionViewModel<BeanType>> list = new List<OptionViewModel<BeanType>>();
            list.Add(new OptionViewModel<BeanType>(Strings.BeanType_Organic, BeanType.Organic));
            list.Add(new OptionViewModel<BeanType>(Strings.BeanType_Breakfast, BeanType.Breakfast));
            list.Add(new OptionViewModel<BeanType>(Strings.BeanType_DarkRoast, BeanType.DarkRoast));
            list.Add(new OptionViewModel<BeanType>(Strings.BeanType_House, BeanType.House));
            list.Add(new OptionViewModel<BeanType>(Strings.BeanType_LightRoast, BeanType.LightRoast));

            foreach (OptionViewModel<BeanType> option in list)
            {
                // Select the default value.
                if (option.GetValue() == base.CupOfCoffee.BeanType)
                    option.IsSelected = true;

                option.PropertyChanged += this.OnBeanTypeOptionPropertyChanged;
            }

            list.Sort();

            _availableBeanTypes = new ReadOnlyCollection<OptionViewModel<BeanType>>(list);
        }

        void OnBeanTypeOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OptionViewModel<BeanType> option = sender as OptionViewModel<BeanType>;
            if (option.IsSelected)
                this.CupOfCoffee.BeanType = option.GetValue();
        }

        #endregion // AvailableBeanTypes

        #region AvailableDrinkSizes

        /// <summary>
        /// Returns a read-only collection of all drink sizes that the user can select.
        /// </summary>
        public ReadOnlyCollection<OptionViewModel<DrinkSize>> AvailableDrinkSizes
        {
            get
            {
                if (_availableDrinkSizes == null)
                    this.CreateAvailableDrinkSizes();

                return _availableDrinkSizes;
            }
        }

        void CreateAvailableDrinkSizes()
        {
            List<OptionViewModel<DrinkSize>> list = new List<OptionViewModel<DrinkSize>>();
            list.Add(new OptionViewModel<DrinkSize>(Strings.DrinkSize_Small, DrinkSize.Small, 0));
            list.Add(new OptionViewModel<DrinkSize>(Strings.DrinkSize_Medium, DrinkSize.Medium, 1));
            list.Add(new OptionViewModel<DrinkSize>(Strings.DrinkSize_Large, DrinkSize.Large, 2));

            foreach (OptionViewModel<DrinkSize> option in list)
                option.PropertyChanged += this.OnDrinkSizeOptionPropertyChanged;

            list.Sort();

            _availableDrinkSizes = new ReadOnlyCollection<OptionViewModel<DrinkSize>>(list);
        }

        void OnDrinkSizeOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OptionViewModel<DrinkSize> option = sender as OptionViewModel<DrinkSize>;
            if (option.IsSelected)
                this.CupOfCoffee.DrinkSize = option.GetValue();
        }

        #endregion // AvailableDrinkSizes

        #region DisplayName

        public override string DisplayName
        {
            get { return Strings.PageDisplayName_TypeAndSize; }
        }

        #endregion // DisplayName

        #endregion // Properties

        #region Methods

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return base.CupOfCoffee.DrinkSize.HasValue;
        }

        #endregion // Methods
    }
}