namespace TextTemplateApp
{
    using ModelWrapperBase;
    using TestModels;
     
    public partial class AddressWrapper : ModelWrapper<Address>
    {
        public AddressWrapper(Address model) : base(model)
        {
        }
         
        public System.String City
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool CityIsChanged => GetIsChanged(nameof(City));
         
        public System.String CityOriginalValue => GetOriginalValue<System.String>(nameof(City));
         
        public System.Int32 Id
        {
            get => GetValue<System.Int32>();
            set => SetValue(value);
        }
         
        public bool IdIsChanged => GetIsChanged(nameof(Id));
         
        public System.Int32 IdOriginalValue => GetOriginalValue<System.Int32>(nameof(Id));
         
        public System.String State
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool StateIsChanged => GetIsChanged(nameof(State));
         
        public System.String StateOriginalValue => GetOriginalValue<System.String>(nameof(State));
         
        public System.String Street
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool StreetIsChanged => GetIsChanged(nameof(Street));
         
        public System.String StreetOriginalValue => GetOriginalValue<System.String>(nameof(Street));
         
        public System.String StreetNumber
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool StreetNumberIsChanged => GetIsChanged(nameof(StreetNumber));
         
        public System.String StreetNumberOriginalValue => GetOriginalValue<System.String>(nameof(StreetNumber));
         
        public System.String ZipCode
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool ZipCodeIsChanged => GetIsChanged(nameof(ZipCode));
         
        public System.String ZipCodeOriginalValue => GetOriginalValue<System.String>(nameof(ZipCode));
    }
}
