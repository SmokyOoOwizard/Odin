using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.Component.CodeGen.Utils;

namespace Odin.Component.CodeGen.Diagnostics.Impl;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentHasNonPrimitiveValuesAnalyzer : AComponentPropsAndFieldsDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "ComponentRules_0005";
    private const string CATEGORY = "Structure";

    private static readonly LocalizableString Message =
        "A component cannot contain non primitive types";

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

        if (typeKind is TypedConstantKind.Primitive or TypedConstantKind.Enum)
            return;

        if (typeKind == TypedConstantKind.Array)
        {
            if (typeSymbol.Type is IArrayTypeSymbol array)
            {
                var elementType = array.ElementType;
                var elementTypeKind = elementType.GetTypedConstantKind();

                if (elementTypeKind is TypedConstantKind.Primitive or TypedConstantKind.Enum)
                    return;
            }
        }

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}