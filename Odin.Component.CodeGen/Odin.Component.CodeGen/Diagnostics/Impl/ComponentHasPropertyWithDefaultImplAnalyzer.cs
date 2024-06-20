using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Odin.Component.CodeGen.Diagnostics.Impl;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentHasPropertyWithDefaultImplAnalyzer : AComponentPropsAndFieldsDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "ComponentRules_0001";
    private const string CATEGORY = "Structure";

    private static readonly LocalizableString Message =
        "A component cannot contain properties with default implementation";

    private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID,
                                                            Message,
                                                            Message,
                                                            CATEGORY,
                                                            DiagnosticSeverity.Warning,
                                                            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override SyntaxKind[] SupportedSyntaxKinds => new[] { SyntaxKind.PropertyDeclaration };

    protected override void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol)
    {
        var declaration = context.Node.ToFullString();

        if (!declaration.Contains("get;") && !declaration.Contains("set;"))
            return;

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}