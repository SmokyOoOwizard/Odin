﻿using Odin.Core.Abstractions.Matchers.Impl;

namespace Odin.Core.Matchers.Extensions;

public static class ComponentMatcherComponentsExtensions
{
    public static ReactiveComponentMatcherBuilder Has<T>(this ReactiveComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder Has<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ReactiveComponentMatcherBuilder NotHas<T>(this ReactiveComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder NotHas<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
}