using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Extension;

namespace Pipeline
{
    public class FaultedStrategy : BuilderStrategy
    {
        public static readonly string PreName  = $"{nameof(FaultedStrategy)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(FaultedStrategy)}.{nameof(PostBuildUp)}";

        public override void PreBuildUp<TContext>(ref TContext context)
        {
            context.Error("Error");
            ((IList<string>)context.Existing).Add(PreName);
        }

        public override void PostBuildUp<TContext>(ref TContext context)
        {
            Assert.Fail();
        }

        public override object Analyse<TContext>(ref TContext context)
            => new Exception(nameof(FaultedStrategy));
    }
}
