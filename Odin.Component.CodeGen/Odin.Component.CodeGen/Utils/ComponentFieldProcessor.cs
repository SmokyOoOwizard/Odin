using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Declaration;

namespace Odin.Component.CodeGen.Utils;

public static class ComponentFieldProcessor
{
    public static IEnumerable<ComponentFieldDeclaration> GetFieldDeclarations(ImmutableArray<ISymbol> componentFields)
    {
        var processedMembers = componentFields
                              .Select(w => w as IFieldSymbol)
                              .Where(w => w != null)
                              .Select(w => w!)
                              .Where(w => !w.IsImplicitlyDeclared)
                              .Select(w =>
                               {
                                   var isIndex = w.GetAttributes()
                                                  .Any(a => a.AttributeClass?.ToDisplayString() ==
                                                            typeof(IndexByAttribute).FullName);

                                   var type = w.Type;
                                   var fieldType = type.GetFieldType();
                                   var collectionType = ECollectionType.None;

                                   if (!fieldType.HasValue)
                                   {
                                       if (type is IArrayTypeSymbol arrayType)
                                       {
                                           collectionType = ECollectionType.Array;
                                           fieldType = arrayType.ElementType.GetFieldType();
                                       }
                                       else
                                           return null;
                                   }

                                   if (!fieldType.HasValue)
                                       return null;

                                   ComponentFieldDeclaration? result = new ComponentFieldDeclaration
                                   {
                                       Name = w.Name,
                                       CollectionType = collectionType,
                                       IsIndex = isIndex,
                                       Type = fieldType.Value,
                                   };

                                   return result;
                               })
                              .Where(str => str.HasValue)
                              .Select(str => str!.Value);

        return processedMembers;
    }

    public static IEnumerable<string> GetCodeFieldDeclarations(ImmutableArray<ISymbol> componentFields)
    {
        var processedMembers = GetFieldDeclarations(componentFields)
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