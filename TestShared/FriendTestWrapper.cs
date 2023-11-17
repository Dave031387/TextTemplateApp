namespace TestShared
{
    using ModelWrapperBase;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using TestModels;

    public class FriendTestWrapper : ModelWrapper<Friend>
    {
        public FriendTestWrapper(Friend friend) : base(friend)
        {
        }

        public AddressTestWrapper? Address { get; private set; }

        public DateTime? Birthday
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public bool BirthdayIsChanged => GetIsChanged(nameof(Birthday));

        public DateTime? BirthdayOriginalValue => GetOriginalValue<DateTime?>(nameof(Birthday));

        public ChangeTrackingCollection<FriendEmailTestWrapper>? Emails { get; private set; }

        public string FirstName
        {
            get => GetValue<string>()!;
            set => SetValue(value);
        }

        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));

        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName))!;

        public int FriendGroupId
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool FriendGroupIdIsChanged => GetIsChanged(nameof(FriendGroupId));

        public int FriendGroupIdOriginalValue => GetOriginalValue<int>(nameof(FriendGroupId));

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public bool IsDeveloper
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IsDeveloperIsChanged => GetIsChanged(nameof(IsDeveloper));

        public bool IsDeveloperOriginalValue => GetOriginalValue<bool>(nameof(IsDeveloper));

        public string LastName
        {
            get => GetValue<string>()!;
            set => SetValue(value);
        }

        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));

        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName))!;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("First name is required",
                    new[] { nameof(FirstName) });
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Last name is required",
                    new[] { nameof(LastName) });
            }

            if (IsDeveloper && Emails!.Count == 0)
            {
                yield return new ValidationResult("A developer must have an email-address",
                    new[] { nameof(IsDeveloper), nameof(Emails) });
            }
        }

        protected override void InitializeCollectionProperties()
        {
            if (Model.Emails is null)
            {
                throw new ArgumentException("Emails cannot be null");
            }

            Emails = new ChangeTrackingCollection<FriendEmailTestWrapper>(Model.Emails.Select(e => new FriendEmailTestWrapper(e)));
            RegisterCollection(Emails, Model.Emails);
        }

        protected override void InitializeComplexProperties()
        {
            if (Model.Address is null)
            {
                throw new ArgumentException("Address cannot be null");
            }

            Address = new AddressTestWrapper(Model.Address);
            RegisterComplex(Address);
        }
    }
}