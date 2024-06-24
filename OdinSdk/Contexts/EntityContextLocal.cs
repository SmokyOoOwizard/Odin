using Odin.Abstractions.Entities;
using OdinSdk.Components;

namespace OdinSdk.Contexts;

public struct EntityContextLocal
{
    public IEntityComponentsRepository Changes { get; set; }
}