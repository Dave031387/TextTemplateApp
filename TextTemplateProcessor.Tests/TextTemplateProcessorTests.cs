namespace TemplateProcessor
{
    using static Globals;
    using static TemplateProcessor.Messages.Messages;

    public class TextTemplateProcessorTests
    {
        private static readonly Mock<ITextReader> _mockTextReader = new();
        private static readonly Mock<ITextWriter> _mockTextWriter = new();

        private readonly TestHelper _testHelper = new()
        {
            ResetIndentProcessor = true,
            ResetLocater = true,
            ResetLogger = true,
            ResetNameGenerator = true,
            ResetTokenProcessor = true
        };

        public TextTemplateProcessorTests() => _testHelper.Reset();

        [Theory]
        [InlineData("@-2", 0)]
        [InlineData("O-2", 1)]
        public void GenerateSegment_CalculatedIndentIsNegative_LineIndentIsZero(string code, int finalIndent)
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@+1 Line 1",
                $"{code} Line 2",
                "    Line 3"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string pad1 = new(' ', processor.TabSize);
            string pad3 = finalIndent < 1 ? string.Empty : new(' ', processor.TabSize);
            string expectedMsg = string.Format(MsgLeftIndentHasBeenTruncated, segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 2, expectedMsg)
            };
            int expectedIndent = processor.TabSize * finalIndent;
            string[] expectedText = new[]
            {
                $"{pad1}Line 1",
                "Line 2",
                $"{pad3}Line 3"
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertGeneratedText(processor, expectedText);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expectedIndent);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_RequiredTokenNotInTokenDictionary_LogsAnErrorAndUsesTheEmptyValue()
        {
            // Arrange
            string segmentName = "Segment1";
            string tokenName1 = "Token1";
            string tokenValue1 = "Value1";
            string tokenName2 = "Token2";
            string tokenName3 = "Token3";
            string tokenValue3 = "Value3";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"    Line 1 {Token(tokenName1)}",
                $"    {Token(tokenName2)} {Token(tokenName3)}"
            };
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, tokenValue1 },
                { tokenName3, tokenValue3 }
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgTokenValueIsEmpty, segmentName, tokenName2);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 2, expectedMsg)
            };
            string[] expectedText = new[]
            {
                $"Line 1 {tokenValue1}",
                $" {tokenValue3}"
            };

            // Act
            processor.GenerateSegment(segmentName, tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_SegmentHasFirstTimeIndent_IndentIsAppliedOnlyOnFirstTime()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName} FTI=1",
                "@-1 Line 1",
                "@+1 Line 2"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string pad = new(' ', processor.TabSize);
            int expectedIndent = 2 * processor.TabSize;
            string[] expectedText = new[]
            {
                $"{pad}Line 1",
                $"{pad}{pad}Line 2",
                $"{pad}Line 1",
                $"{pad}{pad}Line 2",
                $"{pad}Line 1",
                $"{pad}{pad}Line 2"
            };

            // Act
            processor.GenerateSegment(segmentName);
            processor.GenerateSegment(segmentName);
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(3);
            AssertGeneratedText(processor, expectedText);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expectedIndent);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_SegmentHasNoTextLines_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "### Segment2",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgSegmentHasNoTextLines, segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertGeneratedText(processor, null);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_SegmentHasPadSegment_PadIsAppliedOnlyAfterFirstTime()
        {
            // Arrange
            string padSegment = "PadSegment";
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {padSegment}",
                "    PAD",
                $"### {segmentName} PAD={padSegment}",
                "    Line 1",
                "    Line 2"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string[] expectedText = new[]
            {
                "Line 1",
                "Line 2",
                "PAD",
                "Line 1",
                "Line 2",
                "PAD",
                "Line 1",
                "Line 2"
            };

            // Act
            processor.GenerateSegment(segmentName);
            processor.GenerateSegment(segmentName);
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(5);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_TemplateNotLoaded_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "    Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, false);
            string expectedMsg = string.Format(MsgAttemptToGenerateSegmentBeforeItWasLoaded, segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertGeneratedText(processor, null);
            AssertFlags(processor, false, false);
            processor.ControlDictionary
                .Should()
                .BeEmpty();
            processor.SegmentDictionary
                .Should()
                .BeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Theory]
        [InlineData("@+2", 4, 4)]
        [InlineData("O+2", 4, 2)]
        [InlineData("@-1", 1, 1)]
        [InlineData("O-1", 1, 2)]
        [InlineData("@=0", 0, 0)]
        [InlineData("O=0", 0, 2)]
        public void GenerateSegment_TextHasIndentCodes_GeneratesIndentedTextLines(string code, int indent2, int indent3)
        {
            // Arrange
            string segmentName = "Segment1";
            int indent1 = 2;
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"@+{indent1} Line 1",
                $"{code} Line 2",
                "    Line 3"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string pad1 = new(' ', indent1 * processor.TabSize);
            string pad2 = new(' ', indent2 * processor.TabSize);
            string pad3 = new(' ', indent3 * processor.TabSize);
            string[] expectedText = new[]
            {
                $"{pad1}Line 1",
                $"{pad2}Line 2",
                $"{pad3}Line 3"
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(1);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_TokenDictionaryContainsEmptyValue_LogsAnErrorAndUsesTheValue()
        {
            // Arrange
            string segmentName = "Segment1";
            string tokenName1 = "Token1";
            string tokenValue1 = "Value1";
            string tokenName2 = "Token2";
            string tokenValue2 = "";
            string tokenName3 = "Token3";
            string tokenValue3 = "Value3";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"    Line 1 {Token(tokenName1)}",
                $"    {Token(tokenName2)} {Token(tokenName3)}"
            };
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, tokenValue1 },
                { tokenName2, tokenValue2 },
                { tokenName3, tokenValue3 }
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgTokenWithEmptyValue, segmentName, tokenName2);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };
            string[] expectedText = new[]
            {
                $"Line 1 {tokenValue1}",
                $"{tokenValue2} {tokenValue3}"
            };

            // Act
            processor.GenerateSegment(segmentName, tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(3, expectedLogEntries);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_TokenDictionaryContainsInvalidName_LogsAnErrorAndIgnoresTheToken()
        {
            // Arrange
            string segmentName = "Segment1";
            string tokenName1 = "Token1";
            string tokenValue1 = "Value1";
            string tokenName2 = "Token2";
            string tokenValue2 = "Value2";
            string tokenName3 = "_Token3";
            string tokenValue3 = "Value3";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"    Line 1 {Token(tokenName1)}",
                $"    {Token(tokenName2)} Line 2"
            };
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, tokenValue1 },
                { tokenName2, tokenValue2 },
                { tokenName3, tokenValue3 }
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgTokenDictionaryContainsInvalidTokenName, segmentName, tokenName3);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };
            string[] expectedText = new[]
            {
                $"Line 1 {tokenValue1}",
                $"{tokenValue2} Line 2"
            };

            // Act
            processor.GenerateSegment(segmentName, tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_TokenDictionaryContainsUnknownToken_LogsAnErrorAndIgnoresTheToken()
        {
            // Arrange
            string segmentName = "Segment1";
            string tokenName1 = "Token1";
            string tokenValue1 = "Value1";
            string tokenName2 = "Token2";
            string tokenValue2 = "Value2";
            string tokenName3 = "Token3";
            string tokenValue3 = "Value3";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"    Line 1 {Token(tokenName1)}",
                $"    {Token(tokenName2)} Line 2"
            };
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, tokenValue1 },
                { tokenName2, tokenValue2 },
                { tokenName3, tokenValue3 }
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgUnknownTokenName, segmentName, tokenName3);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };
            string[] expectedText = new[]
            {
                $"Line 1 {tokenValue1}",
                $"{tokenValue2} Line 2"
            };

            // Act
            processor.GenerateSegment(segmentName, tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_UnknownSegmentName_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment2";
            string[] textLines = new[]
            {
                "### Segment1",
                "@=0 Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgUnknownSegmentName, segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, segmentName, 0, expectedMsg)
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertGeneratedText(processor, null);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_ValidSegment_GeneratesTextLines()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@=0 Line 1",
                "    Line 2"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string[] expectedText = new[]
            {
                "Line 1",
                "Line 2"
            };

            // Act
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(1);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void GenerateSegment_ValidTokenDictionary_ReplacesTokensWithTokenValues()
        {
            // Arrange
            string segmentName = "Segment1";
            string tokenName1 = "Token1";
            string tokenValue1 = "Value1";
            string tokenName2 = "Token2";
            string tokenValue2 = "Value2";
            string tokenName3 = "Token3";
            string tokenValue3 = "Value3";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                $"    Line 1 {Token(tokenName1)}",
                $"    {Token(tokenName2)} {Token(tokenName3)}"
            };
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, tokenValue1 },
                { tokenName2, tokenValue2 },
                { tokenName3, tokenValue3 }
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string[] expectedText = new[]
            {
                $"Line 1 {tokenValue1}",
                $"{tokenValue2} {tokenValue3}"
            };

            // Act
            processor.GenerateSegment(segmentName, tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(1);
            AssertGeneratedText(processor, expectedText);
            TokenProcessor._tokenDictionary[tokenName1]
                .Should()
                .Be(tokenValue1);
            TokenProcessor._tokenDictionary[tokenName2]
                .Should()
                .Be(tokenValue2);
            TokenProcessor._tokenDictionary[tokenName3]
                .Should()
                .Be(tokenValue3);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_AttemptToLoadTwice_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@=0 Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);
            string ExpectedMsg = string.Format(MsgAttemptToLoadMoreThanOnce, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, ExpectedMsg)
            };

            // Act
            processor.LoadTemplate();

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertFlags(processor, true, false);
            processor.GeneratedText
                .Should()
                .NotBeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_EmptyFile_LogsAnError()
        {
            // Arrange
            string[] textLines = new[]
            {
                ""
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, false);
            string expectedMsg = MsgTemplateFileIsEmpty;
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate();

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertFlags(processor, false, false);
            processor.ControlDictionary
                .Should()
                .BeEmpty();
            processor.SegmentDictionary
                .Should()
                .BeEmpty();
            VerifyMocks(Times.Once(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_FirstFileHasBeenWritten_LoadsNextTemplate()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@=0 Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, true, true);
            processor.GenerateSegment(segmentName);
            processor.WriteGeneratedTextToFile(OutputFilePathString);
            ConsoleLogger.Clear();
            SetupMockTextReader(textLines, 1, 3);
            SetupMockTextWriter(true);
            string expectedMsg = string.Format(MsgLoadingTemplateFile, TemplateFileNameString2);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate(TemplateFilePathString2);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertFlags(processor, true, false);
            processor.GeneratedText
                .Should()
                .BeEmpty();
            processor.ControlDictionary
                .Should()
                .ContainSingle();
            processor.SegmentDictionary
                .Should()
                .ContainSingle();
            VerifyMocks(Times.Once(), TemplateFilePathString2, Times.Once(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_FirstFileNotWrittenYet_LogsAnErrorAndLoadsTheTemplate()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@=0 Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, 1, 4);
            processor.GenerateSegment(segmentName);
            string expectedMsg = string.Format(MsgNextLoadRequestBeforeFirstIsWritten, TemplateFileNameString2, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate(TemplateFilePathString2);

            // Assert
            ConsoleLogValidater.AssertLogContents(4, expectedLogEntries);
            AssertFlags(processor, true, false);
            processor.GeneratedText
                .Should()
                .BeEmpty();
            processor.ControlDictionary
                .Should()
                .ContainSingle();
            processor.SegmentDictionary
                .Should()
                .ContainSingle();
            VerifyMocks(Times.Once(), TemplateFilePathString2, Times.Once(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_ReuseSameFilePath_LogsAnError()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "@=0 Line1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);
            string expectedMsg = string.Format(MsgAttemptToLoadMoreThanOnce, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate(TemplateFilePathString1);

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertFlags(processor, true, false);
            processor.GeneratedText
                .Should()
                .NotBeEmpty();
            processor.ControlDictionary
                .Should()
                .ContainSingle();
            processor.SegmentDictionary
                .Should()
                .ContainSingle();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_ValidPathNotSet_LogsAnError()
        {
            // Arrange
            Mock<ITextReader> mockTextReader = new();
            Mock<ITextWriter> mockTextWriter = new();
            TextTemplateProcessor processor = new(mockTextReader.Object, mockTextWriter.Object);
            SetupMockTextReader(Array.Empty<string>(), true);
            string expectedMsg = MsgUnableToLoadTemplate;
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate();

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertFlags(processor, false, false);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void LoadTemplate_ValidTemplateFile_ParsesAndLoadsTheTemplate()
        {
            // Arrange
            string segmentName1 = "Segment1";
            string segmentName2 = "Segment2";
            string line1 = "Line 1";
            string line2 = "Line 2";
            string line3 = "Line 3";
            string[] textLines = new[]
            {
                $"### {segmentName1}",
                $"O=1 {line1}",
                $"### {segmentName2} FTI=1, PAD={segmentName1}",
                $"@+1 {line2}",
                $"    {line3}"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, false);
            ControlItem expectedControlItem1 = new()
            {
                FirstTimeIndent = 0,
                IsFirstTime = true,
                PadSegment = null
            };
            ControlItem expectedControlItem2 = new()
            {
                FirstTimeIndent = 1,
                IsFirstTime = true,
                PadSegment = segmentName1
            };
            TextItem expectedTextItem1 = new(1, false, true, line1);
            TextItem expectedTextItem2 = new(1, true, false, line2);
            TextItem expectedTextItem3 = new(0, true, false, line3);
            List<TextItem> expectedList1 = new()
            {
                expectedTextItem1
            };
            List<TextItem> expectedList2 = new()
            {
                expectedTextItem2,
                expectedTextItem3
            };
            string expectedMsg = string.Format(MsgLoadingTemplateFile, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.LoadTemplate();

            // Assert
            AssertFlags(processor, true, false);
            processor.ControlDictionary
                .Should()
                .HaveCount(2);
            processor.SegmentDictionary
                .Should()
                .HaveCount(2);
            processor.ControlDictionary
                .Should()
                .ContainKeys(segmentName1, segmentName2);
            processor.SegmentDictionary
                .Should()
                .ContainKeys(segmentName1, segmentName2);
            processor.ControlDictionary[segmentName1]
                .Should()
                .BeEquivalentTo(expectedControlItem1);
            processor.ControlDictionary[segmentName2]
                .Should()
                .BeEquivalentTo(expectedControlItem2);
            processor.SegmentDictionary[segmentName1]
                .Should()
                .BeEquivalentTo(expectedList1);
            processor.SegmentDictionary[segmentName2]
                .Should()
                .BeEquivalentTo(expectedList2);
            ConsoleLogValidater.AssertLogContents(3, expectedLogEntries);
            VerifyMocks(Times.Once(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void ResetAll_WhenInvoked_ResetsTemplateEnvironment()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                "    Line 1",
                $"### {segmentName}",
                "@=0 Line 1",
                "    Line 2"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);
            string expectedMsg = string.Format(MsgTemplateHasBeenReset, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Reset, string.Empty, 0, expectedMsg)
            };

            // Act
            processor.ResetAll();

            // Assert
            ConsoleLogValidater.AssertLogContents(2, expectedLogEntries);
            AssertFlags(processor, false, false);
            AssertResetGeneratedText(processor);
            processor.SegmentDictionary
                .Should()
                .BeEmpty();
            processor.ControlDictionary
                .Should()
                .BeEmpty();
            DefaultSegmentNameGenerator.Next
                .Should()
                .Be(DefaultSegmentName1);
            TokenProcessor._tokenDictionary
                .Should()
                .BeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void ResetGeneratedText_WhenInvoked_ResetsGeneratedText()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                "    Line 1",
                $"### {segmentName}",
                "@=0 Line 1 <#=Token1#>",
                "    Line 2"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);
            string expectedMsg = string.Format(MsgGeneratedTextHasBeenReset, TemplateFileNameString1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Reset, string.Empty, 0, expectedMsg)
            };
            ConsoleLogger.Clear();

            // Act
            processor.ResetGeneratedText();

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertFlags(processor, true, false);
            AssertResetGeneratedText(processor);
            processor.SegmentDictionary
                .Should()
                .NotBeEmpty();
            processor.ControlDictionary
                .Should()
                .NotBeEmpty();
            DefaultSegmentNameGenerator.Next
                .Should()
                .Be(DefaultSegmentName2);
            TokenProcessor._tokenDictionary
                .Should()
                .NotBeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Theory]
        [InlineData("Segment2")]
        [InlineData("")]
        [InlineData(null)]
        public void ResetSegment_InvalidSegment_LogsAnError(string segmentName)
        {
            // Arrange
            string padSegment = "PadSegment";
            string[] textLines = new[]
            {
                $"### {padSegment}",
                "    ",
                $"### Segment1 FTI=1, PAD={padSegment}",
                "    Line 1"
            };
            string badSegment = segmentName is null ? string.Empty : segmentName;
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            string expectedMsg = string.Format(MsgUnableToResetSegment, segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, badSegment, 0, expectedMsg)
            };

            // Act
            processor.ResetSegment(segmentName!);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void ResetSegment_ValidSegment_ResetsTheFirstTimeFlag()
        {
            // Arrange
            string padSegment = "PadSegment";
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {padSegment}",
                "    PAD",
                $"### {segmentName} FTI=1, PAD={padSegment}",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);
            processor.GenerateSegment(segmentName);
            string pad = new(' ', processor.TabSize);
            string[] expectedText = new[]
            {
                $"{pad}Line 1",
                $"{pad}PAD",
                $"{pad}Line 1",
                $"{pad}{pad}Line 1"
            };

            // Act
            processor.ResetSegment(segmentName);
            processor.GenerateSegment(segmentName);

            // Assert
            ConsoleLogValidater.AssertLogContents(4);
            AssertGeneratedText(processor, expectedText);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(5, 5)]
        [InlineData(9, 9)]
        [InlineData(10, 9)]
        [InlineData(11, 9)]
        public void SetTabSize_IntegerValue_SetsTabSizeAndConstrainsItToValidRange(int tabSize, int expectedTabSize)
        {
            // Arrange
            string[] textLines = new[]
            {
                "### Segment1",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);

            // Act
            processor.SetTabSize(tabSize);

            // Assert
            processor.TabSize
                .Should()
                .Be(expectedTabSize);
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void TextTemplateProcessorConstructor_ValidFilePath_InitializesProperties()
        {
            // Arrange
            string[] textLines = new[]
            {
                ""
            };
            Mock<ITextReader> mockTextReader = new();
            Mock<ITextWriter> mockTextWriter = new();
            SetupMockTextReader(textLines);

            // Act
            TextTemplateProcessor processor = new(mockTextReader.Object, mockTextWriter.Object);

            // Assert
            AssertFlags(processor, false, false);
            processor.CurrentIndent
                .Should()
                .Be(0);
            processor.TabSize
                .Should()
                .Be(4);
            ConsoleLogger.LogEntries
                .Should()
                .NotBeNull();
            ConsoleLogValidater.AssertLogContents(0);
            processor.ControlDictionary
                .Should()
                .BeEmpty();
            processor.SegmentDictionary
                .Should()
                .BeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), It.IsAny<string>(), Times.Never());
        }

        [Fact]
        public void WriteGeneratedTextToFile_ProblemWithOutputFile_DoesNothing()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines);
            processor.GenerateSegment(segmentName);

            // Act
            processor.WriteGeneratedTextToFile(OutputFilePathString);

            // Assert
            AssertFlags(processor, true, false);
            processor.GeneratedText
                .Should()
                .NotBeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), OutputFilePathString, Times.Once());
        }

        [Fact]
        public void WriteGeneratedTextToFile_ValidOutputFileAndNoReset_DoesNotResetGeneratedText()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, true, true);
            processor.GenerateSegment(segmentName);

            // Act
            processor.WriteGeneratedTextToFile(OutputFilePathString, false);

            // Assert
            AssertFlags(processor, true, true);
            processor.GeneratedText
                .Should()
                .NotBeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), OutputFilePathString, Times.Once());
        }

        [Fact]
        public void WriteGeneratedTextToFile_ValidOutputFileWithReset_ResetsGeneratedText()
        {
            // Arrange
            string segmentName = "Segment1";
            string[] textLines = new[]
            {
                $"### {segmentName}",
                "    Line 1"
            };
            TextTemplateProcessor processor = SetupTextTemplateProcessor(textLines, true, true);
            processor.GenerateSegment(segmentName);

            // Act
            processor.WriteGeneratedTextToFile(OutputFilePathString);

            // Assert
            AssertFlags(processor, true, true);
            processor.GeneratedText
                .Should()
                .BeEmpty();
            VerifyMocks(Times.Never(), It.IsAny<string>(), Times.Never(), OutputFilePathString, Times.Once());
        }

        private static void AssertFlags(TextTemplateProcessor processor, bool isTemplateLoaded, bool isOutputFileWritten)
        {
            processor.IsTemplateLoaded
                .Should()
                .Be(isTemplateLoaded);
            processor.IsOutputFileWritten
                .Should()
                .Be(isOutputFileWritten);
        }

        private static void AssertGeneratedText(
            TextTemplateProcessor processor,
            string[]? expectedText)
        {
            if (expectedText is null)
            {
                processor.GeneratedText
                    .Should()
                    .BeEmpty();
            }
            else
            {
                int expectedLineCount = expectedText.Length;

                processor.GeneratedText
                    .Should()
                    .HaveCount(expectedLineCount);
                processor.GeneratedText
                    .Should()
                    .BeEquivalentTo(expectedText);
            }
        }

        private static void AssertResetGeneratedText(TextTemplateProcessor processor)
        {
            processor.GeneratedText
                .Should()
                .BeEmpty();
            Locater.CurrentSegment
                .Should()
                .BeEmpty();
            Locater.LineNumber
                .Should()
                .Be(0);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(0);
        }

        private static void SetupFileNameProperty(int firstCount, int secondCount)
        {
            int totalCount = firstCount + secondCount;
            int currentCount = 0;
            Moq.Language.ISetupSequentialResult<string> setup = _mockTextReader.SetupSequence(textReader => textReader.FileName);

            while (currentCount < totalCount)
            {
                setup = setup.Returns(currentCount < firstCount ? TemplateFileNameString1 : TemplateFileNameString2);
                currentCount++;
            }
        }

        private static void SetupMockTextReader(string[] textLines, bool isEmptyFilePath = false)
        {
            _mockTextReader.Reset();
            _mockTextReader.Setup(textReader => textReader.ReadTextFile()).Returns(textLines);
            _mockTextReader.Setup(textReader => textReader.SetFilePath(It.IsAny<string>()));

            if (isEmptyFilePath)
            {
                _mockTextReader.Setup(textReader => textReader.FileName).Returns(string.Empty);
                _mockTextReader.Setup(textReader => textReader.FullFilePath).Returns(string.Empty);
            }
            else
            {
                _mockTextReader.Setup(textReader => textReader.FileName).Returns(TemplateFileNameString1);
                _mockTextReader.Setup(textReader => textReader.FullFilePath).Returns(TemplateFilePathString1);
            }
        }

        private static void SetupMockTextReader(string[] textLines, int firstCount, int secondCount)
        {
            _mockTextReader.Reset();
            Moq.Language.ISetupSequentialResult<string> setup = _mockTextReader.SetupSequence(textReader => textReader.FullFilePath);
            _mockTextReader.Setup(textReader => textReader.ReadTextFile()).Returns(textLines);
            SetupFileNameProperty(firstCount, secondCount);
            setup = setup.Returns(TemplateFilePathString1);
            setup = setup.Returns(TemplateFilePathString1);
            setup = setup.Returns(TemplateFilePathString2);
            setup = setup.Returns(TemplateFilePathString2);
            setup = setup.Returns(TemplateFilePathString2);
            setup = setup.Returns(TemplateFilePathString2);
            //_mockTextReader.Setup(textReader => textReader.FullFilePath).Returns(TemplateFilePathString1);
            _mockTextReader.Setup(textReader => textReader.SetFilePath(It.IsAny<string>()));
        }

        private static void SetupMockTextWriter(bool textWriterReturn)
        {
            _mockTextWriter.Reset();
            _mockTextWriter.Setup(textWriter => textWriter.CreateOutputFile(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns(textWriterReturn);
        }

        private static TextTemplateProcessor SetupTextTemplateProcessor(string[] textLines, bool loadTemplate = true, bool textWriterReturn = false)
        {
            TextTemplateProcessor processor = new(_mockTextReader.Object, _mockTextWriter.Object);

            SetupMockTextReader(textLines);

            if (loadTemplate)
            {
                processor.LoadTemplate();
                SetupMockTextReader(textLines);
            }

            SetupMockTextWriter(textWriterReturn);
            ConsoleLogger.Clear();
            return processor;
        }

        private static TextTemplateProcessor SetupTextTemplateProcessor(string[] textLines, int firstCount, int secondCount, bool textWriterReturn = false)
        {
            TextTemplateProcessor processor = new(_mockTextReader.Object, _mockTextWriter.Object);

            SetupMockTextReader(textLines);

            processor.LoadTemplate();

            SetupMockTextReader(textLines, firstCount, secondCount);
            SetupMockTextWriter(textWriterReturn);
            ConsoleLogger.Clear();
            return processor;
        }

        private static string Token(string tokenName) => $"<#={tokenName}#>";

        private static void VerifyMocks(Times readTextTimes, string filePathIn, Times setFilePathTimes, string filePathOut, Times createOutputTimes)
        {
            _mockTextReader.Verify(textReader => textReader.ReadTextFile(), readTextTimes);
            _mockTextReader.Verify(textReader => textReader.SetFilePath(filePathIn), setFilePathTimes);
            _mockTextWriter.Verify(textWriter => textWriter.CreateOutputFile(filePathOut, It.IsAny<IEnumerable<string>>()), createOutputTimes);
        }
    }
}