﻿using System;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);


    public partial class Defaults
    {
        #region Fields

        readonly object _syncRoot = new object();

        protected int _count;
        protected int _prime = 2;
        protected Policy[] _data;
        protected Metadata[] _meta;

        #endregion


        #region Constructors

        internal Defaults() 
        {
            // Build Chains
            TypePipeline         = new StagedChain<BuilderStage, PipelineProcessor>();
            FactoryPipeline      = new StagedChain<BuilderStage, PipelineProcessor>();
            InstancePipeline     = new StagedChain<BuilderStage, PipelineProcessor>();
            UnregisteredPipeline = new StagedChain<BuilderStage, PipelineProcessor>();

            // Storage
            _data = new Policy[Prime.Numbers[_prime]];
            _meta = new Metadata[Prime.Numbers[++_prime]];

            // Resolvers
            TypeResolver = DummyResolver;
            FactoryResolver = DummyResolver;
            InstanceResolver = DummyResolver;
            UnregisteredFactory = DummyResolver;
        }

        #endregion


        #region Events

        /// <summary>
        /// Event fired when one of the default policies has changed
        /// </summary>
        public DefaultPolicyChangedHandler? DefaultPolicyChanged;

        #endregion
    }
}
