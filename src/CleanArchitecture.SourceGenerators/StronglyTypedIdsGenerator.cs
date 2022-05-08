using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CleanArchitecture.SourceGenerators
{
    [Generator]
    public class StronglyTypedIdsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver)) return;

            foreach (var classDeclarationSyntax in syntaxReceiver.Classes)
            {
                var model = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (!(model.GetDeclaredSymbol(classDeclarationSyntax) is ITypeSymbol symbol)) return;

                var symbolNamespace = classDeclarationSyntax.GetNamespace();
                var symbolName = symbol.Name;

                context.AddSource(
                    $"{symbolName}Id.g.cs",
                    CreateStronglyTypedIdSource(symbolNamespace, $"{symbolName}Id"));

                context.AddSource(
                    $"{symbol.Name}.g.cs",
                    CreateStronglyTypedIdImplementationSource(symbolNamespace, symbolName));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private static string CreateStronglyTypedIdSource(string fullNamespace, string symbolName)
        {
            return $@"#nullable enable
using CleanArchitecture.Domain.DomainObjects;
namespace {fullNamespace};
public partial record {symbolName} : StronglyTypedIdBase 
{{
    public {symbolName}(Guid? id) : base(id) {{}}
    public {symbolName}(string? id) : base(id) {{}}

    public static implicit operator Guid({symbolName}? id) => id?.Id ?? Guid.Empty;
    public static implicit operator Guid?({symbolName}? id) => id?.Id;

    public static explicit operator {symbolName}(Guid guid) => new(guid);
    public static explicit operator {symbolName}(string guidIdString) => new(guidIdString);

    public static {symbolName} Empty {{ get; }} = new(Guid.Empty);
    public static {symbolName} NewId() => new(Guid.NewGuid());

    public override string ToString() => base.ToString();
}}";

        }

        private static string CreateStronglyTypedIdImplementationSource(string fullNamespace, string symbolName)
        {
            return $@"namespace {fullNamespace};
public partial class {symbolName}
{{
    public {symbolName}Id Id {{ get; set; }} = {symbolName}Id.NewId();
}}";
        }
    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        private const string MarkupInterfaceName = "IEntity";
        public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is ClassDeclarationSyntax classDeclaration)) return;
            if (classDeclaration.BaseList is null) return;

            var typesName = classDeclaration.BaseList.Types.Select(type => type.Type.ToString());

            if (typesName.Any(type => type is MarkupInterfaceName))
                Classes.Add(classDeclaration);
        }
    }
}
