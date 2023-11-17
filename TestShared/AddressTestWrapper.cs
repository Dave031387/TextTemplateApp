namespace TestShared
{
    using ModelWrapperBase;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class AddressTestWrapper : ModelWrapper<Address>
    {
        public AddressTestWrapper(Address address) : base(address)
        {
        }

        public string? City
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool CityIsChanged => GetIsChanged(nameof(City));

        public string? CityOriginalValue => GetOriginalValue<string?>(nameof(City));

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public string? State
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool StateIsChanged => GetIsChanged(nameof(State));

        public string? StateOriginalValue => GetOriginalValue<string?>(nameof(State));

        public string? Street
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool StreetIsChanged => GetIsChanged(nameof(Street));

        public string? StreetNumber
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool StreetNumberIsChanged => GetIsChanged(nameof(StreetNumber));

        public string? StreetNumberOriginalValue => GetOriginalValue<string?>(nameof(StreetNumber));

        public string? StreetOriginalValue => GetOriginalValue<string?>(nameof(Street));

        public string? ZipCode
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool ZipCodeIsChanged => GetIsChanged(nameof(ZipCode));

        public string? ZipCodeOriginalValue => GetOriginalValue<string?>(nameof(ZipCode));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(State))
            {
                yield return new ValidationResult("State is required",
                    new[] { nameof(State) });
            }

            if (string.IsNullOrWhiteSpace(City))
            {
                yield return new ValidationResult("City is required",
                    new[] { nameof(City) });
            }
        }
    }
}