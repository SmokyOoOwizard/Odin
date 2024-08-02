using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Odin.Core.Abstractions.Components.Attributes;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.CodeGen.Abstractions.Utils;

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
                                           if (arrayType.Rank > 1)
                                               return null;
                                           
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
}