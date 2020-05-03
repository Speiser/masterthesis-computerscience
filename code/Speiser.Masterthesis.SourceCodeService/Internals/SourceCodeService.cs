using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Speiser.Masterthesis.SourceCodeService.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts.Syntax;

namespace Speiser.Masterthesis.SourceCodeService.Internals
{
    internal class SourceCodeService : ISourceCodeService
    {
        private static readonly string[] IgnoredFiles = new[]
        {
            "ocelot.json",
            "bundleconfig.json",
            "launchSettings.json",
            "package-lock.json",
            "web.config"
        };
        private static readonly string[] IgnoredFolders = new[]
        {
            "wwwroot",
            @"obj\Debug"
        };

        public bool IsDotnetProject(string path)
            => new DirectoryInfo(path).GetFiles().Any(f => f.Extension == ".csproj");

        public string GetProjectFile(string path) => this.FindProject(new DirectoryInfo(path));

        public IEnumerable<ConfigurationFileInfo> GetConfigurationFiles(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            var project = this.FindProject(directoryInfo);
            return this.GetAllSourceFiles(directoryInfo, ".json", ".config")
                .Where(configFile => !IgnoredFiles.Contains(configFile.Split('\\').Last())
                                  && !IgnoredFolders.Any(ignored => configFile.Contains(ignored)))
                .Select(configFile => new ConfigurationFileInfo
                {
                    Path = configFile,
                    Content = File.ReadAllText(configFile)
                });
        }

        public SourceProjectRepresentation GetParsedSource(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            var project = this.FindProject(directoryInfo);
            var sourceFiles = this.GetAllSourceFiles(directoryInfo, ".cs")
                .Where(sf => !sf.Contains(@"obj\Debug"));

            var types = ParseProjectToInternalRepresentation(sourceFiles);
            return new SourceProjectRepresentation
            {
                ProjectFile = project,
                Types = types
            };
        }

        private IEnumerable<string> GetAllSourceFiles(DirectoryInfo info, params string[] extensions)
        {
            var files = new List<string>();

            foreach (var sub in info.GetDirectories())
            {
                files.AddRange(this.GetAllSourceFiles(sub, extensions));
            }

            foreach (var file in info.GetFiles())
            {
                if (extensions.Contains(file.Extension))
                {
                    files.Add(file.FullName);
                }
            }

            return files;
        }

        private string FindProject(DirectoryInfo info)
            => info.GetFiles().SingleOrDefault(f => f.Extension == ".csproj")?.FullName;

        private IEnumerable<CodeClass> ParseProjectToInternalRepresentation(IEnumerable<string> sourceFiles)
        {
            var allTypes = new List<CodeClass>();

            foreach (var file in sourceFiles)
            {
                var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                var root = tree.GetCompilationUnitRoot();

                var decl = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                allTypes.AddRange(GetTypes(decl));
            }

            return allTypes;
        }

        private static IEnumerable<CodeClass> GetTypes(IEnumerable<ClassDeclarationSyntax> classDeclarations)
        {
            return classDeclarations.Select(classDeclaration => new CodeClass
            {
                Identifier = classDeclaration.Identifier.Text,
                Base = classDeclaration.BaseList?.Types.Select(t => t.ToString()) ?? Enumerable.Empty<string>(),
                Attributes = GetAttributes(classDeclaration.AttributeLists),
                Methods = GetMethods(classDeclaration)
            });
        }

        private static IEnumerable<CodeMethod> GetMethods(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(methodDeclaration =>
                {
                    // Note: This will not handle ArrowExpressions
                    var statements = methodDeclaration.Body is null
                        ? Enumerable.Empty<CodeStatement>()
                        : GetBodyStatements(methodDeclaration.Body);
                    return new CodeMethod
                    {
                        Attributes = GetAttributes(methodDeclaration.AttributeLists),
                        Identifier = methodDeclaration.Identifier.Text,
                        Statements = statements
                    };
                });
        }

        private static IEnumerable<CodeStatement> GetBodyStatements(BlockSyntax blockSyntax)
            => blockSyntax.Statements.Select(s => new CodeStatement { Statement = s.ToFullString() });

        private static IEnumerable<CodeAttribute> GetAttributes(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var attributes = new List<CodeAttribute>();
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var temp = new CodeAttribute
                    {
                        Identifier = attribute.Name.ToString()
                    };

                    if (attribute.ArgumentList != null)
                        temp.Param = attribute.ArgumentList.Arguments.Select(x => x.ToString());

                    attributes.Add(temp);
                }
            }
            return attributes;
        }
    }
}
