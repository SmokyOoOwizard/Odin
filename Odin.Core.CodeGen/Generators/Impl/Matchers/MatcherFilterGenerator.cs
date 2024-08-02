using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Odin.Abstractions.Components.Utils;
using Odin.CodeGen.Abstractions;

namespace Odin.Core.CodeGen.Generators.Impl.Matchers;

[Generator]
public class MatcherFilterGenerator : AComponentMatcherIncrementalGenerator
{
    protected override void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<(MethodDeclarationSyntax, FilterComponent)> matchers
    )
    {
        var namespaceName = context.Compilation.AssemblyName;

        var processedMatchers = matchers.Select(c =>
                                         {
                                             var json = JsonConvert.SerializeObject(c.Item2);
                                             var id = TypeComponentUtils.GetComponentTypeId(json);

                                             var parent = (ClassDeclarationSyntax)c.Item1.Parent!;

                                             var semanticModel =
                                                 context.Compilation.GetSemanticModel(c.Item1.SyntaxTree);
                                             var typeSymbol = semanticModel.GetDeclaredSymbol(parent)!;

                                             return new
                                             {
                                                 json,
                                                 id,
                                                 filter = c.Item2,
                                                 syntax = c.Item1,
                                                 symbol = typeSymbol
                                             };
                                         })
                                        .GroupBy(c => c.id)
                                        .Select(c => c.First())
                                        .ToArray();

        var filterCases = processedMatchers.Select(c =>
        {
            var fullName = c.symbol.ToDisplayString().Replace('.', '_');
            return $"case {c.id}: return Filter_{fullName};";
        }).ToArray();

        var filterFunctions = processedMatchers
                             .Select(c =>
                              {
                                  var fullName = c.symbol.ToDisplayString().Replace('.', '_');

                                  var hasPart = "entity.Components.Has(entity, {0})";
                                  var notHasPart = "!entity.Components.Has(entity, {0})";

                                  var addedPart = $"({notHasPart} && entity.Changes.Has(entity, {{0}}))";
                                  var removedPart = $"({hasPart} && entity.Changes.WasRemoved(entity, {{0}}))";
                                  var anyChangesPart =
                                      "(entity.Changes.Has(entity, {0}) || entity.Changes.WasRemoved(entity, {0}))";

                                  string AllPart(string[] arg) => $"({string.Join(" && ", arg)})";
                                  string AnyPart(string[] arg) => $"({string.Join(" || ", arg)})";
                                  string NotPart(string[] arg) => $"(!{string.Join(" && !", arg)})";

                                  string FilterToString(FilterComponent component)
                                  {
                                      var type = component.Type;
                                      if (type is EFilterType.All or EFilterType.Any or EFilterType.Not)
                                      {
                                          if (component.Children == null || component.Children.Length == 0)
                                          {
                                              return "false";
                                          }

                                          var children = component.Children.Select(FilterToString).ToArray();

                                          var formated = type switch
                                          {
                                              EFilterType.All => AllPart(children),
                                              EFilterType.Any => AnyPart(children),
                                              EFilterType.Not => NotPart(children),
                                          };

                                          return formated;
                                      }

                                      {
                                          var id = TypeComponentUtils.GetComponentTypeId(component.GenericArg);

                                          var conditionPreFormated = type switch
                                          {
                                              EFilterType.Has => hasPart,
                                              EFilterType.NotHas => notHasPart,
                                              EFilterType.Added => addedPart,
                                              EFilterType.Removed => removedPart,
                                              EFilterType.AnyChanges => anyChangesPart,
                                          };

                                          var formated = string.Format(conditionPreFormated, id);

                                          return formated;
                                      }
                                  }

                                  var condition = FilterToString(c.filter);

                                  return $@"
    private static bool Filter_{fullName}(Entity entity)
    {{
        if ({condition})
            return true;
        return false;
    }}
";
                              })
                             .ToArray();

        var code = $@"
// <auto-generated/>

using System;
using System.Linq;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Collectors;

namespace {namespaceName};
public partial class MatcherFilterRepository
{{
    public FilterComponentDelegate GetFilter(ulong matcherId) 
    {{
        switch (matcherId)
        {{
            {string.Join("\n\t\t\t", filterCases)}
            default: 
                throw new Exception($""Matcher with id {{matcherId}} not found"");
        }} 
    }}

{string.Join("\n", filterFunctions)}
}}
";

        context.AddSource("MatcherFilterRepository.Filters.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}