using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.CodeGen.Utils;

public static class ComponentFieldCodeGen
{

    public static IEnumerable<string> GetCodeFieldDeclarations(ImmutableArray<ISymbol> componentFields)
    {
        var processedMembers = ComponentFieldProcessor.GetFieldDeclarations(componentFields)
                              .Select(w =>
                               {
                                   var fieldDeclaration = new[]
                                   {
                                       $".AddField(\"{w.Name}\")",
                                       w.IsIndex ? ".Index()" : "",
                                       $".Type({nameof(EFieldType)}.{w.Type})",
                                       w.CollectionType != ECollectionType.None
                                           ? $".Collection({nameof(ECollectionType)}.{w.CollectionType})"
                                           : "",
                                   }.Where(str => !string.IsNullOrWhiteSpace(str));

                                   return string.Join("\n\t\t\t\t", fieldDeclaration);
                               })
                              .Where(str => !string.IsNullOrWhiteSpace(str))
                              .Select(str => str!);

        return processedMembers;
    }
}