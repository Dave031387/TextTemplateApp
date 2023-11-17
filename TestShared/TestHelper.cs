namespace TestShared
{
    using TemplateProcessor;
    using TemplateProcessor.Logger;

    public class TestHelper
    {
        public bool ResetIndentProcessor { get; set; } = false;

        public bool ResetLocater { get; set; } = false;

        public bool ResetLogger { get; set; } = false;

        public bool ResetNameGenerator { get; set; } = false;

        public bool ResetTokenProcessor { get; set; } = false;

        public void Reset()
        {
            if (ResetNameGenerator)
            {
                DefaultSegmentNameGenerator.Reset();
            }

            if (ResetIndentProcessor)
            {
                IndentProcessor.Reset();
            }

            if (ResetLocater)
            {
                Locater.Reset();
            }

            if (ResetLogger)
            {
                ConsoleLogger.Clear();
            }

            if (ResetTokenProcessor)
            {
                TokenProcessor.ClearTokens();
            }
        }
    }
}