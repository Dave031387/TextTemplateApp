namespace ModelWrapperBase
{
    using System;
    using TestModels;
    using static TestShared.Globals;

    public class ModelWrapperTests
    {
        private const string Comment = nameof(FriendEmail.Comment);
        private const string Email = nameof(FriendEmail.Email);
        private const string Id = nameof(FriendEmail.Id);
        private static readonly string _changedComment = "Changed comment";
        private static readonly string _changedEmailAddress = "cool.cat@gmail.com";
        private static readonly string _invalidComment = "Contains invalid email address";
        private static readonly string _invalidEmailAddress = "john.doe";
        private static readonly int _invalidId = 0;
        private static readonly string _invalidPropertyName = "InvalidProperty";
        private static readonly string _validComment = "Contains valid email address.";
        private static readonly string _validEmailAddress = "john.doe@email.com";
        private static readonly int _validId = 1;

        private readonly FriendEmail _invalidEmail = new()
        {
            Id = _invalidId,
            Email = _invalidEmailAddress,
            Comment = _invalidComment
        };

        private readonly FriendEmail _validEmail = new()
        {
            Id = _validId,
            Email = _validEmailAddress,
            Comment = _validComment
        };

        [Fact]
        public void AcceptChanges_PropertiesAreModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _invalidEmailAddress
            };

            // Act
            modelWrapper.AcceptChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeTrue();
            modelWrapper.IsValid
                .Should()
                .BeFalse();
            modelWrapper.Model.Email
                .Should()
                .Be(_invalidEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_validEmailAddress);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreModifiedAndContainValidValues_AcceptsChanges()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail)
            {
                Email = _changedEmailAddress,
                Comment = _changedComment
            };

            // Act
            modelWrapper.AcceptChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeTrue();
            modelWrapper.Model.Email
                .Should()
                .Be(_changedEmailAddress);
            modelWrapper.Model.Comment
                .Should()
                .Be(_changedComment);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_changedEmailAddress);
            modelWrapper.TestGetOriginalValue(Comment)
                .Should()
                .Be(_changedComment);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreNotModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail);

            // Act
            modelWrapper.AcceptChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeFalse();
            modelWrapper.Model.Email
                .Should()
                .Be(_invalidEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_invalidEmailAddress);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreNotModifiedAndContainValidValues_DoesNothing()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Act
            modelWrapper.AcceptChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeTrue();
            modelWrapper.Model.Email
                .Should()
                .Be(_validEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_validEmailAddress);
        }

        [Fact]
        public void GetIsChanged_PropertyIsChanged_ReturnsTrue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Comment = _changedComment
            };

            // Act
            bool isChanged = modelWrapper.TestGetIsChanged(Comment);

            // Assert
            isChanged
                .Should()
                .BeTrue();
            modelWrapper.Comment
                .Should()
                .Be(_changedComment);
            modelWrapper.IsChanged
                .Should()
                .BeTrue();
            modelWrapper.TestGetIsChanged(Id)
                .Should()
                .BeFalse();
            modelWrapper.TestGetIsChanged(Email)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GetIsChanged_PropertyIsChangedAndThenChangedBack_ReturnsFalse()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Comment = _changedComment
            };
            modelWrapper.Comment = _validComment;

            // Act
            bool isChanged = modelWrapper.TestGetIsChanged(Comment);

            // Assert
            isChanged
                .Should()
                .BeFalse();
            modelWrapper.Comment
                .Should()
                .Be(_validComment);
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChanged_ReturnsOriginalValue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Comment = _changedComment
            };

            // Act
            string originalValue = modelWrapper.TestGetOriginalValue(Comment)!;

            // Assert
            originalValue
                .Should()
                .Be(_validComment);
            modelWrapper.Model.Comment
                .Should()
                .Be(_changedComment);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChangedAndThenChangedBack_ReturnsOriginalValue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Comment = _changedComment
            };
            modelWrapper.Comment = _validComment;

            // Act
            string originalValue = modelWrapper.TestGetOriginalValue(Comment)!;

            // Assert
            originalValue
                .Should()
                .Be(_validComment);
            modelWrapper.Model.Comment
                .Should()
                .Be(_validComment);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChangedTwice_ReturnsOriginalValue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Comment = _invalidComment
            };
            modelWrapper.Comment = _changedComment;

            // Act
            string originalValue = modelWrapper.TestGetOriginalValue(Comment)!;

            // Assert
            originalValue
                .Should()
                .Be(_validComment);
            modelWrapper.Model.Comment
                .Should()
                .Be(_changedComment);
        }

        [Fact]
        public void GetValue_InvalidPropertyName_ThrowsAnException()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);
            string expectedMessage = $"The property \"{_invalidPropertyName}\" is not a member of \"{nameof(FriendEmail)}\".";

            // Act/Assert
            try
            {
                _ = modelWrapper.TestGetValue(_invalidPropertyName);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<ArgumentException>();
                ex.Message
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void GetValue_PropertyNameIsNull_ThrowsAnException()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);
            string expectedMessage = "Value cannot be null. (Parameter 'propertyName')";

            // Act/Assert
            try
            {
                _ = modelWrapper.TestGetValue(null);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<ArgumentNullException>();
                ex.Message
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void GetValue_ValidPropertyName_ReturnsPropertyValue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Act
            string propertyValue = modelWrapper.TestGetValue(Comment)!;

            // Assert
            propertyValue
                .Should()
                .Be(_validComment);
        }

        [Fact]
        public void IsChanged_AfterObjectConstruction_ReturnsFalse()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_PropertySetToNewValue_ReturnsTrue()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _changedEmailAddress
            };

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsChanged_PropertySetToNewValueAndThenBackToOriginal_ReturnsFalse()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _changedEmailAddress
            };

            // Act
            modelWrapper.Email = _validEmailAddress;

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_PropertySetToOriginalValue_ReturnsFalse()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _validEmailAddress
            };

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_TwoPropertiesSetToNewValuesAndThenOneChangedBackToOriginal_ReturnsTrue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _changedEmailAddress,
                Comment = _changedComment
            };

            // Act
            modelWrapper.Email = _validEmailAddress;

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_AfterConstructionWithInvalidModel_ReturnsFalse()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail);

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_AfterConstructionWithValidModel_ReturnsTrue()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_InvalidPropertySetToValidValue_ReturnsTrue()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail)
            {
                Email = _validEmailAddress
            };

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_OneOfTwoInvalidPropertyValuesSetToValidValue_ReturnsFalse()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail)
            {
                Email = string.Empty,
                Comment = string.Empty
            };

            // Act
            modelWrapper.Email = _validEmailAddress;

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_ValidPropertySetToInvalidValue_ReturnsFalse()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _invalidEmailAddress
            };

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ModelWrapper_ConstructUsingInvalidModel_InitializesModelProperty()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail);

            // Assert
            modelWrapper.Model
                .Should()
                .Be(_invalidEmail);
        }

        [Fact]
        public void ModelWrapper_ConstructUsingNullModel_ThrowsException()
        {
            // Arrange
            string expectedMessage = "Value cannot be null. (Parameter 'model')";

            // Act/Assert
            try
            {
                FriendEmailTestWrapper modelWrapper = new(null!);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<ArgumentNullException>();
                ex.Message
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void ModelWrapper_ConstructUsingValidModel_InitializesModelProperty()
        {
            // Arrange/Act
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Assert
            modelWrapper.Model
                .Should()
                .Be(_validEmail);
        }

        [Fact]
        public void RejectChanges_PropertiesAreModifiedAndContainInvalidValues_RejectsChanges()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail)
            {
                Email = _invalidEmailAddress
            };

            // Act
            modelWrapper.RejectChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeTrue();
            modelWrapper.Model.Email
                .Should()
                .Be(_validEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_validEmailAddress);
        }

        [Fact]
        public void RejectChanges_PropertiesAreModifiedAndContainValidValues_RejectsChanges()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail)
            {
                Email = _validEmailAddress
            };

            // Act
            modelWrapper.RejectChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeFalse();
            modelWrapper.Model.Email
                .Should()
                .Be(_invalidEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_invalidEmailAddress);
        }

        [Fact]
        public void RejectChanges_PropertiesAreNotModifiedAndContainInvalidValues_RejectsChanges()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail);

            // Act
            modelWrapper.RejectChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeFalse();
            modelWrapper.Model.Email
                .Should()
                .Be(_invalidEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_invalidEmailAddress);
        }

        [Fact]
        public void RejectChanges_PropertiesAreNotModifiedAndContainValidValues_DoesNothing()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Act
            modelWrapper.RejectChanges();

            // Assert
            modelWrapper.IsChanged
                .Should()
                .BeFalse();
            modelWrapper.IsValid
                .Should()
                .BeTrue();
            modelWrapper.Model.Email
                .Should()
                .Be(_validEmailAddress);
            modelWrapper.TestGetOriginalValue(Email)
                .Should()
                .Be(_validEmailAddress);
        }

        [Fact]
        public void SetValue_InvalidPropertyName_ThrowsAnException()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);
            string expectedMessage = $"The property \"{_invalidPropertyName}\" is not a member of \"{nameof(FriendEmail)}\".";

            // Act/Assert
            try
            {
                modelWrapper.TestSetValue(_changedComment, _invalidPropertyName);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<ArgumentException>();
                ex.Message
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void SetValue_InvalidPropertyValue_FailsValidation()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Act
            modelWrapper.TestSetValue(_invalidEmailAddress, Email);
            List<string> errors = (modelWrapper.GetErrors(Email) as List<string>)!;

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeFalse();
            errors
                .Should()
                .ContainEquivalentOf("Email is not a valid email address");
        }

        [Fact]
        public void SetValue_PropertyNameIsNull_ThrowsAnException()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);
            string expectedMessage = "Value cannot be null. (Parameter 'propertyName')";

            // Act/Assert
            try
            {
                modelWrapper.TestSetValue(_changedComment, null);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<ArgumentNullException>();
                ex.Message
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void SetValue_ValidPropertyName_SetsTheNewPropertyValue()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_validEmail);

            // Act
            modelWrapper.TestSetValue(_changedComment, Comment);

            // Assert
            modelWrapper.Model.Comment
                .Should()
                .Be(_changedComment);
        }

        [Fact]
        public void SetValue_ValidPropertyValue_PassesValidation()
        {
            // Arrange
            FriendEmailTestWrapper modelWrapper = new(_invalidEmail);

            // Act
            modelWrapper.TestSetValue(_validEmailAddress, Email);
            List<string> errors = (modelWrapper.GetErrors(Email) as List<string>)!;

            // Assert
            modelWrapper.IsValid
                .Should()
                .BeTrue();
            errors
                .Should()
                .BeEmpty();
        }
    }
}