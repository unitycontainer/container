using System;
using System.Collections;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Defaults
    {
        #region Fields

        readonly object _syncRoot = new object();

        protected int Count;
        protected int Prime = 5;

        [CLSCompliant(false)] protected Policy[] Data;
        [CLSCompliant(false)] protected Metadata[] Meta;

        private readonly int BUILD_PIPELINE;
        private readonly int BUILD_PIPELINE_TYPE;
        private readonly int BUILD_PIPELINE_FACTORY;
        private readonly int BUILD_PIPELINE_INSTANCE;

        private readonly int TO_ARRAY;
        private readonly int TO_ENUMERATION;

        #endregion


        #region Constructors

        internal Defaults()
        {
            // Build Chains
            TypeChain = new StagedChain<BuildStage, PipelineProcessor>();
            FactoryChain = new StagedChain<BuildStage, PipelineProcessor>();
            InstanceChain = new StagedChain<BuildStage, PipelineProcessor>();
            UnregisteredChain = new StagedChain<BuildStage, PipelineProcessor>();

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            BUILD_PIPELINE          = Allocate(typeof(PipelineFactory));
            BUILD_PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(PipelineFactory));
            BUILD_PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(PipelineFactory));
            BUILD_PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(PipelineFactory));

            // Enumerators
            TO_ARRAY       = Allocate(typeof(Array),       typeof(Func<Scope, Type[], Metadata[]>));
            TO_ENUMERATION = Allocate(typeof(IEnumerable), typeof(Func<Scope, Type[], Metadata[]>));
        }

        #endregion
    }
}
