using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Matchers;

namespace Odin.CodeGen.Abstractions;

public abstract class AComponentMatcherIncrementalGenerator : ISourceGenerator
{
    private static readonly string ComponentMatcherBaseName = typeof(AComponentMatcherBase).FullName!;

    protected abstract void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<(MethodDeclarationSyntax, FilterComponent)> matchers
    );

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var matchers = context.Compilation.SyntaxTrees
                              .SelectMany(st => st
                                               .GetRoot()
                                               .DescendantNodes()
                                               .OfType<ClassDeclarationSyntax>())
                              .Select(c =>
                               {
                                   var sm = context.Compilation.GetSemanticModel(c.SyntaxTree);

                                   return sm.GetDeclaredSymbol(c)!;
                               })
                              .OfType<INamedTypeSymbol>()
                              .Where(s => TypeUtils.CheckBaseType(s, ComponentMatcherBaseName))
                              .Where(symbol=>symbol.DeclaredAccessibility == Accessibility.Public)
                              .Select(s =>
                               {
                                   var members = s.GetMembers()
                                                  .First(ms => ms.Name == nameof(AComponentMatcherBase.Configure) &&
                                                              ms is IMethodSymbol);


                                   var declaring = members.DeclaringSyntaxReferences.First();

                                   var node = (MethodDeclarationSyntax)declaring.GetSyntax();

                                   var model = context.Compilation.GetSemanticModel(node.SyntaxTree);
                                     
                                   var parsedFilter = Parse(node, model);

                                   return (node, parsedFilter);
                               }).ToArray();

        GenerateCode(context, matchers);
    }

    private FilterComponent Parse(MethodDeclarationSyntax node, SemanticModel model)
    {
        if (node.Body == null)
            throw new Exception(); // todo

        var rawNode = node.Body.ChildNodes().FirstOrDefault(c =>
        {
            var raw = c.ToString();
            return raw.StartsWith("Filter().");
        })?.ToString();

        if (string.IsNullOrWhiteSpace(rawNode))
            return new FilterComponent() { Type = EFilterType.All };

        rawNode = rawNode!.RemoveComments();
        rawNode = rawNode.Replace("Filter()", "");

        var parsedFilter = ParseFunction(rawNode, node, model);

        return parsedFilter;
    }

    private FilterComponent[] ParseArguments(
        string text,
        MethodDeclarationSyntax node,
        SemanticModel model
    )
    {
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<FilterComponent>();

        var arguments = new List<FilterComponent>();

        var tailStart = 0;
        while (tailStart < text.Length)
        {
            var substring = text.Substring(tailStart);
            var end = 0;
            while (true)
            {
                var stringWithOffset = substring.Substring(end);
                var group = StringParser.ParseRecursiveGroups(stringWithOffset);
                if (group.StartIndex == -1)
                {
                    break;
                }

                end += group.EndIndex;

                var tail = stringWithOffset.Substring(group.EndIndex);
                var commaOffset = tail.IndexOf(',');
                if (commaOffset != -1)
                {
                    var stringBeforeComma = tail.Substring(0, commaOffset);
                    if (string.IsNullOrWhiteSpace(stringBeforeComma))
                        break;
                }
            }

            var funcOffset = substring.IndexOf('.');
            var funcText = substring.Substring(funcOffset, end - funcOffset);

            var parsedFunction = ParseFunction(funcText, node, model);
            arguments.Add(parsedFunction);

            var commaIndex = substring.IndexOf(',') + 1;
            if (commaIndex == 0)
            {
                break;
            }

            tailStart += commaIndex;
        }


        return arguments.ToArray();
    }

    private FilterComponent ParseFunction(string text, MethodDeclarationSyntax node, SemanticModel model)
    {
        var group = StringParser.ParseRecursiveGroups(text);
        if (group.StartIndex == -1)
            return new FilterComponent() { Type = EFilterType.Unknown };

        var dotIndex = text.IndexOf('.');
        var methodName = text.Substring(dotIndex + 1, group.StartIndex - 1);
        var genericArg = string.Empty;

        var genericStart = methodName.IndexOf('<');
        var genericEnd = methodName.IndexOf('>');
        if (genericStart != -1 && genericEnd != -1)
        {
            genericArg = methodName.Substring(genericStart + 1, genericEnd - genericStart - 1);
            methodName = methodName.Substring(0, genericStart);

            var oo = node
                    .ChildNodes()
                    .Traverse(c => c.ChildNodes())
                    .OfType<GenericNameSyntax>()
                    .SelectMany(c => c.TypeArgumentList.Arguments)
                    .OfType<IdentifierNameSyntax>()
                    .First(c => c.Identifier.ValueText == genericArg);

            if(model.GetSymbolInfo(oo).Symbol is not INamedTypeSymbol typeSymbol)
                throw new Exception(); // todo

            genericArg = typeSymbol.OriginalDefinition.ToString();
        }

        var methodType = FilterTypeUtils.GetFilterType(methodName);

        var chain = new List<FilterComponent>();

        var tailStart = group.EndIndex;
        while (tailStart < text.Length)
        {
            var tailRaw = text.Substring(tailStart, text.Length - tailStart);
            var body = StringParser.ParseRecursiveGroups(tailRaw);
            if (body.StartIndex == -1)
                break;

            var tailPart = tailRaw.Substring(0, body.EndIndex);
            tailStart += tailPart.Length;

            chain.Add(ParseFunction(tailPart, node, model));
        }

        var children = ParseArguments(group.InnerText, node, model);

        var parsed = new FilterComponent
        {
            GenericArg = genericArg,
            Type = methodType,
            Children = children
        };

        if (chain.Count > 0)
        {
            var parentChildren = new List<FilterComponent>
            {
                parsed
            };
            parentChildren.AddRange(chain);

            var parent = new FilterComponent
            {
                Type = EFilterType.All,
                Children = parentChildren.ToArray()
            };

            return parent;
        }


        return parsed;
    }
}