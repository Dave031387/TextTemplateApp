namespace ModelWrapperBase
{
    using System.Collections.Generic;

    public class NotifyDataErrorInfoBaseTests
    {
        private const string Age = nameof(NotifyDataErrorInfoBaseTestModel.Age);
        private const string Email = nameof(NotifyDataErrorInfoBaseTestModel.Email);
        private const string FirstName = nameof(NotifyDataErrorInfoBaseTestModel.FirstName);
        private const string LastName = nameof(NotifyDataErrorInfoBaseTestModel.LastName);

        [Fact]
        public void ClearErrors_ErrorsListContainsMultipleErrors_ClearsAllErrorsFromTheList()
        {
            // Arrange
            NotifyDataErrorInfoBaseTestModel model = new()
            {
                FirstName = "John",
                Age = 9,
                Email = "@gmail.com"
            };

            model.Validate();
            model.HasErrors
                .Should()
                .BeTrue("The Errors collection shouldn't be empty at the start of this test.");

            // Act
            model.Clear();

            // Assert
            model.HasErrors
                .Should()
                .BeFalse();
            ValidateErrors(model, FirstName, 0);
            ValidateErrors(model, LastName, 0);
            ValidateErrors(model, Age, 0);
            ValidateErrors(model, Email, 0);
        }

        [Fact]
        public void Validate_ModelContainsMultipleErrors_ReturnsAllErrorMessages()
        {
            // Arrange
            NotifyDataErrorInfoBaseTestModel model = new()
            {
                LastName = "Doe",
                Age = 101,
                Email = "john.doe"
            };

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeTrue();
            ValidateErrors(model, FirstName, 1, "First Name is required");
            ValidateErrors(model, LastName, 0);
            ValidateErrors(model, Age, 1, "Age must not be greater than 100");
            ValidateErrors(model, Email, 1, "Email is not a valid email address");
        }

        [Fact]
        public void Validate_ModelContainsOneError_ReturnsErrorMessage()
        {
            // Arrange
            NotifyDataErrorInfoBaseTestModel model = new()
            {
                LastName = "Doe",
                Age = 55,
                Email = "johndoe@gmail.com"
            };

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeTrue();
            ValidateErrors(model, FirstName, 1, "First Name is required");
            ValidateErrors(model, LastName, 0);
            ValidateErrors(model, Age, 0);
            ValidateErrors(model, Email, 0);
        }

        [Fact]
        public void Validate_NoValidationErrors_ErrorsListShouldBeEmpty()
        {
            // Arrange
            NotifyDataErrorInfoBaseTestModel model = new()
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 55,
                Email = "johndoe@gmail.com"
            };

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeFalse();
            ValidateErrors(model, FirstName, 0);
            ValidateErrors(model, LastName, 0);
            ValidateErrors(model, Age, 0);
            ValidateErrors(model, Email, 0);
        }

        private static void ValidateErrors(NotifyDataErrorInfoBaseTestModel model, string propertyName, int expectedErrorCount, string expectedErrorMessage = "")
        {
            List<string> errors = (model.GetErrors(propertyName) as List<string>)!;

            errors
                .Should()
                .NotBeNull();

            if (expectedErrorCount < 1)
            {
                errors
                    .Should()
                    .BeEmpty();
            }
            else
            {
                errors
                    .Should()
                    .HaveCount(expectedErrorCount);
                errors
                    .Should()
                    .ContainEquivalentOf(expectedErrorMessage);
            }
        }
    }
}