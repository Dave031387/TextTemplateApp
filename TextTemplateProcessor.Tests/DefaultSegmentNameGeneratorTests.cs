namespace TemplateProcessor
{
    using static Globals;

    public class DefaultSegmentNameGeneratorTests
    {
        private readonly TestHelper _testHelper = new()
        {
            ResetNameGenerator = true
        };

        public DefaultSegmentNameGeneratorTests() => _testHelper.Reset();

        [Fact]
        public void Next_FirstCallAfterReset_StartsOverWithFirstValue()
        {
            // Arrange
            _ = DefaultSegmentNameGenerator.Next;
            _ = DefaultSegmentNameGenerator.Next;
            _ = DefaultSegmentNameGenerator.Next;
            DefaultSegmentNameGenerator.Reset();

            // Act
            string actual = DefaultSegmentNameGenerator.Next;

            // Assert
            actual
                .Should()
                .Be(DefaultSegmentName1);
        }

        [Fact]
        public void Next_OnFirstCall_SuppliesCorrectValue()
        {
            // Act
            string actual = DefaultSegmentNameGenerator.Next;

            // Assert
            actual
                .Should()
                .Be(DefaultSegmentName1);
        }

        [Fact]
        public void Next_OnSecondCall_SuppliesCorrectValue()
        {
            // Arrange
            _ = DefaultSegmentNameGenerator.Next;

            // Act
            string actual = DefaultSegmentNameGenerator.Next;

            // Assert
            actual
                .Should()
                .Be(DefaultSegmentName2);
        }
    }
}