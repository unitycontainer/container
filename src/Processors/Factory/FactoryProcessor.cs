using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class FactoryProcessor : PipelineProcessor
    {
        public override IEnumerable<Expression> GetExpressions(Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? GetResolver(Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed)
        {
            return seed;
        }
    }
}
