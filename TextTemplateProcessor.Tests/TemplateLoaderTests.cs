namespace TemplateProcessor
{
    using System.Collections.Generic;
    using static TemplateProcessor.Messages.Messages;
    using TextReader = IO.TextReader;
    using TextWriter = IO.TextWriter;

    public class TemplateLoaderTests
    {
        private const string DefaultSegment = "DefaultSegment1";

        private readonly TestHelper _testHelper = new()
        {
            ResetLocater = true,
            ResetLogger = true,
            ResetNameGenerator = true,
            ResetTokenProcessor = true
        };

        private readonly TextTemplateProcessor _textTemplateProcessor = new(new TextReader(), new TextWriter());

        public TemplateLoaderTests()
        {
            _testHelper.Reset();
            _textTemplateProcessor.ControlDictionary.Clear();
            _textTemplateProcessor.SegmentDictionary.Clear();
        }

        [Fact]
        public void LoadTemplate_DuplicateSegmentNames_LogsAnErrorAndUsesDefaultSegmentName()
        {
            // Arrange
            string segmentName = "Segment1";
            List<string> lines = new()
            {
                $"### {segmentName}",
                "    Line 1",
                $"### {segmentName}",
                "    Line 2"
            };
            string expectedMsg = string.Format(MsgFoundDuplicateSegmentName, segmentName, DefaultSegment);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, DefaultSegment, 3, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .HaveCount(2);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(segmentName);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(DefaultSegment);
        }

        [Fact]
        public void LoadTemplate_FirstLineHasInvalidPrefix_LogsAnErrorAndGeneratesDefaultSegment()
        {
            // Arrange
            List<string> lines = new()
            {
                "##  Error",
                "    Line 1"
            };
            string expectedMsg = string.Format(MsgMissingInitialSegmentHeader, DefaultSegment);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, DefaultSegment, 1, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainSingle();
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(DefaultSegment);
        }

        [Fact]
        public void LoadTemplate_FirstLineIsNotSegmentHeader_LogsAnErrorAndGeneratesDefaultSegment()
        {
            // Arrange
            List<string> lines = new()
            {
                "    Line 1"
            };
            string expectedMsg = string.Format(MsgMissingInitialSegmentHeader, DefaultSegment);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, DefaultSegment, 1, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainSingle();
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(DefaultSegment);
        }

        [Fact]
        public void LoadTemplate_LastLineIsSegmentHeader_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment2";
            List<string> lines = new()
            {
                "### Segment1",
                "    Line 1",
                $"### {segmentName}"
            };
            string expectedMsg = string.Format(MsgNoTextLinesFollowingSegmentHeader, segmentName);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, segmentName, 3, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }

        [Fact]
        public void LoadTemplate_LineContainsInvalidPrefix_ItemIsNotAddedToControlOrSegmentDictionary()
        {
            // Arrange
            string segmentName = "Segment1";
            string expectedLine = "Good Line";
            List<string> lines = new()
            {
                $"### {segmentName}",
                "the Bad Line",
                $"    {expectedLine}"
            };
            TemplateLoader templateLoader = new(_textTemplateProcessor);

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainSingle();
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(segmentName);
            _textTemplateProcessor.SegmentDictionary
                .Should()
                .ContainSingle();
            _textTemplateProcessor.SegmentDictionary
                .Should()
                .ContainKey(segmentName);
            _textTemplateProcessor.SegmentDictionary[segmentName]
                .Should()
                .HaveCount(1);
            _textTemplateProcessor.SegmentDictionary[segmentName][0].Text
                .Should()
                .Be(expectedLine);
        }

        [Fact]
        public void LoadTemplate_LinesContainValidTokens_AllTokensAreAddedToTokenDictionary()
        {
            // Arrange
            string segmentName = "Segment1";
            string token1 = "Token1";
            string token2 = "Token2";
            string token3 = "Token3";
            List<string> lines = new()
            {
                $"### {segmentName}",
                $"    Line 1 <#={token1}#>",
                "    Line 2",
                $"    <#={token2}#> and <#={token3}#>.",
                $"    And <#={token3}#><#={token1}#> again."
            };
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            string expectedMsg = string.Format(MsgSegmentHasBeenAdded);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, segmentName, 1, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            TokenProcessor._tokenDictionary
                .Should()
                .HaveCount(3);
            TokenProcessor._tokenDictionary
                .Should()
                .ContainKeys(token1, token2, token3);
        }

        [Fact]
        public void LoadTemplate_NoTextLinesBetweenSegmentHeaders_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment1";
            List<string> lines = new()
            {
                $"### {segmentName}",
                "### Segment2",
                "    Line 1"
            };
            string expectedMsg = string.Format(MsgNoTextLinesFollowingSegmentHeader, segmentName);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, segmentName, 2, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }

        [Fact]
        public void LoadTemplate_PadSegmentReferencedBeforeBeingDefined_LogsAnErrorAndIgnoresThePadSegment()
        {
            // Arrange
            string segmentName = "Segment1";
            string padSegment = "Segment2";
            List<string> lines = new()
            {
                "### FirstSegment",
                "    Line 1",
                $"### {segmentName} PAD={padSegment}",
                "    Line 2"
            };
            string expectedMsg = string.Format(MsgPadSegmentsMustBeDefinedEarlier, segmentName, padSegment);
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, segmentName, 3, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(segmentName);
            _textTemplateProcessor.ControlDictionary[segmentName].PadSegment
                .Should()
                .BeNull();
        }

        [Fact]
        public void LoadTemplate_SegmentHasValidTextLines_AllLinesAreAddedToSegmentDictionaryInCorrectOrder()
        {
            // Arrange
            string segmentName = "Segment1";
            string line1 = "Line 1";
            string line2 = "Line 2";
            string line3 = "Line 3";
            List<string> lines = new()
            {
                $"### {segmentName}",
                $"    {line1}",
                $"    {line2}",
                $"    {line3}"
            };
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            string expectedMsg = string.Format(MsgSegmentHasBeenAdded);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, segmentName, 1, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
            _textTemplateProcessor.SegmentDictionary[segmentName]
                .Should()
                .HaveCount(3);
            _textTemplateProcessor.SegmentDictionary[segmentName][0].Text
                .Should()
                .Be(line1);
            _textTemplateProcessor.SegmentDictionary[segmentName][1].Text
                .Should()
                .Be(line2);
            _textTemplateProcessor.SegmentDictionary[segmentName][2].Text
                .Should()
                .Be(line3);
        }

        [Theory]
        [InlineData("", 0, null)]
        [InlineData("FTI=1", 1, null)]
        [InlineData("PAD=PadSegment", 0, "PadSegment")]
        [InlineData("FTI=2, PAD=PadSegment", 2, "PadSegment")]
        [InlineData("PAD=PadSegment, FTI=3", 3, "PadSegment")]
        public void LoadTemplate_ValidSegmentHeader_AddsControlItemToDictionary(string options, int firstTimeIndent, string padSegment)
        {
            // Arrange
            string segmentName = "Segment1";
            string padSegmentName = "PadSegment";
            List<string> lines = new()
            {
                $"### {padSegmentName}",
                "   ",
                $"### {segmentName} {options}",
                "    Line 1"
            };
            TemplateLoader templateLoader = new(_textTemplateProcessor);
            string expectedMsg = string.Format(MsgSegmentHasBeenAdded);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, padSegmentName, 1, expectedMsg),
                new(LogEntryType.Parsing, segmentName, 3, expectedMsg)
            };

            // Act
            templateLoader.LoadTemplate(lines);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            _textTemplateProcessor.ControlDictionary
                .Should()
                .ContainKey(segmentName);
            _textTemplateProcessor.ControlDictionary[segmentName].FirstTimeIndent
                .Should()
                .Be(firstTimeIndent);
            _textTemplateProcessor.ControlDictionary[segmentName].PadSegment
                .Should()
                .Be(padSegment);
        }
    }
}