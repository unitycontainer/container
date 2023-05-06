using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Builder;

namespace Pipeline
{
    public class FaultedStrategy
    {
        public static readonly string PreName  = $"{nameof(FaultedStrategy)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(FaultedStrategy)}.{nameof(PostBuildUp)}";

        public void PreBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            context.Error("Error");
            ((IList<string>)context.Existing).Add(PreName);
        }

        public void PostBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            Assert.Fail();
        }

        public object Analyze<TContext>(ref TContext context)
            => new Exception(nameof(FaultedStrategy));
    }
}
