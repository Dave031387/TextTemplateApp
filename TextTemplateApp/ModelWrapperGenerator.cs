namespace TextTemplateApp
{
    using ClassPropertyParser;
    using System.Reflection;
    using TemplateProcessor.Console;

    public class ModelWrapperGenerator : TextTemplateConsoleBase
    {
        private const string GeneratedFileNameSuffix = "Wrapper.g.cs";
        private const string SegmentClassStart = "ClassStart";
        private const string SegmentClosingBrace = "ClosingBrace";
        private const string SegmentCollectionInitItem = "CollectionInitItem";
        private const string SegmentCollectionInitStart = "CollectionInitStart";
        private const string SegmentCollectionProperty = "CollectionProperty";
        private const string SegmentCollectionUsings = "CollectionUsings";
        private const string SegmentComplexInitItem = "ComplexInitItem";
        private const string SegmentComplexInitStart = "ComplexInitStart";
        private const string SegmentComplexProperty = "ComplexProperty";
        private const string SegmentComplexUsing = "ComplexUsing";
        private const string SegmentNamespace = "Namespace";
        private const string SegmentSimpleProperty = "SimpleProperty";
        private const string TokenClassName = "ClassName";
        private const string TokenItemType = "ItemType";
        private const string TokenModelNamespace = "ModelNamespace";
        private const string TokenPropertyName = "PropertyName";
        private const string TokenPropertyType = "PropertyType";
        private const string TokenWrapperNamespace = "WrapperNamespace";
        private readonly Dictionary<string, string> _tokenDictionary = new();
        private readonly string _wrapperNamespace;

        public ModelWrapperGenerator(string templateFilePath, string outputDirectoryPath, string wrapperNamespace)
        {
            LoadTemplate(templateFilePath);
            SetOutputDirectory(outputDirectoryPath);
            _wrapperNamespace = wrapperNamespace;
        }

        public void Generate(IClassPropertyParser parser)
        {
            _tokenDictionary.Clear();

            InsertToken(TokenWrapperNamespace, _wrapperNamespace);

            foreach (Type modelType in parser.ClassTypes)
            {
                GenerateModelWrapper(parser, modelType);
            }

            _ = ShowContinuationPrompt("The generator has completed successfully. Press [ENTER] to exit.");
        }

        private void GenerateCollectionProperties(IClassPropertyParser parser)
        {
            foreach (PropertyInfo propertyInfo in parser.CollectionProperties)
            {
                InsertToken(TokenPropertyName, propertyInfo.Name);
                InsertToken(TokenItemType, propertyInfo.PropertyType.GenericTypeArguments[0].Name);
                GenerateSegment(SegmentCollectionProperty, _tokenDictionary);
            }
        }

        private void GenerateCollectionPropertyInitialization(IClassPropertyParser parser)
        {
            if (parser.CollectionProperties.Any())
            {
                GenerateSegment(SegmentCollectionInitStart);

                foreach (PropertyInfo propertyInfo in parser.CollectionProperties)
                {
                    InsertToken(TokenPropertyName, propertyInfo.Name);
                    InsertToken(TokenItemType, propertyInfo.PropertyType.GenericTypeArguments[0].Name);
                    GenerateSegment(SegmentCollectionInitItem, _tokenDictionary);
                }

                GenerateSegment(SegmentClosingBrace);
            }
        }

        private void GenerateComplexProperties(IClassPropertyParser parser)
        {
            foreach (PropertyInfo propertyInfo in parser.ComplexProperties)
            {
                InsertToken(TokenPropertyType, propertyInfo.PropertyType.Name);
                InsertToken(TokenPropertyName, propertyInfo.Name);
                GenerateSegment(SegmentComplexProperty, _tokenDictionary);
            }
        }

        private void GenerateComplexPropertyInitialization(IClassPropertyParser parser)
        {
            if (parser.ComplexProperties.Any())
            {
                GenerateSegment(SegmentComplexInitStart);

                foreach (PropertyInfo propertyInfo in parser.ComplexProperties)
                {
                    InsertToken(TokenPropertyName, propertyInfo.Name);
                    InsertToken(TokenPropertyType, propertyInfo.PropertyType.Name);
                    GenerateSegment(SegmentComplexInitItem, _tokenDictionary);
                }

                GenerateSegment(SegmentClosingBrace);
            }
        }

        private void GenerateModelWrapper(IClassPropertyParser parser, Type modelType)
        {
            GenerateNamespaceAndUsings(modelType, parser);
            parser.GetAllProperties(modelType);
            GenerateSegment(SegmentClassStart);
            GenerateSimpleProperties(parser);
            GenerateComplexProperties(parser);
            GenerateCollectionProperties(parser);
            GenerateComplexPropertyInitialization(parser);
            GenerateCollectionPropertyInitialization(parser);
            GenerateSegment(SegmentClosingBrace);
            GenerateSegment(SegmentClosingBrace);
            WriteGeneratedTextToFile(modelType.Name + GeneratedFileNameSuffix);
        }

        private void GenerateNamespaceAndUsings(Type modelType, IClassPropertyParser parser)
        {
            int index = modelType.FullName!.LastIndexOf('.');
            string modelNamespace = modelType.FullName[..index];

            InsertToken(TokenModelNamespace, modelNamespace);
            InsertToken(TokenClassName, modelType.Name);

            GenerateSegment(SegmentNamespace, _tokenDictionary);

            if (parser.CollectionProperties.Any())
            {
                GenerateSegment(SegmentCollectionUsings);
            }
            else if (parser.ComplexProperties.Any())
            {
                GenerateSegment(SegmentComplexUsing);
            }
        }

        private void GenerateSimpleProperties(IClassPropertyParser parser)
        {
            foreach (PropertyInfo propertyInfo in parser.SimpleProperties)
            {
                InsertToken(TokenPropertyType, ClassPropertyParser.GetTypeName(propertyInfo.PropertyType));
                InsertToken(TokenPropertyName, propertyInfo.Name);
                GenerateSegment(SegmentSimpleProperty, _tokenDictionary);
            }
        }

        private void InsertToken(string tokenName, string tokenValue)
        {
            if (_tokenDictionary.ContainsKey(tokenName))
            {
                _tokenDictionary[tokenName] = tokenValue;
            }
            else
            {
                _tokenDictionary.Add(tokenName, tokenValue);
            }
        }
    }
}