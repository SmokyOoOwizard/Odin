using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Odin.Component.CodeGen.Diagnostics.Impl;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentHasRefValuesAnalyzer : AComponentPropsAndFieldsDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "ComponentRules_0003";
    private const string CATEGORY = "Structure";

    private static readonly LocalizableString Message =
        "A component cannot contain reference fields";

    private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID,
                                                            Message,
                                                            Message,
                                                            CATEGORY,
                                                            DiagnosticSeverity.Warning,
                                                            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override SyntaxKind[] SupportedSyntaxKinds => new[] { SyntaxKind.FieldDeclaration};

    protected override void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol)
    {
        if (declaredSymbol is not IFieldSymbol typeSymbol)
            return;
        
        if (!typeSymbol.Type.IsReferenceType || typeSymbol.Type.OriginalDefinition.ToString() == "string")
            return;

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}