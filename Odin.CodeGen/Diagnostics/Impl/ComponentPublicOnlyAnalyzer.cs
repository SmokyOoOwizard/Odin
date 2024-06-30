using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Odin.Component.CodeGen.Diagnostics.Impl;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentPublicOnlyAnalyzer : AComponentTypeDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "ComponentRules_0006";
    private const string CATEGORY = "Declaration";

    private static readonly LocalizableString Message = "A component can only be a public";

    private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID,
                                                            Message,
                                                            Message,
                                                            CATEGORY,
                                                            DiagnosticSeverity.Error,
                                                            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override SyntaxKind[] SupportedSyntaxKinds => new[] { SyntaxKind.StructDeclaration };

    protected override void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol)
    {
        var typeSymbol = declaredSymbol as INamedTypeSymbol;
        if (typeSymbol == null)
            return;

        if (typeSymbol.DeclaredAccessibility == Accessibility.Public)
            return;

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}