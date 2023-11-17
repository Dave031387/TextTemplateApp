// Ignore Spelling: Validater

namespace TemplateProcessor
{
    public class NameValidaterTests
    {
        [Theory]
        [InlineData("123ABC")]
        [InlineData("_Nope")]
        [InlineData("A,bc")]
        [InlineData("A-bc")]
        [InlineData("Abc:")]
        [InlineData("A(BC)")]
        [InlineData("Ab=c")]
        [InlineData("ABC?")]
        [InlineData("A@bc")]
        [InlineData("ab*c")]
        [InlineData("ab$c")]
        [InlineData("a&bc")]
        public void IsValidName_NameIsNotValid_ReturnsFalse(string name)
        {
            // Act
            bool isValid = NameValidater.IsValidName(name);

            // Assert
            isValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValidName_NameIsNull_ReturnsFalse()
        {
            // Act
            bool isValid = NameValidater.IsValidName(null);

            // Assert
            isValid
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("z")]
        [InlineData("Z_")]
        [InlineData("a1")]
        [InlineData("Good_Bye")]
        [InlineData("F150")]
        [InlineData("B_36CR")]
        [InlineData("M__1x")]
        [InlineData("A_b_c_")]
        public void IsValidName_NameIsValid_ReturnsTrue(string name)
        {
            // Act
            bool isValid = NameValidater.IsValidName(name);

            // Assert
            isValid
                .Should()
                .BeTrue();
        }
    }
}