namespace TextTemplateApp
{
    using ClassPropertyParser;

    internal class Program
    {
        private const string OutputDirectoryPath = @".\TextTemplateApp\Wrappers\Generated";
        private const string TemplateFilePath = @".\TextTemplateApp\Wrappers\Template\WrapperTemplate.txt";
        private const string WrapperNamespace = nameof(TextTemplateApp);

        private static void Main()
        {
            IClassPropertyParser parser = new ClassPropertyParser(typeof(TestModels.Friend));
            ModelWrapperGenerator generator = new(TemplateFilePath, OutputDirectoryPath, WrapperNamespace);
            generator.ClearOutputDirectory();
            generator.Generate(parser);
        }
    }
}