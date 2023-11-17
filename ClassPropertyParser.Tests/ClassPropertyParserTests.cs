namespace ClassPropertyParser
{
    using System.Reflection;

    public class ClassPropertyParserTests
    {
        [Fact]
        public void ClassPropertyParser_DefaultConstructor_InitializesEmptyLists()
        {
            // Arrange/Act
            ClassPropertyParser parser = new();

            // Assert
            parser.ClassTypes
                .Should()
                .NotBeNull();
            parser.ClassTypes
                .Should()
                .BeEmpty();
            parser.CollectionProperties
                .Should()
                .NotBeNull();
            parser.CollectionProperties
                .Should()
                .BeEmpty();
            parser.ComplexProperties
                .Should()
                .NotBeNull();
            parser.ComplexProperties
                .Should()
                .BeEmpty();
            parser.SimpleProperties
                .Should()
                .NotBeNull();
            parser.SimpleProperties
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void ClassPropertyParser_NullClassType_InitializesEmptyLists()
        {
            // Arrange/Act
            ClassPropertyParser parser = new(null!);

            // Assert
            parser.ClassTypes
                .Should()
                .NotBeNull();
            parser.ClassTypes
                .Should()
                .BeEmpty();
            parser.CollectionProperties
                .Should()
                .NotBeNull();
            parser.CollectionProperties
                .Should()
                .BeEmpty();
            parser.ComplexProperties
                .Should()
                .NotBeNull();
            parser.ComplexProperties
                .Should()
                .BeEmpty();
            parser.SimpleProperties
                .Should()
                .NotBeNull();
            parser.SimpleProperties
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void ClassPropertyParser_ValidClassType_GetsAllClassTypesInSameAssembly()
        {
            // Arrange
            List<Type> expectedTypes = new()
            {
                typeof(TestModels.Address),
                typeof(TestModels.Friend),
                typeof(TestModels.FriendEmail),
                typeof(TestModels.FriendGroup)
            };

            // Act
            ClassPropertyParser parser = new(typeof(TestModels.Friend));

            // Assert
            parser.ClassTypes
                .Should()
                .NotBeNull();
            parser.ClassTypes
                .Should()
                .HaveCount(expectedTypes.Count);
            parser.ClassTypes
                .Should()
                .Contain(expectedTypes);
            parser.CollectionProperties
                .Should()
                .NotBeNull();
            parser.CollectionProperties
                .Should()
                .BeEmpty();
            parser.ComplexProperties
                .Should()
                .NotBeNull();
            parser.ComplexProperties
                .Should()
                .BeEmpty();
            parser.SimpleProperties
                .Should()
                .NotBeNull();
            parser.SimpleProperties
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetAllProperties_NullClassType_ClearsAllPropertyLists()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            parser.GetAllProperties(typeof(TestModels.Friend));

            // Act
            parser.GetAllProperties(null!);

            // Assert
            parser.CollectionProperties
                .Should()
                .BeEmpty();
            parser.ComplexProperties
                .Should()
                .BeEmpty();
            parser.SimpleProperties
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetAllProperties_ValidClassType_GetsAllPropertiesForClassType()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            List<string> expectedCollectionProperties = new()
            {
                nameof(TestModels.Friend.Emails)
            };
            List<string> expectedComplexProperties = new()
            {
                nameof(TestModels.Friend.Address)
            };
            List<string> expectedSimpleProperties = new()
            {
                nameof(TestModels.Friend.Birthday),
                nameof(TestModels.Friend.FirstName),
                nameof(TestModels.Friend.FriendGroupId),
                nameof(TestModels.Friend.Id),
                nameof(TestModels.Friend.IsDeveloper),
                nameof(TestModels.Friend.LastName)
            };

            // Act
            parser.GetAllProperties(typeof(TestModels.Friend));
            List<string> actualCollectionProperties = GetPropertyNames(parser.CollectionProperties);
            List<string> actualComplexProperties = GetPropertyNames(parser.ComplexProperties);
            List<string> actualSimpleProperties = GetPropertyNames(parser.SimpleProperties);

            // Assert
            parser.CollectionProperties
                .Should()
                .HaveCount(expectedCollectionProperties.Count);
            actualCollectionProperties
                .Should()
                .Contain(expectedCollectionProperties);
            parser.ComplexProperties
                .Should()
                .HaveCount(expectedComplexProperties.Count);
            actualComplexProperties
                .Should()
                .Contain(expectedComplexProperties);
            parser.SimpleProperties
                .Should()
                .HaveCount(expectedSimpleProperties.Count);
            actualSimpleProperties
                .Should()
                .Contain(expectedSimpleProperties);
        }

        [Fact]
        public void GetCollectionProperties_NullClassType_ClearsCollectionPropertyList()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            _ = parser.GetCollectionProperties(typeof(TestModels.Friend));

            // Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetCollectionProperties(null!);

            // Assert
            parser.CollectionProperties
                .Should()
                .BeEmpty();
            propertyInfos
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetCollectionProperties_ValidClassType_GetsAllCollectionPropertiesForClassType()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            List<string> expected = new()
            {
                nameof(TestModels.Friend.Emails)
            };

            // Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetCollectionProperties(typeof(TestModels.Friend)).ToList();
            List<string> actual = GetPropertyNames(propertyInfos);

            // Assert
            parser.CollectionProperties
                .Should()
                .HaveCount(expected.Count);
            propertyInfos
                .Should()
                .BeEquivalentTo(parser.CollectionProperties);
            actual
                .Should()
                .Contain(expected);
        }

        [Fact]
        public void GetComplexProperties_NullClassType_ClearsComplexPropertyList()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            _ = parser.GetComplexProperties(typeof(TestModels.Friend));

            // Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetComplexProperties(null!).ToList();

            // Assert
            parser.ComplexProperties
                .Should()
                .BeEmpty();
            propertyInfos
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetComplexProperties_ValidClassType_GetsAllComplexPropertiesForClassType()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            List<string> expected = new()
            {
                nameof(TestModels.Friend.Address)
            };

            // Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetComplexProperties(typeof(TestModels.Friend)).ToList();
            List<string> actual = GetPropertyNames(propertyInfos);

            // Assert
            parser.ComplexProperties
                .Should()
                .HaveCount(expected.Count);
            propertyInfos
                .Should()
                .BeEquivalentTo(parser.ComplexProperties);
            actual
                .Should()
                .Contain(expected);
        }

        [Fact]
        public void GetSimpleProperties_NullClassType_ClearsSimplePropertyList()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            _ = parser.GetSimpleProperties(typeof(TestModels.Friend));

            //Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetSimpleProperties(null!).ToList();

            // Assert
            parser.SimpleProperties
                .Should()
                .BeEmpty();
            propertyInfos
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetSimpleProperties_ValidClassType_GetsAllSimplePropertiesForClassType()
        {
            // Arrange
            ClassPropertyParser parser = new(typeof(TestModels.Friend));
            List<string> expected = new()
            {
                nameof(TestModels.Friend.Birthday),
                nameof(TestModels.Friend.FirstName),
                nameof(TestModels.Friend.FriendGroupId),
                nameof(TestModels.Friend.Id),
                nameof(TestModels.Friend.IsDeveloper),
                nameof(TestModels.Friend.LastName)
            };

            // Act
            IEnumerable<PropertyInfo> propertyInfos = parser.GetSimpleProperties(typeof(TestModels.Friend)).ToList();
            List<string> actual = GetPropertyNames(propertyInfos);

            // Assert
            parser.SimpleProperties
                .Should()
                .HaveCount(expected.Count);
            propertyInfos
                .Should()
                .BeEquivalentTo(parser.SimpleProperties);
            actual
                .Should()
                .Contain(expected);
        }

        [Theory]
        [InlineData(typeof(string), "System.String")]
        [InlineData(typeof(List<string>), "System.Collections.Generic.List<System.String>")]
        [InlineData(typeof(int[]), "System.Int32[]")]
        [InlineData(typeof(Dictionary<int, DateTime>), "System.Collections.Generic.Dictionary<System.Int32,System.DateTime>")]
        public void GetTypeName_ValidType_ReturnsFullTypeName(Type type, string expectedTypeName)
        {
            // Act
            string actualTypeName = ClassPropertyParser.GetTypeName(type);

            // Assert
            actualTypeName
                .Should()
                .Be(expectedTypeName);
        }

        private static List<string> GetPropertyNames(IEnumerable<PropertyInfo> propertyInfos)
                    => propertyInfos.Select(t => t.Name).ToList();
    }
}