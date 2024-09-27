using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rubicon.SourceGenerators
{
    [Generator]
    public class StaticAutoloadSingletonGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            
        }

        public void Execute(GeneratorExecutionContext context)
        {
            INamedTypeSymbol[] godotClassesWithAttr = context
                .Compilation.SyntaxTrees
                .SelectMany(tree =>
                    tree.GetRoot().DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .SelectGodotScriptClasses(context.Compilation)
                        .Where(x => x.symbol.InheritsFrom("GodotSharp", "Godot.Node") && x.symbol.GetAttributes().Any(a => a.AttributeClass?.IsStaticAutoloadAttribute() ?? false))
                        .Select(x => x.symbol))
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .ToArray();

            foreach (INamedTypeSymbol autoLoadClass in godotClassesWithAttr)
            {
                
            }
        }

        private void MakeStaticAutoloadClass()
        {
            
        }
    }
}