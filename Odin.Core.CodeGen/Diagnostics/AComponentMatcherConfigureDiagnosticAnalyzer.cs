using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.Abstractions.Collectors.Matcher;

namespace Odin.Component.CodeGen.Diagnostics;

public abstract class AComponentMatcherConfigureDiagnosticAnalyzer : DiagnosticAnalyzer
{
    private static readonly string ComponentMatcherBaseName = typeof(AReactiveComponentMatcher).FullName!;

    private SyntaxKind[] SupportedSyntaxKinds => new[] { SyntaxKind.MethodDeclaration };

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAction, SupportedSyntaxKinds);
    }


    private void AnalyzeAction(SyntaxNodeAnalysisContext context)
    {
        var memberSymbol = context.ContainingSymbol?.OriginalDefinition;
        if (memberSymbol == null)
            return;

        var name = memberSymbol.Name;
        if (name != nameof(AReactiveComponentMatcher.Configure) && memberSymbol.IsOverride)
            return;

        var containingType = memberSymbol.ContainingType;
        var baseType = containingType.BaseType;

        var hasMatcherType = false;
        while (baseType != null)
        {
            var baseTypeName = baseType.OriginalDefinition.ToDisplayString();
            if (baseTypeName == ComponentMatcherBaseName)
            {
                hasMatcherType = true;
                break;
            }

            baseType = baseType.BaseType;
        }

        if (!hasMatcherType)
            return;


        Analyze(context, memberSymbol);
    }

    protected abstract void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol);
}