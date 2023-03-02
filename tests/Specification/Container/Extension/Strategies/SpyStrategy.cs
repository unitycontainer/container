#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
#elif UNITY_V5 || UNITY_V6
using Unity.Builder;
using Unity.Strategies;
#else
using Unity.Extension;
#endif

namespace Regression.Container
{
    /// <summary>
    /// Syntax specific to Unity v6+
    /// </summary>
    public partial class SpyStrategy : BuilderStrategy
    {
        #region Fields

        private bool _called = false;
        private object _existing = null;

        #endregion


        #region Properties

        public object Existing => _existing;

        public object Target => _existing;

        public bool BuildUpWasCalled => _called;

        #endregion


        #region Syntax specific to Unity v4
        #if UNITY_V4

        public override void PreBuildUp(IBuilderContext context)
        {
            _called = true;
            _existing = context.Existing;

            SpyPolicy policy = context.Policies.Get<SpyPolicy>(context.BuildKey);
            
            // Mark the policy
            if (policy != null) policy.WasSpiedOn = true;
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            // Spy on created object
            _existing = context.Existing;
        }

        #endif
        #endregion


        #region Syntax specific to Unity v5
        #if UNITY_V5 || UNITY_V6

        public override void PreBuildUp(ref BuilderContext context)
        {
            _called = true;
            _existing = context.Existing;

            var policy = (SpyPolicy)context.Get(null, typeof(SpyPolicy));

            if (policy != null) policy.WasSpiedOn = true;
        }

        public override void PostBuildUp(ref BuilderContext context)
        {
            // Spy on created object
            _existing = context.Existing;
        }

        #endif
        #endregion


        #region Syntax specific to Unity v6
        #if !UNITY_V4 && !UNITY_V5 && !UNITY_V6


        public override void PreBuildUp<TContext>(ref TContext context)
        {
            _called = true;
            _existing = context.Existing;

#if BEHAVIOR_V5
            var policy = (SpyPolicy)context.Get(null, typeof(SpyPolicy));
#else
            var policy = context.Policies.GetDefault<SpyPolicy>();
#endif
            // Mark the policy
            if (policy != null) policy.WasSpiedOn = true;
        }


        public override void PostBuildUp<TContext>(ref TContext context)
        {
            // Spy on created object
            _existing = context.Existing;
        }

        #endif
        #endregion
    }
}
