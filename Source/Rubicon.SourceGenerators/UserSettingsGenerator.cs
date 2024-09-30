using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Rubicon.SourceGenerators;

[Generator]
public class UserSettingsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        
    }

    public void Execute(GeneratorExecutionContext context)
    {
        INamedTypeSymbol? settingsData = context.Compilation.GetTypeByMetadataName(GenerationConstants.UserSettingsData);
        if (settingsData is null)
            throw new Exception("Could not find UserSettingsData found in \"GenerationConstants.UserSettingsData\".");

        var results = RecursiveSearchForValidOptions(settingsData.GetMembers().ToArray());
        string settingsNameSpace = settingsData.GetNamespaceName();
        
        StringBuilder dataClass = new StringBuilder();
        dataClass.Append("using Godot;\n" +
                         "using Godot.Collections;\n" +
                         "\n" +
                         $"namespace {settingsNameSpace};\n" +
                         "\n" +
                         $"public partial class {settingsData.Name}\n" +
                         "{\n");
        
        // Make load method
        dataClass.Append("\tpublic partial void Load(ConfigFile config)\n" +
                         "\t{\n");
        
        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;
            
            string section = result.pathTo.Substring(0, result.pathTo.IndexOf('.'));
            string key = result.pathTo.Substring(result.pathTo.IndexOf('.') + 1).Replace('.', '/');
            
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t{result.pathTo} = ({result.type.ToDisplayString()})config.GetValue(\"{section}\", \"{key}\").AsInt64();\n");
            else
                dataClass.Append($"\t\t{result.pathTo} = ({result.type.ToDisplayString()})config.GetValue(\"{section}\", \"{key}\");\n");
        }
        
        // Make CreateConfigFileInstance()
        dataClass.Append("\t}\n" +
                         "\n" +
                         "\tpublic partial ConfigFile CreateConfigFileInstance()\n" +
                         "\t{\n" +
                         "\t\tConfigFile file = new ConfigFile();\n\n");
        
        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;
            
            string section = result.pathTo.Substring(0, result.pathTo.IndexOf('.'));
            string key = result.pathTo.Substring(result.pathTo.IndexOf('.') + 1).Replace('.', '/');
            
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\tfile.SetValue(\"{section}\", \"{key}\", (long){result.pathTo});\n");
            else
                dataClass.Append($"\t\tfile.SetValue(\"{section}\", \"{key}\", {result.pathTo});\n");
        }
        
        // Make GetSetting
        dataClass.Append("\t\treturn file;\n" +
                         "\t}\n" +
                         "\n" +
                         "\tpublic partial Variant GetSetting(string key)\n" +
                         "\t{\n" +
                         "\t\tswitch (key)\n" +
                         "\t\t{\n");
        
        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;
            
            dataClass.Append($"\t\t\tcase \"{result.pathTo.Replace('.', '/')}\":\n");
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t\t\treturn (long){result.pathTo};\n");
            else 
                dataClass.Append($"\t\t\t\treturn {result.pathTo};\n");
        }

        dataClass.Append("\t\t\tdefault:\n" +
                         "\t\t\t\treturn default;\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "\n" +
                         "\tpublic partial void SetSetting(string key, Variant val)\n" +
                         "\t{\n" +
                         "\t\tswitch (key)\n" +
                         "\t\t{\n");

        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;
            
            dataClass.Append($"\t\t\tcase \"{result.pathTo.Replace('.', '/')}\":\n");
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t\t\t{result.pathTo} = ({result.type.ToDisplayString()})val.AsInt64();");
            else
                dataClass.Append($"\t\t\t\t{result.pathTo} = ({result.type.ToDisplayString()})val;");
            
            dataClass.Append("\tbreak;\n");
        }

        dataClass.Append("\t\t}\n" +
                         "\t}\n" +
                         "}");
        
        context.AddSource($"{settingsData.Name}.g.cs", dataClass.ToString());
    }

    private (string pathTo, ITypeSymbol type)[] RecursiveSearchForValidOptions(ISymbol[] symbols)
    {
        IPropertySymbol[] properties = symbols
            .Where(x => x.Kind is SymbolKind.Property && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IPropertySymbol>()
            .Where(x => !x.IsReadOnly && !x.IsWriteOnly && !x.IsImplicitlyDeclared)
            .ToArray();
        
        IFieldSymbol[] fields = symbols
            .Where(x => x.Kind is SymbolKind.Field && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IFieldSymbol>()
            .Where(x => !x.IsImplicitlyDeclared)
            .ToArray();

        List<(string pathTo, ITypeSymbol type)> results = new();
        foreach (IPropertySymbol property in properties)
        {
            if (property.Type.TypeKind is TypeKind.Class 
                && !property.Type.InheritsFrom("GodotSharp", "Godot.Collections.Array")
                && !property.Type.InheritsFrom("GodotSharp", "Godot.Collections.Dictionary"))
            {
                var propertyResults =
                    RecursiveSearchForValidOptions(property.Type.GetMembers().ToArray());

                foreach (var result in propertyResults)
                    results.Add((property.Name + "." + result.pathTo, result.type));
                
                continue;
            }
                
            results.Add((property.Name, property.Type));
        }
        
        foreach (IFieldSymbol field in fields)
        {
            if (field.Type.TypeKind is TypeKind.Class
                && !field.Type.InheritsFrom("GodotSharp", "Godot.Collections.Array")
                && !field.Type.InheritsFrom("GodotSharp", "Godot.Collections.Dictionary"))
            {
                var fieldResults =
                    RecursiveSearchForValidOptions(field.Type.GetMembers().ToArray());

                foreach (var result in fieldResults)
                    results.Add((field.Name + "." + result.pathTo, result.type));
                
                continue;
            }
                
            results.Add((field.Name, field.Type));
        }

        return results.ToArray();
    }
}