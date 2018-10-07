using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CoffeeLibrary;
using ZingitWizard.Resources;

namespace ZingitWizard.ViewModel
{
    /// <summary>
    /// This page in the wizard displays all of the options selected by the user
    /// in the previous pages.
    /// </summary>
    public class CoffeeSummaryPageViewModel : CoffeeWizardPageViewModelBase
    {
        #region Fields

        readonly ReadOnlyCollection<OptionViewModel<BeanType>> _availableBeanTypes;
        readonly ReadOnlyCollection<OptionViewModel<DrinkSize>> _availableDrinkSizes;
        readonly ReadOnlyCollection<OptionViewModel<Flavorings>> _availableFlavorings;
        readonly ReadOnlyCollection<OptionViewModel<Temperature>> _availableTemperatures;

        #endregion // Fields

        #region Constructor

        public CoffeeSummaryPageViewModel(
            ReadOnlyCollection<OptionViewModel<BeanType>> availableBeanTypes,
            ReadOnlyCollection<OptionViewModel<DrinkSize>> availableDrinkSizes,
            ReadOnlyCollection<OptionViewModel<Flavorings>> availableFlavorings,
            ReadOnlyCollection<OptionViewModel<Temperature>> availableTemperatures)
            : base(null)
        {
            _availableBeanTypes = availableBeanTypes;
            _availableDrinkSizes = availableDrinkSizes;
            _availableFlavorings = availableFlavorings;
            _availableTemperatures = availableTemperatures;
        }

        #endregion // Constructor

        #region Properties

        public string BeanType
        {
            get { return GetSelectedOptionDisplayName(_availableBeanTypes); }
        }

        public override string DisplayName
        {
            get { return Strings.PageDisplayName_Summary; }
        }

        public string DrinkSize
        {
            get { return GetSelectedOptionDisplayName(_availableDrinkSizes); }
        }

        public string Flavorings
        {
            get
            {
                List<String> flavoringNames =
                    (from flavoringOption in _availableFlavorings
                     where flavoringOption.IsSelected
                     select flavoringOption.DisplayName).ToList();

                string returnValue = null;

                for (int i = 0; i < flavoringNames.Count; ++i)
                {
                    returnValue += flavoringNames[i];

                    if (i < flavoringNames.Count - 1)
                        returnValue += ", ";
                }

                return returnValue ?? Strings.CoffeeSummaryPageViewModel_Label_NoFlavoringsSelected;
            }
        }

        public string Temperature
        {
            get { return GetSelectedOptionDisplayName(_availableTemperatures); }
        }

        #endregion // Properties

        #region Methods

        internal override bool IsValid()
        {
            return true;
        }

        #endregion // Methods

        #region Private Helpers

        static string GetSelectedOptionDisplayName<T>(ReadOnlyCollection<OptionViewModel<T>> availableOptions)
        {
            if (availableOptions == null)
                throw new ArgumentNullException("availableOptions");

            return (from option in availableOptions
                    where option.IsSelected
                    select option.DisplayName).FirstOrDefault();
        }

        #endregion // Private Helpers
    }
}