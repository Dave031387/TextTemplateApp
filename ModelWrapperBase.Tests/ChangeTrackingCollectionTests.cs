namespace ModelWrapperBase
{
    using TestModels;

    public class ChangeTrackingCollectionTests
    {
        private readonly Address _address = new()
        {
            Id = 30,
            Street = "1234",
            City = "Hometown",
            State = "NY",
            ZipCode = "12345"
        };

        private readonly FriendEmail _invalidEmail1 = new();

        private readonly FriendEmail _invalidEmail2 = new();

        private readonly FriendEmail _validEmail1 = new();

        private readonly FriendEmail _validEmail2 = new();

        private readonly FriendEmail _validEmail3 = new();

        public ChangeTrackingCollectionTests() => ResetTestEmails();

        [Fact]
        public void AcceptChanges_CollectionContainsInvalidItems_DoesNothing()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail2 = testWrapper.Emails[1];
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails[2];
            string modifiedEmailValue = "invalid";
            modifiedEmail.Email = modifiedEmailValue;

            // Act
            testWrapper.AcceptChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 1, 0, true, false);
            AssertCollectionItems(testWrapper.Emails.ModifiedItems, modifiedEmail);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail1, unmodifiedEmail2, modifiedEmail);
            AssertParentWrapperProperties(testWrapper, true, false);
            modifiedEmail.Email
                .Should()
                .Be(modifiedEmailValue);
        }

        [Fact]
        public void AcceptChanges_CollectionContainsValidAddedItems_AcceptsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper addedEmail1 = new(new()
            {
                Id = 11,
                Email = "dan@somewhere.com",
                Comment = "Added email #1"
            });
            FriendEmailTestWrapper addedEmail2 = new(new()
            {
                Id = 12,
                Email = "ed@nowhere.com",
                Comment = "Added email #2"
            });
            testWrapper.Emails.Add(addedEmail1);
            testWrapper.Emails.Add(addedEmail2);

            // Act
            testWrapper.AcceptChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail, addedEmail1, addedEmail2);
            AssertParentWrapperProperties(testWrapper, false, true);
        }

        [Fact]
        public void AcceptChanges_CollectionContainsValidModifiedItems_AcceptsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _invalidEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail1 = testWrapper.Emails![1];
            FriendEmailTestWrapper modifiedEmail2 = testWrapper.Emails[0];
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails[2];
            string modifiedCommentValue = "Modified email";
            string modifiedEmailValue = "modified@email.com";
            modifiedEmail1.Comment = modifiedCommentValue;
            modifiedEmail2.Email = modifiedEmailValue;

            // Act
            testWrapper.AcceptChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, modifiedEmail1, modifiedEmail2, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
            modifiedEmail1.Comment
                .Should()
                .Be(modifiedCommentValue);
            modifiedEmail2.Email
                .Should()
                .Be(modifiedEmailValue);
        }

        [Fact]
        public void AcceptChanges_CollectionIsValidAndHasAddedModifiedAndRemovedItems_AcceptsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _invalidEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![1];
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails[2];
            FriendEmailTestWrapper removedEmail = testWrapper.Emails[0];
            FriendEmailTestWrapper addedEmail = new(new()
            {
                Id = 4,
                Email = "john.doe@company.com",
                Comment = "Added email"
            });
            testWrapper.Emails.Remove(removedEmail);
            testWrapper.Emails.Add(addedEmail);
            string modifiedCommentValue = "Modified email";
            modifiedEmail.Comment = modifiedCommentValue;

            // Act
            testWrapper.AcceptChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail, modifiedEmail, addedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
            modifiedEmail.Comment
                .Should()
                .Be(modifiedCommentValue);
        }

        [Fact]
        public void AcceptChanges_CollectionIsValidAndHasRemovedItems_AcceptsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _invalidEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper removedEmail1 = testWrapper.Emails[1];
            FriendEmailTestWrapper removedEmail2 = testWrapper.Emails[2];
            testWrapper.Emails.Remove(removedEmail1);
            testWrapper.Emails.Remove(removedEmail2);

            // Act
            testWrapper.AcceptChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
        }

        [Fact]
        public void AddedItems_AddAndThenRemoveInvalidItemToCollection_ShouldRevertToOriginalState()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper invalidEmail = new(_invalidEmail2);
            testWrapper.Emails.Add(invalidEmail);

            // Act
            testWrapper.Emails.Remove(invalidEmail);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
        }

        [Fact]
        public void AddedItems_AddInvalidItemsToCollection_UpdatesAddedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper invalidEmail = new(_invalidEmail2);
            FriendEmailTestWrapper validEmail = new(_validEmail3);

            // Act
            testWrapper.Emails.Add(invalidEmail);
            testWrapper.Emails.Add(validEmail);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 2, 0, 0, true, false);
            AssertCollectionItems(testWrapper.Emails.AddedItems, invalidEmail, validEmail);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail, invalidEmail, validEmail);
            AssertParentWrapperProperties(testWrapper, true, false);
        }

        [Fact]
        public void AddedItems_AddValidItemsToCollection_UpdatesAddedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper validEmail1 = new(_validEmail2);
            FriendEmailTestWrapper validEmail2 = new(_validEmail3);

            // Act
            testWrapper.Emails.Add(validEmail1);
            testWrapper.Emails.Add(validEmail2);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 2, 0, 0, true, true);
            AssertCollectionItems(testWrapper.Emails.AddedItems, validEmail1, validEmail2);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail, validEmail1, validEmail2);
            AssertParentWrapperProperties(testWrapper, true, true);
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructUsingEmptyList_InitializesProperties()
        {
            // Arrange/Act
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false);

            // Assert
            AssertCollectionProperties(testWrapper.Emails!, 0, 0, 0, false, true);
            AssertParentWrapperProperties(testWrapper, false, true);
            testWrapper.Emails
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructUsingListOfValidEmails_InitializesProperties()
        {
            // Arrange/Act
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _validEmail2, _validEmail3);

            // Assert
            AssertCollectionProperties(testWrapper.Emails!, 0, 0, 0, false, true);
            AssertParentWrapperProperties(testWrapper, false, true);
            testWrapper.Emails
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructUsingListWithInvalidEmails_InitializesProperties()
        {
            // Arrange/Act
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _invalidEmail2, _validEmail3);

            // Assert
            AssertCollectionProperties(testWrapper.Emails!, 0, 0, 0, false, false);
            AssertParentWrapperProperties(testWrapper, false, false);
            testWrapper.Emails
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public void ModifiedItems_MakeInvalidChangesAndThenReverseThem_ShouldRevertToOriginalState()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails![1];
            string originalValue = modifiedEmail.Email!;
            modifiedEmail.Email = "invalid";

            // Act
            modifiedEmail.Email = originalValue;

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertParentWrapperProperties(testWrapper, false, true);
            modifiedEmail.Email
                .Should()
                .Be(originalValue);
        }

        [Fact]
        public void ModifiedItems_MakeInvalidChangesToAValidItem_UpdatesModifiedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails![1];
            string modifiedEmailValue = "invalid";

            // Act
            modifiedEmail.Email = modifiedEmailValue;

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 1, 0, true, false);
            AssertCollectionItems(testWrapper.Emails.ModifiedItems, modifiedEmail);
            AssertParentWrapperProperties(testWrapper, true, false);
            modifiedEmail.Email
                .Should()
                .Be(modifiedEmailValue);
        }

        [Fact]
        public void ModifiedItems_MakeValidChangesAndThenReverseThem_ShouldRevertToOriginalState()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _invalidEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails![0];
            string originalValue = modifiedEmail.Email!;
            modifiedEmail.Email = "john.doe@outlook.com";

            // Act
            modifiedEmail.Email = originalValue;

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, false);
            AssertParentWrapperProperties(testWrapper, false, false);
            modifiedEmail.Email
                .Should()
                .Be(originalValue);
        }

        [Fact]
        public void ModifiedItems_MakeValidChangesToAnInvalidItem_UpdatesModifiedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _invalidEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails![0];
            string modifiedEmailValue = "john.doe@outlook.com";

            // Act
            modifiedEmail.Email = modifiedEmailValue;

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 1, 0, true, true);
            AssertCollectionItems(testWrapper.Emails.ModifiedItems, modifiedEmail);
            AssertParentWrapperProperties(testWrapper, true, true);
            modifiedEmail.Email
                .Should()
                .Be(modifiedEmailValue);
        }

        [Fact]
        public void ModifiedItems_ModifyMultipleItems_UpdatesModifiedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail1 = testWrapper.Emails![1];
            FriendEmailTestWrapper modifiedEmail2 = testWrapper.Emails[0];
            string modifiedEmailValue = "sally@email.com";
            string modifiedCommentValue = "Modified the comment";

            // Act
            modifiedEmail1.Email = modifiedEmailValue;
            modifiedEmail2.Comment = modifiedCommentValue;

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 2, 0, true, true);
            AssertCollectionItems(testWrapper.Emails.ModifiedItems, modifiedEmail1, modifiedEmail2);
            AssertParentWrapperProperties(testWrapper, true, true);
            modifiedEmail1.Email
                .Should()
                .Be(modifiedEmailValue);
            modifiedEmail2.Comment
                .Should()
                .Be(modifiedCommentValue);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenAdded_RejectsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper addedEmail1 = new(new()
            {
                Id = 11,
                Email = "invalid",
                Comment = "Added email #1"
            });
            FriendEmailTestWrapper addedEmail2 = new(new()
            {
                Id = 12,
                Email = "dan@gmail.com",
                Comment = "Added email #2"
            });
            testWrapper.Emails.Add(addedEmail1);
            testWrapper.Emails.Add(addedEmail2);

            // Act
            testWrapper.RejectChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenAddedModifiedAndRemoved_RejectsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper removedEmail = testWrapper.Emails[1];
            FriendEmailTestWrapper modifiedEmail = testWrapper.Emails[2];
            FriendEmailTestWrapper addedEmail = new(new()
            {
                Id = 11,
                Email = "bob@spam.com",
                Comment = "Added email"
            });
            string originalEmailValue = modifiedEmail.Email!;
            modifiedEmail.Email = "invalid";
            testWrapper.Emails.Add(addedEmail);
            testWrapper.Emails.Remove(removedEmail);

            // Act
            testWrapper.RejectChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail, removedEmail, modifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
            modifiedEmail.Email
                .Should()
                .Be(originalEmailValue);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenModified_RejectsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper modifiedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails[1];
            FriendEmailTestWrapper modifiedEmail2 = testWrapper.Emails[2];
            string originalEmailValue1 = modifiedEmail1.Email!;
            string originalEmailValue2 = modifiedEmail2.Email!;
            modifiedEmail1.Email = "johnny@blueskies.com";
            modifiedEmail2.Email = "invalid";

            // Act
            testWrapper.RejectChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, modifiedEmail1, unmodifiedEmail, modifiedEmail2);
            AssertParentWrapperProperties(testWrapper, false, true);
            modifiedEmail1.Email
                .Should()
                .Be(originalEmailValue1);
            modifiedEmail2.Email
                .Should()
                .Be(originalEmailValue2);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenRemoved_RejectsChanges()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper removedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper removedEmail2 = testWrapper.Emails[1];
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails[2];
            string originalEmailValue = removedEmail2.Email!;
            removedEmail2.Email = "invalid";
            testWrapper.Emails.Remove(removedEmail1);
            testWrapper.Emails.Remove(removedEmail2);

            // Act
            testWrapper.RejectChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, true);
            AssertCollectionItems(testWrapper.Emails, removedEmail1, removedEmail2, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, false, true);
            removedEmail2.Email
                .Should()
                .Be(originalEmailValue);
        }

        [Fact]
        public void RejectChanges_NothingChangedInTheCollection_DoesNothing()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(false, _validEmail1, _invalidEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail2 = testWrapper.Emails[1];
            FriendEmailTestWrapper unmodifiedEmail3 = testWrapper.Emails[2];

            // Act
            testWrapper.RejectChanges();

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 0, false, false);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail1, unmodifiedEmail2, unmodifiedEmail3);
            AssertParentWrapperProperties(testWrapper, false, false);
        }

        [Fact]
        public void RemovedItems_MakeInvalidChangesToAnItemAndThenRemoveIt_UpdatesRemovedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _validEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail2 = testWrapper.Emails[1];
            FriendEmailTestWrapper removedEmail = testWrapper.Emails[2];
            removedEmail.Email = "invalid";

            // Act
            testWrapper.Emails.Remove(removedEmail);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 1, true, true);
            AssertCollectionItems(testWrapper.Emails.RemovedItems, removedEmail);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail1, unmodifiedEmail2);
            AssertParentWrapperProperties(testWrapper, true, true);
        }

        [Fact]
        public void RemovedItems_RemoveInvalidItem_UpdatesRemovedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _invalidEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail1 = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail2 = testWrapper.Emails[2];
            FriendEmailTestWrapper removedEmail = testWrapper.Emails[1];

            // Act
            testWrapper.Emails.Remove(removedEmail);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 1, true, true);
            AssertCollectionItems(testWrapper.Emails.RemovedItems, removedEmail);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail1, unmodifiedEmail2);
            AssertParentWrapperProperties(testWrapper, true, true);
        }

        [Fact]
        public void RemovedItems_RemoveMultipleItems_UpdatesRemovedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _invalidEmail2, _validEmail3);
            FriendEmailTestWrapper unmodifiedEmail = testWrapper.Emails![2];
            FriendEmailTestWrapper removedEmail1 = testWrapper.Emails[1];
            FriendEmailTestWrapper removedEmail2 = testWrapper.Emails[0];

            // Act
            testWrapper.Emails.Remove(removedEmail1);
            testWrapper.Emails.Remove(removedEmail2);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 2, true, true);
            AssertCollectionItems(testWrapper.Emails.RemovedItems, removedEmail1, removedEmail2);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail);
            AssertParentWrapperProperties(testWrapper, true, true);
        }

        [Fact]
        public void RemovedItems_RemoveValidItem_UpdatesRemovedItems()
        {
            // Arrange
            FriendTestWrapper testWrapper = GetFriendTestWrapper(true, _validEmail1, _invalidEmail2, _validEmail3);
            FriendEmailTestWrapper removedEmail = testWrapper.Emails![0];
            FriendEmailTestWrapper unmodifiedEmail1 = testWrapper.Emails[1];
            FriendEmailTestWrapper unmodifiedEmail2 = testWrapper.Emails[2];

            // Act
            testWrapper.Emails.Remove(removedEmail);

            // Assert
            AssertCollectionProperties(testWrapper.Emails, 0, 0, 1, true, false);
            AssertCollectionItems(testWrapper.Emails.RemovedItems, removedEmail);
            AssertCollectionItems(testWrapper.Emails, unmodifiedEmail1, unmodifiedEmail2);
            AssertParentWrapperProperties(testWrapper, true, false);
        }

        private static void AssertCollectionItems(IEnumerable<FriendEmailTestWrapper> actualEmailList, params FriendEmailTestWrapper[] expectedEmails)
        {
            if (expectedEmails.Length == 0)
            {
                actualEmailList
                    .Should()
                    .BeEmpty();
            }
            else
            {
                List<FriendEmailTestWrapper> expectedEmailList = new();

                foreach (FriendEmailTestWrapper email in expectedEmails)
                {
                    expectedEmailList.Add(email);
                }

                actualEmailList
                    .Should()
                    .BeEquivalentTo(expectedEmailList);
            }
        }

        private static void AssertCollectionProperties(
            ChangeTrackingCollection<FriendEmailTestWrapper> emails,
            int addedItemsCount,
            int modifiedItemsCount,
            int removedItemsCount,
            bool isChanged,
            bool isValid)
        {
            emails.AddedItems
                .Should()
                .NotBeNull();
            emails.ModifiedItems
                .Should()
                .NotBeNull();
            emails.RemovedItems
                .Should()
                .NotBeNull();

            if (addedItemsCount > 0)
            {
                emails.AddedItems
                    .Should()
                    .HaveCount(addedItemsCount);
            }
            else
            {
                emails.AddedItems
                    .Should()
                    .BeEmpty();
            }

            if (modifiedItemsCount > 0)
            {
                emails.ModifiedItems
                    .Should()
                    .HaveCount(modifiedItemsCount);
            }
            else
            {
                emails.ModifiedItems
                    .Should()
                    .BeEmpty();
            }

            if (removedItemsCount > 0)
            {
                emails.RemovedItems
                    .Should()
                    .HaveCount(removedItemsCount);
            }
            else
            {
                emails.RemovedItems
                    .Should()
                    .BeEmpty();
            }

            emails.IsChanged
                .Should()
                .Be(isChanged);
            emails.IsValid
                .Should()
                .Be(isValid);
        }

        private static void AssertParentWrapperProperties(FriendTestWrapper testWrapper, bool isChanged, bool isValid)
        {
            testWrapper.IsChanged
                .Should()
                .Be(isChanged);
            testWrapper.IsValid
                .Should()
                .Be(isValid);
        }

        private static void ResetTestEmail(FriendEmail friendEmail, int id, string email, string comment)
        {
            friendEmail.Id = id;
            friendEmail.Email = email;
            friendEmail.Comment = comment;
        }

        private FriendTestWrapper GetFriendTestWrapper(bool isDeveloper, params FriendEmail[] emails)
        {
            Friend friend = new()
            {
                Id = 1000,
                FirstName = "Bob",
                LastName = "Dillon",
                Birthday = new DateTime(1953, 3, 24),
                IsDeveloper = isDeveloper,
                Address = _address,
                Emails = emails.ToList()
            };

            return new FriendTestWrapper(friend);
        }

        private void ResetTestEmails()
        {
            ResetTestEmail(_invalidEmail1, 100, "john.doe", "Invalid email #1");
            ResetTestEmail(_invalidEmail2, 101, "jane.doe@domain", "");
            ResetTestEmail(_validEmail1, 1, "bob@email.com", "Bob's primary email address");
            ResetTestEmail(_validEmail2, 2, "bob@gmail.com", "Bob's alternate email address");
            ResetTestEmail(_validEmail3, 3, "bob.dillon@bigco.com", "Bob's work email address");
        }
    }
}