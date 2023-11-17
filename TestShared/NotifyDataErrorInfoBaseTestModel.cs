namespace TestShared
{
    using ModelWrapperBase;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class NotifyDataErrorInfoBaseTestModel : NotifyDataErrorInfoBase, IValidatableObject
    {
        private readonly Regex _validEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public int Age { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public void Clear() => ClearErrors();

        public void Validate()
        {
            List<ValidationResult> results = new();
            ValidationContext context = new(this);
            Validator.TryValidateObject(this, context, results);

            if (results.Any())
            {
                List<string> propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                foreach (string propertyName in propertyNames)
                {
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList()!;
                    OnErrorsChanged(propertyName);
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("First Name is required", new[] { nameof(FirstName) });
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Last Name is required", new[] { nameof(LastName) });
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required", new[] { nameof(Email) });
            }
            else if (!_validEmail.IsMatch(Email))
            {
                yield return new ValidationResult("Email is not a valid email address", new[] { nameof(Email) });
            }

            if (Age < 18)
            {
                yield return new ValidationResult("Age must not be less than 18", new[] { nameof(Age) });
            }

            if (Age > 100)
            {
                yield return new ValidationResult("Age must not be greater than 100", new[] { nameof(Age) });
            }
        }
    }
}