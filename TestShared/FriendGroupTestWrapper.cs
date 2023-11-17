namespace TestShared
{
    using ModelWrapperBase;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class FriendGroupTestWrapper : ModelWrapper<FriendGroup>
    {
        public FriendGroupTestWrapper(FriendGroup friendGroup) : base(friendGroup)
        {
        }

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public string? Name
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool NameIsChanged => GetIsChanged(nameof(Name));

        public string? NameOriginalValue => GetOriginalValue<string?>(nameof(Name));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Name is required",
                    new[] { nameof(Name) });
            }
        }
    }
}