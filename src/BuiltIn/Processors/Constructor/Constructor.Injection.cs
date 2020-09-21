using System;
using System.Reflection;
using Unity.Container;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        protected virtual object? FromInjectedCtor(ref PipelineBuilder<object?> builder, ConstructorInfo[] constructors)
        {
            return default;
        }

        protected virtual Pipeline? FromInjectedCtor(ref PipelineBuilder<Pipeline?> builder, ConstructorInfo[] constructors)
        {
            return default;
        }
    }
}
