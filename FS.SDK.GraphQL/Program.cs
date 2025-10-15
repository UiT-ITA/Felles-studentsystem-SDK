// See https://aka.ms/new-console-template for more information
using GraphQlClientGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text;
using System;
using System.IO;

Console.WriteLine("Load configuration");

var builder = new ConfigurationBuilder()
    //.SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
    //.AddEnvironmentVariables();
IConfiguration config = builder.Build();

Console.WriteLine("Fix folder");
DirectoryInfo sourceDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\..\\FS.SDK.GraphQL.Model");
Console.WriteLine($"Output directory: {sourceDir.FullName}");


Console.WriteLine("Generating FS GraphQL classes");

string apikeyname = config["apikeyname"] ?? throw new Exception("apikeyname not found in appsettings.json");
string apikey = config["apikey"] ?? throw new Exception("apikey not found in appsettings.json");

// Pass an empty list instead of a single KeyValuePair
var schema = await GraphQlGenerator.RetrieveSchema(
    HttpMethod.Post,
    "https://gw-uit.intark.uh-it.no/fs-graphql/",
    new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(apikeyname, apikey) }
);


// Single-file generation

//var configuration = new GraphQlGeneratorConfiguration()
//{ 
//    TargetNamespace = "FS.SDK.GraphQL.Model", 
//    MemberAccessibility = MemberAccessibility.Public,
//    CodeDocumentationType = CodeDocumentationType.DescriptionAttribute, 

//};
//var generator = new GraphQlGenerator(configuration);

//var builder = new StringBuilder();

//using var writer = new StringWriter(builder);
//var generationContext = new SingleFileGenerationContext(schema, writer) { LogMessage = Console.WriteLine };
//generator.Generate(generationContext);
//var csharpCode = builder.ToString();
//Console.WriteLine(csharpCode);


// Multiple file generation
var gqlconfig = new GraphQlGeneratorConfiguration
{
    CodeDocumentationType = CodeDocumentationType.Disabled,
    TargetNamespace = "FS.SDK.GraphQL.Model",
    CSharpVersion = CSharpVersion.CSharp12,
    JsonPropertyGeneration = JsonPropertyGenerationOption.CaseSensitive,
    //JsonPropertyGeneration = JsonPropertyGenerationOption.Always,
    IncludeDeprecatedFields = false,
    PropertyGeneration = PropertyGenerationOption.AutoProperty,
    // EnumValueNaming = EnumValueNamingOption.Original
    EnumValueNaming = EnumValueNamingOption.CSharp

    // BooleanTypeMapping
    // FloatTypeMapping
    // IdTypeMapping
    // IntegerTypeMapping
    // ScalarFieldTypeMappingProvider

    // InputObjectMode
    // MemberAccessibility

    // ClassPrefix
    // ClassSuffix
    // FileScopedNamespaces
    // GenerationOrder

    // CustomClassNameMapping
    // DataClassMemberNullability
    // EnableNullableReferences
    // GeneratePartialClasses
};

var generator = new GraphQlGenerator(gqlconfig);

var strbuilder = new StringBuilder();
using var writer = new StringWriter(strbuilder);

var codeFileEmitter = new FileSystemEmitter(sourceDir.FullName);

GenerationContext context = new MultipleFileGenerationContext(
   schema: schema,
   codeFileEmitter: codeFileEmitter,
   projectFileName: "FS.SDK.GraphQL.Model.csproj", 
   objectTypes: GeneratedObjectType.All
   // GeneratedObjectType.DataClasses
   );

generator.Generate(context);

