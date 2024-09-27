using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rubicon.SourceGenerators;

public static class SourceGeneratorUtil
{
    /// <summary>
    /// Whether the symbol inherits from the type provided.
    /// </summary>
    /// <param name="symbol">The current symbol</param>
    /// <param name="assemblyName">The assembly name (typically the csproj name, like "GodotSharp")</param>
    /// <param name="typeFullName">The type's full name (Example: Godot.Node)</param>
    /// <returns></returns>
    public static bool InheritsFrom(this ITypeSymbol? symbol, string assemblyName, string typeFullName)
    {
        while (symbol != null)
        {
            if (symbol.ContainingAssembly?.Name == assemblyName && symbol.FullQualifiedNameOmitGlobal() == typeFullName)
                return true;

            symbol = symbol.BaseType;
        }

        return false;
    }
    
    /// <summary>
    /// Tests if the script is a Godot script.
    /// </summary>
    /// <param name="cds">The <see cref="ClassDeclarationSyntax"/></param>
    /// <param name="compilation">The compilation</param>
    /// <param name="symbol">The symbol to test</param>
    /// <returns>True if it is a valid Godot class, false if not.</returns>
    private static bool TryGetGodotScriptClass(this ClassDeclarationSyntax cds, Compilation compilation, out INamedTypeSymbol? symbol)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(cds.SyntaxTree);
        INamedTypeSymbol? classTypeSymbol = semanticModel.GetDeclaredSymbol(cds);

        if (classTypeSymbol?.BaseType == null || !classTypeSymbol.BaseType.InheritsFrom("GodotSharp", "Godot.GodotObject"))
        {
            symbol = null;
            return false;
        }

        symbol = classTypeSymbol;
        return true;
    }
    
    /// <summary>
    /// Only selects classes inheriting from Godot.GodotObject.
    /// </summary>
    /// <param name="source">The source</param>
    /// <param name="compilation">The compilation</param>
    /// <returns>Godot inheriting classes</returns>
    public static IEnumerable<(ClassDeclarationSyntax cds, INamedTypeSymbol symbol)> SelectGodotScriptClasses(this IEnumerable<ClassDeclarationSyntax> source, Compilation compilation)
    {
        foreach (var cds in source)
        {
            if (cds.TryGetGodotScriptClass(compilation, out var symbol))
                yield return (cds, symbol!);
        }
    }
        
    /// <summary>
    /// A <see cref="SymbolDisplayFormat.FullyQualifiedFormat"/> that omits the global namespace.
    /// </summary>
    private static SymbolDisplayFormat FullyQualifiedFormatOmitGlobal { get; } =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    /// <summary>
    /// Get the fully qualified name of a symbol. Omits the global namespace.
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <returns>The fully qualified name w/o the global namespace (Example: Rubicon.Data.Generation.StaticAutoloadSingletonAttribute)</returns>
    public static string FullQualifiedNameOmitGlobal(this ITypeSymbol symbol) // SymbolDisplayTypeQualificationStyle
        => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatOmitGlobal);

    /// <summary>
    /// Get the fully qualified name of a namespace symbol. Omits the global namespace.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol</param>
    /// <returns>The fully qualified name w/o the global namespace (Example: Rubicon.Data.Generation)</returns>
    public static string FullQualifiedNameOmitGlobal(this INamespaceSymbol namespaceSymbol)
        => namespaceSymbol.ToDisplayString(FullyQualifiedFormatOmitGlobal);
        
    /// <summary>
    /// Checks if the provided symbol is of the type specified at <see cref="GenerationConstants.StaticAutoloadAttr"/>.
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <returns>Whether the type is matching or not</returns>
    public static bool IsStaticAutoloadAttribute(this INamedTypeSymbol symbol)
        => symbol.FullQualifiedNameOmitGlobal() == GenerationConstants.StaticAutoloadAttr;
}