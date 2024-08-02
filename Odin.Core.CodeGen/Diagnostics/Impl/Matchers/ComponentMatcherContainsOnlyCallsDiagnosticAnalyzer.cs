using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.Component.CodeGen.Diagnostics.Impl.Matchers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentMatcherContainsOnlyCallsDiagnosticAnalyzer : AComponentMatcherConfigureDiagnosticAnalyzer
{
    private const string DIAGNOSTIC_ID = "MatcherRules_0002";
    private const string CATEGORY = "Matcher";

    private static readonly LocalizableString Message =
        "Only method calls are allowed";

    private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID,
                                                            Message,
                                                            Message,
                                                            CATEGORY,
                                                            DiagnosticSeverity.Error,
                                                            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);


    protected override void Analyze(SyntaxNodeAnalysisContext context, ISymbol declaredSymbol)
    {
        var declarationSyntax = context.Node as MemberDeclarationSyntax;
        if (declarationSyntax == null)
            return;

        var calls = declarationSyntax.ChildNodes()
                                     .Traverse(c => c.ChildNodes())
                                     .Where(c => c is not InvocationExpressionSyntax)
                                     .Where(c => c is not ExpressionStatementSyntax)
                                     .Where(c => c is not ArgumentListSyntax)
                                     .Where(c => c is not ArgumentSyntax)
                                     .Where(c => c is not SimpleLambdaExpressionSyntax)
                                     .Where(c => c is not MemberAccessExpressionSyntax)
                                     .Where(c => c is not GenericNameSyntax)
                                     .Where(c => c is not TypeArgumentListSyntax)
                                     .Where(c => c is not BlockSyntax)
                                     .Where(c => c is not IdentifierNameSyntax)
                                     .Where(c => c is not ParameterSyntax)
                                     .Where(c => c is not ParameterListSyntax)
                                     .Where(c => c is not PredefinedTypeSyntax);

        foreach (var call in calls)
        {
            var diagnostic = Diagnostic.Create(Rule, call.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}