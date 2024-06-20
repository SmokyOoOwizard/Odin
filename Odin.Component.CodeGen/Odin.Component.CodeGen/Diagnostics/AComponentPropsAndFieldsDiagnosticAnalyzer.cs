﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Diagnostics;

public abstract class AComponentPropsAndFieldsDiagnosticAnalyzer : DiagnosticAnalyzer
{
    private static readonly string ComponentInterfaceName = typeof(IComponent).FullName!;

    protected abstract SyntaxKind[] SupportedSyntaxKinds { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAction, SupportedSyntaxKinds);
    }

    private void AnalyzeAction(SyntaxNodeAnalysisContext context)
    {
        var memberSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
        if (memberSymbol == null)
            return;

        var containingType = memberSymbol.ContainingType;
        if (containingType == null)
            return;

        var interfaces = containingType.Interfaces;
        if (interfaces == null)
            return;

        if (interfaces.Select(s => s.OriginalDefinition.ToDisplayString())
                      .All(s => s != ComponentInterfaceName))
            return;

        Analyze(context, memberSymbol);
    }

    protected abstract void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol);
}