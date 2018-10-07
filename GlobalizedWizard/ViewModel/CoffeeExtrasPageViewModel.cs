using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CoffeeLibrary;
using ZingitWizard.Resources;

namespace ZingitWizard.ViewModel
{
    /// <summary>
    /// The wizard page that allows the user to select additional options
    /// that they might want to add to their cup of coffee.
    /// </summary>
    public class CoffeeExtrasPageViewModel : CoffeeWizardPageViewModelBase
    {
        #region Fields

        ReadOnlyCollection<OptionViewModel<Flavorings>> _availableFlavorings;
        ReadOnlyCollection<OptionViewModel<Temperature>> _availableTemperatures;

        #endregion // Fields

        #region Constructor

        public CoffeeExtrasPageViewModel(CupOfCoffee cupOfCoffee)
            : base(cupOfCoffee)
        {
        }

        #endregion // Constructor

        #region Properties

        #region AvailableFlavorings

        /// <summary>
        /// Returns a read-only collection of all flavorings that the user can select.
        /// </summary>
        public ReadOnlyCollection<OptionViewModel<Flavorings>> AvailableFlavorings
        {
            get
            {
                if (_availableFlavorings == null)
                    this.CreateAvailableFlavorings();

                return _availableFlavorings;
            }
        }

        void CreateAvailableFlavorings()
        {
            List<OptionViewModel<Flavorings>> list = new List<OptionViewModel<Flavorings>>();
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_Caramel, Flavorings.Caramel));
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_Gingerbread, Flavorings.Gingerbread));
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_Hazelnut, Flavorings.Hazelnut));
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_IrishCream, Flavorings.IrishCream));
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_Pumpkin, Flavorings.Pumpkin));
            list.Add(new OptionViewModel<Flavorings>(Strings.Flavoring_Vanilla, Flavorings.Vanilla));

            foreach (OptionViewModel<Flavorings> option in list)
                option.PropertyChanged += this.OnFlavoringOptionPropertyChanged;

            list.Sort();

            _availableFlavorings = new ReadOnlyCollection<OptionViewModel<Flavorings>>(list);
        }

        void OnFlavoringOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Since the user can select more than one additional flavoring to add to their
            // cup of coffee, we check all available flavorings and combine the enum values.

            Flavorings flavorings = Flavorings.None;

            foreach (OptionViewModel<Flavorings> option in this.AvailableFlavorings)
                if (option.IsSelected)
                    flavorings |= option.GetValue();

            base.CupOfCoffee.Flavorings = flavorings;
        }

        #endregion // AvailableFlavorings

        #region AvailableTemperatures

        /// <summary>
        /// Returns a read-only collection of all coffee temperatures that the user can select.
        /// </summary>
        public ReadOnlyCollection<OptionViewModel<Temperature>> AvailableTemperatures
        {
            get
            {
                if (_availableTemperatures == null)
                    this.CreateAvailableTemperatures();

                return _availableTemperatures;
            }
        }

        void CreateAvailableTemperatures()
        {
            List<OptionViewModel<Temperature>> list = new List<OptionViewModel<Temperature>>();
            list.Add(new OptionViewModel<Temperature>(Strings.Temperature_ExtraHot, Temperature.ExtraHot, 1));
            list.Add(new OptionViewModel<Temperature>(Strings.Temperature_Normal, Temperature.Normal, 0));
            list.Add(new OptionViewModel<Temperature>(Strings.Temperature_WithIce, Temperature.WithIce, 2));

            foreach (OptionViewModel<Temperature> option in list)
            {
                // Select the default value.
                if (option.GetValue() == base.CupOfCoffee.Temperature)
                    option.IsSelected = true;

                option.PropertyChanged += this.OnTemperatureOptionPropertyChanged;
            }

            list.Sort();

            _availableTemperatures = new ReadOnlyCollection<OptionViewModel<Temperature>>(list);
        }

        void OnTemperatureOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OptionViewModel<Temperature> option = sender as OptionViewModel<Temperature>;
            if (option.IsSelected)
                this.CupOfCoffee.Temperature = option.GetValue();
        }

        #endregion // AvailableTemperatures

        #region DisplayName

        public override string DisplayName
        {
            get { return Strings.PageDisplayName_Extras; }
        }

        #endregion // DisplayName

        #endregion // Properties

        #region Methods

        internal override bool IsValid()
        {
            return true;
        }

        #endregion // Methods
    }
}