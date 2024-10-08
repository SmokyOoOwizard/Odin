﻿using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Core.Tests.Contexts;

public class InMemoryContextCollectionFilterTests : AEntityCollectionFilterTests
{
    public InMemoryContextCollectionFilterTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectionFilterTests)))
    {
    }
}