using System;
using System.Reflection;
using Unity.Composition;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;

namespace Unity.Storage
{
    public class DefaultPolicies : PolicySet
    {
        #region Fields

        private StagedStrategyChain<PipelineBuilder, PipelineStage> _typeStages;
        private StagedStrategyChain<PipelineBuilder, PipelineStage> _factoryStages;
        private StagedStrategyChain<PipelineBuilder, PipelineStage> _instanceStages;

        #endregion


        #region Constructors

        // Non-nullable field is uninitialized.
        #pragma warning disable CS8618 

        public DefaultPolicies(UnityContainer owner)
            : base(owner)
        {
        }


        #pragma warning restore CS8618
        #endregion


        #region Pipelines

        public PipelineBuilder[] TypePipeline { get; private set; }
        public StagedStrategyChain<PipelineBuilder, PipelineStage> TypeStages
        {
            get => _typeStages;
            set
            {
                _typeStages = value;
                _typeStages.Invalidated += (s, e) => TypePipeline = _typeStages.ToArray();

                TypePipeline = _typeStages.ToArray();
            }
        }

        public PipelineBuilder[] FactoryPipeline { get; private set; }
        public StagedStrategyChain<PipelineBuilder, PipelineStage> FactoryStages
        {
            get => _factoryStages;
            set
            {
                _factoryStages = value;
                _factoryStages.Invalidated += (s, e) => FactoryPipeline = _factoryStages.ToArray();

                FactoryPipeline = _factoryStages.ToArray();
            }
        }

        public PipelineBuilder[] InstancePipeline { get; private set; }
        public StagedStrategyChain<PipelineBuilder, PipelineStage> InstanceStages
        {
            get => _instanceStages;
            set
            {
                _instanceStages = value;
                _instanceStages.Invalidated += (s, e) => InstancePipeline = _instanceStages.ToArray();

                InstancePipeline = _instanceStages.ToArray();
            }
        }

        #endregion


        #region Default Lifetime

        public ITypeLifetimeManager TypeLifetimeManager { get; set; }

        public IFactoryLifetimeManager FactoryLifetimeManager { get; set; }

        public IInstanceLifetimeManager InstanceLifetimeManager { get; set; }

        #endregion



        #region Public Members

        public CompositionDelegate ComposeMethod { get; set; }

        public ISelect<ConstructorInfo> CtorSelector { get; set; }

        public ISelect<PropertyInfo> PropertiesSelector { get; set; }

        public ISelect<MethodInfo> MethodsSelector { get; set; }

        public ISelect<FieldInfo> FieldsSelector { get; set; }

        #endregion


        #region PolicySet

        public override object? Get(Type policyInterface)
        {
            return policyInterface switch
            {
                Type type when typeof(ISelect<ConstructorInfo>) == type => CtorSelector,
                Type type when typeof(ISelect<PropertyInfo>) == type => PropertiesSelector,
                Type type when typeof(ISelect<MethodInfo>) == type => MethodsSelector,
                Type type when typeof(ISelect<FieldInfo>) == type => FieldsSelector,
                Type type when typeof(CompositionDelegate) == type => ComposeMethod,

                _ => base.Get(policyInterface)
            };
        }

        public override void Set(Type policyInterface, object policy)
        {
            switch (policyInterface)
            {
                case Type type when typeof(ISelect<ConstructorInfo>) == type:
                    CtorSelector = (ISelect<ConstructorInfo>)policy;
                    break;

                case Type type when typeof(ISelect<PropertyInfo>) == type:
                    PropertiesSelector = (ISelect<PropertyInfo>)policy;
                    break;

                case Type type when typeof(ISelect<MethodInfo>) == type:
                    MethodsSelector = (ISelect<MethodInfo>)policy;
                    break;

                case Type type when typeof(ISelect<FieldInfo>) == type:
                    FieldsSelector = (ISelect<FieldInfo>)policy;
                    break;

                case Type type when typeof(CompositionDelegate) == type:
                    ComposeMethod = (CompositionDelegate)policy;
                    break;

                default:
                    base.Set(policyInterface, policy);
                    break;
            }
        }

        #endregion
    }
}
