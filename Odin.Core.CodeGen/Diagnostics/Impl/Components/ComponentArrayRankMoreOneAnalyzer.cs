using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.Core.CodeGen.Diagnostics.Impl.Components;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentArrayRankMoreOneAnalyzer : AComponentPropsAndFieldsDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "ComponentRules_0004";
    private const string CATEGORY = "Structure";

    private static readonly LocalizableString Message =
        "A component cannot contain arrays with rank greater than 1";

    private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID,
                                                            Message,
                                                            Message,
                                                            CATEGORY,
                                                            DiagnosticSeverity.Error,
                                                            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override SyntaxKind[] SupportedSyntaxKinds => new[] { SyntaxKind.FieldDeclaration };

    protected override void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol)
    {
        if (declaredSymbol is not IFieldSymbol typeSymbol)
            return;

        var type = typeSymbol.Type;
        var typeKind = type.GetTypedConstantKind();

        if (typeKind != TypedConstantKind.Array)
            return;

        if (typeSymbol.Type is IArrayTypeSymbol { Rank: 1 })
            return;

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}