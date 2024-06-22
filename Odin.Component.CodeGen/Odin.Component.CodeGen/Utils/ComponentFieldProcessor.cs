using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Declaration;

namespace Odin.Component.CodeGen.Utils;

public static class ComponentFieldProcessor
{
    public static IEnumerable<string> GetFieldDeclarations(ImmutableArray<ISymbol> componentFields)
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
                                       else if (type is INamedTypeSymbol namedType)
                                       {
                                           var listName = typeof(List<>).FullName?.Replace("`1", "<T>");

                                           var definition = namedType.OriginalDefinition.ToDisplayString();

                                           if (definition == listName)
                                           {
                                               collectionType = ECollectionType.Array;
                                               fieldType = namedType.TypeArguments.First().GetFieldType();
                                           }
                                           else if (type.TypeKind == TypeKind.Struct)
                                               return null;
                                           else
                                               return null;
                                       }
                                       else
                                           return null;
                                   }

                                   var fieldDeclaration = new[]
                                   {
                                       $".AddField(\"{w.Name}\")",
                                       isIndex ? ".Index()" : "",
                                       $".Type({nameof(EFieldType)}.{fieldType})",
                                       collectionType != ECollectionType.None
                                           ? $".Collection({nameof(ECollectionType)}.{collectionType})"
                                           : "",
                                   }.Where(str => !string.IsNullOrWhiteSpace(str));

                                   return string.Join("\n\t\t\t\t", fieldDeclaration);
                               })
                              .Where(str => !string.IsNullOrWhiteSpace(str))
                              .Select(str => str!);

        return processedMembers;
    }
}