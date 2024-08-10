namespace Odin.Core.Components.Declaration;

public interface IComponentDeclarations
{
    bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration);
}