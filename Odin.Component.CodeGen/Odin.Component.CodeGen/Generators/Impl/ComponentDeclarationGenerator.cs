﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.CodeGen.Abstractions;
using Odin.Component.CodeGen.Utils;

namespace Odin.Component.CodeGen.Generators.Impl;

[Generator]
public class ComponentDeclarationGenerator : AComponentIncrementalGenerator
{
    protected override void GenerateCode(GeneratorExecutionContext context, IEnumerable<INamedTypeSymbol> components)
    {
        var namespaceName =context.Compilation.AssemblyName;

        var methodBody = components
                        .Select(s =>
                         {
                             var fullName = s.OriginalDefinition.ToDisplayString();

                             var members = s.GetMembers();

                             var processedMembers = ComponentFieldProcessor.GetCodeFieldDeclarations(members);

                             return $"""
                                             Component<{fullName}>()
                                                 .WithName("{fullName}")
                                                 .WithId(TypeComponentUtils.GetComponentTypeId<{fullName}>())
                                                 {string.Join("\n\t\t\t", processedMembers)}
                                                 .Build();
                                     """;
                         });

        var code = $@"
// <auto-generated/>

using Odin.Abstractions.Components.Declaration;
using Odin.Abstractions.Components.Declaration.Builder.States;
using Odin.Abstractions.Components.Utils;

namespace {namespaceName};

public class ComponentDeclarations : AComponentDeclarations<ComponentDeclarations>
{{
    protected override void Configure()
    {{
{string.Join("\n\n", methodBody)}
    }}
}}
";

        context.AddSource("ComponentDeclarations.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}