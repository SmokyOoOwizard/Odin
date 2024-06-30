using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Odin.Abstractions.Collectors.Matcher;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.Component.CodeGen.Diagnostics.Impl.Matchers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComponentMatcherCallsDiagnosticAnalyzer : AComponentMatcherConfigureDiagnosticAnalyzer
{
    private static readonly string AllowMethodNamespace = typeof(AComponentMatcher).Namespace!;

    private const string DIAGNOSTIC_ID = "ComponentRules_0006";
    private const string CATEGORY = "Structure";

    private static readonly LocalizableString Message =
        "A component cannot contain arrays with rank greater than 12";

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
                                     .Where(c => c is InvocationExpressionSyntax)
                                     .Where(c => context.SemanticModel.GetSymbolInfo(c).Symbol is IMethodSymbol)
                                     .Where(c => c != null)
                                     .Select(c => c!);

        foreach (var call in calls)
        {
            var symbol = (IMethodSymbol)context.SemanticModel.GetSymbolInfo(call).Symbol!;
            var name = (symbol.ReducedFrom ?? symbol).ToDisplayString();
            if (name.StartsWith(AllowMethodNamespace))
                continue;

            var diagnostic = Diagnostic.Create(Rule, call.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}