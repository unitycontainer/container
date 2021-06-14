using System;
#if UNITY_V4
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#elif UNITY_V5
using Unity.Strategies;
using Unity.Builder;
using Unity.Extension;
#else
using Unity.Extension;
using Unity;
#endif

#pragma warning disable CS0618 // Type or member is obsolete
namespace Regression.Container
{
    /// <summary>
    /// A simple extension that puts the supplied strategy into the
    /// chain at the indicated stage.
    /// </summary>
    public partial class SpyExtension : UnityContainerExtension,
                                        IUnityContainerExtensionConfigurator
    {
        private Type _policyType;
        private object _policy;
        private BuilderStrategy _strategy;
        private UnityBuildStage _stage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="stage"></param>
        public SpyExtension(BuilderStrategy strategy, UnityBuildStage stage)
        {
            _strategy = strategy;
            _stage = stage;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strategy">The strategy to add</param>
        /// <param name="stage">Stage, or step to add it to</param>
        /// <param name="policy">
        /// The policy to add.
        /// <para>
        /// Unity v4 required a policy to be derived from <see cref="IBuilderPolicy"/>
        /// </para>
        /// <para>
        /// Unity v5 deprecated the interface and relaxed the requirement. Now any object
        /// implementing <paramref name="policyType"/> could be added as a policy.
        /// </para>
        /// </param>
        /// <param name="policyType"><see cref="Type"/> of the policy</param>
        public SpyExtension(BuilderStrategy strategy, UnityBuildStage stage, object policy, Type policyType)
        {
            _strategy = strategy;
            _stage = stage;
            _policy = policy;
            _policyType = policyType;
        }

        public ExtensionContext ExtensionContext => Context;

        // Different variants and versions of Initialize

        #region Unity v4
        #if UNITY_V4

        /// <summary>
        /// A simple extension that puts the supplied strategy into the
        /// chain at the indicated stage.
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.Add(_strategy, _stage);

            if (_policy != null)
                Context.Policies.SetDefault(_policyType, (IBuilderPolicy)_policy);
        }

        #endif
        #endregion


        #region Unity v5 
        #if UNITY_V5

        protected override void Initialize()
        {
            Context.Strategies.Add(this._strategy, this._stage);
            
            if (_policy != null)
                Context.Policies.Set(null, _policyType, _policy);
        }

        #endif
        #endregion


        #region Unity v6+
        #if !UNITY_V4 && !UNITY_V5

        protected override void Initialize()
        {
#if BEHAVIOR_V4 || BEHAVIOR_V5
            
            // v4 & v5 syntax
            Context.Strategies.Add(_strategy, _stage);
#else
            Context.TypePipelineChain.Add(_stage, _strategy);
#endif

            // Add Spy Policy to storage
            if (_policy is not null)
            {
#if BEHAVIOR_V4
                // v4 syntax
                Context.Policies.SetDefault(_policyType, _policy);
#elif BEHAVIOR_V5
                // v5 syntax
                Context.Policies.Set(null, _policyType, _policy);
#else
                Context.Policies.Set(_policyType, _policy);
#endif
            }
        }

        #endif
        #endregion
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
