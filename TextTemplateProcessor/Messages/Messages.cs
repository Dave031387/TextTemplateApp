﻿namespace TemplateProcessor.Messages
{
    /// <include file="../docs.xml" path="docs/members[@name=&quot;messages&quot;]/Messages/*"/>
    internal static class Messages
    {
        internal const string MsgAttemptingToReadFile = "Attempting to read text template file:\n{0}";
        internal const string MsgAttemptToGenerateSegmentBeforeItWasLoaded = "An attempt was made to generate segment \"{0}\" before the template was loaded.";
        internal const string MsgAttemptToLoadMoreThanOnce = "Attempted to load template file \"{0}\" more than once. Repeat loads will be ignored.";
        internal const string MsgClearTheOutputDirectory = "\nCONFIRM: Do you want to clear the contents of the following directory?\n{0}";
        internal const string MsgContinuationPrompt = "Press [ENTER] to continue...";
        internal const string MsgDirectoryNotFound = "The specified directory was not found. Directory path: ";
        internal const string MsgDirectoryPathIsEmptyOrWhitespace = "The directory path must not be empty or contain only whitespace.";
        internal const string MsgErrorWhileReadingFile = "An error occurred while reading the template file. {0}";
        internal const string MsgFileNotFound = "The specified file was not found. Full file path: ";
        internal const string MsgFilePathIsEmptyOrWhitespace = "The file path must not be empty or contain only whitespace.";
        internal const string MsgFilePathNotSet = "A Valid template file path has not been set.";
        internal const string MsgFileSuccessfullyRead = "The text template file has been successfully read.";
        internal const string MsgFirstTimeIndentHasBeenTruncated = "The calculated first time indent for segment \"{0}\" went negative. It will be set to zero.";
        internal const string MsgFirstTimeIndentSetToZero = "The First Time Indent option value was set to zero. This value disables the First Time Indent processing.";
        internal const string MsgFoundDuplicateOptionNameOnHeaderLine = "The option \"{1}\" appears more than once for segment \"{0}\". Only the first occurrence will be used.";
        internal const string MsgFoundDuplicateSegmentName = "Segment name \"{0}\" appears more than once in the template file. Default name \"{1}\" will be used in place of the duplicate.";
        internal const string MsgFoundSolutionDirectoryPath = "The solution directory path was determined to be: {0}";
        internal const string MsgFourthCharacterMustBeBlank = "The fourth character of each line should be blank.";
        internal const string MsgFullPathCannotBeDetermined = "The full path can't be determined because the solution directory path is unknown.";
        internal const string MsgGeneratedTextHasBeenReset = "The generated text cache for template file \"{0}\" has been reset.";
        internal const string MsgGeneratedTextIsEmpty = "Unable to write the output file because the generated text is empty.";
        internal const string MsgGeneratedTextIsNull = "Unable to write to the output file because the generated text is null.";
        internal const string MsgIndentValueMustBeValidNumber = "The indent value \"{0}\" is not a valid integer value.";
        internal const string MsgIndentValueOutOfRange = "The FTI option value must be a number between -9 and 9. The value given was {0}.";
        internal const string MsgInvalidControlCode = "\"{0}\" is not a valid control code.";
        internal const string MsgInvalidDirectoryCharacters = "The directory path contains invalid characters.";
        internal const string MsgInvalidFileNameCharacters = "The file name contains invalid characters.";
        internal const string MsgInvalidFormOfOption = "Segment options must follow the form \"option=value\" with no intervening spaces. Found this instead: \"{0}\"";
        internal const string MsgInvalidPadSegmentName = "\"{0}\" is not a valid name for the PAD option for segment \"{1}\". It will be ignored.";
        internal const string MsgInvalidSegmentName = "\"{0}\" is not a valid segment name. The default name \"{1}\" will be used instead.";
        internal const string MsgLeftIndentHasBeenTruncated = "The calculated line indent for segment \"{0}\" went negative. It will be set to zero.";
        internal const string MsgLoadingTemplateFile = "Loading template file \"{0}\"";
        internal const string MsgMinimumLineLengthInTemplateFileIs3 = "All lines in the template file must be at least 3 characters long.";
        internal const string MsgMissingDirectoryPath = "The specified file path doesn't contain a valid directory path.";
        internal const string MsgMissingFileName = "The file name is missing from the file path.";
        internal const string MsgMissingInitialSegmentHeader = "The template file is missing the initial segment header. The default segment \"{0}\" will be used.";
        internal const string MsgMissingTokenName = "Found token start and end delimiters with no token name between them. The token will be ignored.";
        internal const string MsgNextLoadRequestBeforeFirstIsWritten = "Template file \"{0}\" is being loaded before any output was written for template file \"{1}\"";
        internal const string MsgNoTextLinesFollowingSegmentHeader = "The header line for segment \"{0}\" must be followed by at least one text line.";
        internal const string MsgNullDirectoryPath = "The directory path must not be null.";
        internal const string MsgNullFilePath = "The file path must not be null.";
        internal const string MsgOptionNameMustPrecedeEqualsSign = "An option name must appear immediately before the equals sign with no intervening spaces in the \"{0}\" segment header.";
        internal const string MsgOptionValueMustFollowEqualsSign = "The value for option \"{1}\" must appear immediately after the equals sign with no intervening spaces in the \"{0}\" segment header.";
        internal const string MsgOutputDirectoryCleared = "The output directory has been cleared.";
        internal const string MsgOutputDirectoryNotSet = "The output file can't be written because the output directory hasn't been set.";
        internal const string MsgPadSegmentsMustBeDefinedEarlier = "The PAD segment \"{1}\" referenced by segment \"{0}\" must be defined earlier in the template file.";
        internal const string MsgProcessingSegment = "Processing segment...";
        internal const string MsgSegmentHasBeenAdded = "Segment has been added to the control dictionary.";
        internal const string MsgSegmentHasNoTextLines = "Tried to generate segment \"{0}\" but the segment has no text lines.";
        internal const string MsgSegmentNameMustStartInColumn5 = "The segment name must start in column 5 of the segment header line. The default name \"{0}\" will be used.";
        internal const string MsgTabSizeTooLarge = "The requested tab size is too large. The maximum value \"{0}\" will be used.";
        internal const string MsgTabSizeTooSmall = "The requested tab size is too small. The minimum value \"{0}\" will be used.";
        internal const string MsgTabSizeValueMustBeValidNumber = "The tab size value \"{0}\" is not a valid integer value.";
        internal const string MsgTabSizeValueOutOfRange = "The TAB option value must be a number between 1 and 9. The value given was {0}.";
        internal const string MsgTemplateFileIsEmpty = "The template file is empty.";
        internal const string MsgTemplateHasBeenReset = "The environment for template file \"{0}\" has been reset.";
        internal const string MsgTokenDictionaryContainsInvalidTokenName = "The token dictionary contained an invalid token name \"{1}\" for segment \"{0}\".";
        internal const string MsgTokenDictionaryIsEmpty = "An empty token dictionary was supplied for segment \"{0}\".";
        internal const string MsgTokenDictionaryIsNull = "A null token dictionary was supplied for segment \"{0}\".";
        internal const string MsgTokenHasInvalidName = "Found a token with an invalid name: \"{0}\". It will be ignored.";
        internal const string MsgTokenMissingEndDelimiter = "Found a token start delimiter with no matching end delimiter. The token will be ignored.";
        internal const string MsgTokenValueIsEmpty = "Found token \"{1}\" with no assigned value while generating segment \"{0}\".";
        internal const string MsgTokenWithEmptyValue = "Token \"{1}\" was passed in with an empty value for segment \"{0}\".";
        internal const string MsgTokenWithNullValue = "Token \"{1}\" was passed in with a null value for segment \"{0}\".";
        internal const string MsgUnableToLoadTemplate = "Unable to load the template file because a valid file path has not been set.";
        internal const string MsgUnableToLoadTemplateFile = "Encountered an error when trying to load the template file.\n{0}";
        internal const string MsgUnableToLocateSolutionDirectory = "The directory containing the solution file could not be found.";
        internal const string MsgUnableToResetSegment = "Unable to reset segment \"{0}\" because of a null or unknown segment name.";
        internal const string MsgUnableToSetOutputDirectory = "Encountered an error when trying to set the output directory path.\n{0}";
        internal const string MsgUnableToSetTemplateFilePath = "Unable to set the template file path. {0}";
        internal const string MsgUnableToWriteFile = "Unable to write to output file. {0}";
        internal const string MsgUnknownSegmentName = "A request was made to generate segment \"{0}\" but that segment wasn't found in the template file.";
        internal const string MsgUnknownSegmentOptionFound = "An unknown segment option \"{1}\" was found on segment \"{0}\". It will be ignored.";
        internal const string MsgUnknownTokenName = "An unknown token name \"{1}\" was supplied for segment \"{0}\". It will be ignored.";
        internal const string MsgWritingTextFile = "Writing generated text to file \"{0}\"";
        internal const string MsgYesNoPrompt = "Enter Y (yes) or N (no)...";
    }
}