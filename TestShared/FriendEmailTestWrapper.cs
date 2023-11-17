namespace TestShared
{
    using ModelWrapperBase;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    using TestModels;

    public class FriendEmailTestWrapper : ModelWrapper<FriendEmail>
    {
        private readonly Regex _validEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public FriendEmailTestWrapper(FriendEmail model) : base(model)
        {
        }

        public string? Comment
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool CommentIsChanged => GetIsChanged(nameof(Comment));

        public string? CommentOriginalValue => GetOriginalValue<string>(nameof(Comment));

        public string? Email
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public string? EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public bool TestGetIsChanged(string propertyName) => GetIsChanged(propertyName);

        public string? TestGetOriginalValue(string propertyName) => GetOriginalValue<string?>(propertyName);

        public string? TestGetValue(string? propertyName) => GetValue<string?>(propertyName);

        public void TestSetValue(string? value, string? propertyName) => SetValue(value, propertyName);

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required",
                    new[] { nameof(Email) });
            }

            if (!_validEmail.IsMatch(Email!))
            {
                yield return new ValidationResult("Email is not a valid email address",
                    new[] { nameof(Email) });
            }

            {
                if (string.IsNullOrWhiteSpace(Comment))
                {
                    yield return new ValidationResult("Comment is required",
                        new[] { nameof(Comment) });
                }
            }
        }
    }
}