using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CleanArchitecture.SourceGenerators.Helpers
{
    public static class SyntaxNodeHelper
    {
        public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
        {
            var nameSpace = string.Empty;
            var potentialNamespaceParent = syntax.Parent;

            while (potentialNamespaceParent != null &&
                   !(potentialNamespaceParent is NamespaceDeclarationSyntax)
                   && !(potentialNamespaceParent is FileScopedNamespaceDeclarationSyntax))
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            if (!(potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)) return nameSpace;
            
            nameSpace = namespaceParent.Name.ToString();
            
            while (true)
            {
                if (!(namespaceParent.Parent is NamespaceDeclarationSyntax parent))
                {
                    break;
                }

                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }

            return nameSpace;
        }
    }
}