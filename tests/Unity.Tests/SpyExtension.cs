using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Builder;
using Unity.Extension;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity.Tests
{
    public class SpyExtension : UnityContainerExtension
    {
        private PipelineStage _stage;
        private object _policy;
        private Type _policyType;

        public SpyExtension(PipelineBuilder builder, PipelineStage stage)
        {
            PipelineBuilder = builder;
            this._stage = stage;
        }

        public SpyExtension(PipelineBuilder builder, PipelineStage stage, object policy, Type policyType)
        {
            PipelineBuilder = builder;
            this._stage = stage;
            this._policy = policy;
            this._policyType = policyType;
        }

        protected override void Initialize()
        {
            Context.TypePipeline.Add(PipelineBuilder, _stage);
            Context.FactoryPipeline.Add(PipelineBuilder, _stage);
            Context.InstancePipeline.Add(PipelineBuilder, _stage);

            if (_policy != null)
            {
                Context.Policies.Set(null, null, _policyType, _policy);
            }
        }

        public PipelineBuilder PipelineBuilder { get; }
    }


    // A test strategy that introduces a variable delay in
    // the strategy chain to work out 
    public class DelayStrategy : PipelineBuilder
    {
        private int delayMS = 500;

        public override ResolveDelegate<BuilderContext> Build(ref PipelineContext builder)
        {
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => c.Existing);

            return (ref BuilderContext context) => 
            {
                delayMS = delayMS == 0 ? 500 : 0;
                Thread.Sleep(delayMS);

                return pipeline(ref context);
            };
        }
    }

    // Another test strategy that throws an exception the
    // first time it is executed.
    public class ThrowingStrategy : PipelineBuilder
    {
        private bool shouldThrow = true;

        public override ResolveDelegate<BuilderContext> Build(ref PipelineContext builder)
        {
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => c.Existing);

            return (ref BuilderContext context) =>
            {
                if (shouldThrow)
                {
                    shouldThrow = false;
                    throw new Exception("Throwing from buildup chain");
                }

                return pipeline(ref context);
            };
        }
    }

    public class SpyStrategy : PipelineBuilder
    {
        public Dictionary<(Type, string), int> BuildUpCallCount { get; private set; } = new Dictionary<(Type, string), int>(); 

        public override ResolveDelegate<BuilderContext> Build(ref PipelineContext builder)
        {

            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => c.Existing);

            return (ref BuilderContext context) =>
            {
                Type = context.Type;
                BuildUpWasCalled = true;
                Existing = context.Existing;
                UpdateBuildUpCallCount(context.Type, context.Name);
                UpdateSpyPolicy(ref context);
                Existing = pipeline(ref context);
                return Existing;
            };
        }

        public Type Type { get; private set; }

        public object Existing { get; private set; } = null;

        private void UpdateBuildUpCallCount(Type type, string name)
        {
            var tuple = (type, name);

            if (!BuildUpCallCount.ContainsKey(tuple))
            {
                BuildUpCallCount[tuple] = 1;
                return;
            }

            BuildUpCallCount[tuple]++;
        }

        public bool BuildUpWasCalled { get; private set; } = false;

        private void UpdateSpyPolicy(ref BuilderContext context)
        {
            SpyPolicy policy = (SpyPolicy)context.Get(null, null, typeof(SpyPolicy));

            if (policy != null)
            {
                policy.WasSpiedOn = true;
                policy.Count += 1;
            }
        }
    }


    public class SpyPolicy
    {
        public int Count;

        public bool WasSpiedOn { get; set; }
    }
}
