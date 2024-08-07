﻿using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Matchers;
using Odin.Core.Matchers.Extensions;
using Odin.Core.Repositories.Matchers.Impl;

namespace Odin.Core.Tests;

public class MatchersRepositoryTests
{
    public struct TestComponent : IComponent
    {
        public ulong TestData;
    }
    
    public class TestMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<TestComponent>();
        }
    }
    
    [Fact]
    public void MatcherExists()
    {
        var id = MatchersRepository.GetMatcherId<TestMatcher>();
        
        Assert.NotEqual(0ul, id);
    }
    
    [Fact]
    public void MatcherJsonExists()
    {
        var id = MatchersRepository.GetMatcherId<TestMatcher>();
        var json = MatchersRepository.GetMatcherJson(id);
        
        Assert.NotEmpty(json);
    }
    
    [Fact]
    public void MatcherFilterExists()
    {
        var id = MatchersRepository.GetMatcherId<TestMatcher>();
        var filter = MatchersRepository.GetFilter(id);

        Assert.NotNull(filter);
    }
}