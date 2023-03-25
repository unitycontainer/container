﻿using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    public partial class Policies<TContext> where TContext : IBuilderContext
    {
        #region Fields

        protected int Count;
        
        [CLSCompliant(false)] 
        protected Policy[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CLSCompliant(false)] 
        protected Metadata[] Meta;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        protected readonly object SyncRoot = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        protected int Prime = 3;

        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _strategiesChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _buildPlanChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _instanceChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _factoryChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _mappingChain;

        #endregion


        #region Constructors

        internal Policies()
        {
            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Build Chains
            _strategiesChain = new StagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>();

            // Setup build on change for the chains
            StrategiesChain.Invalidated += OnTypeChainChanged;

            // Resolve Unregistered Type
            Allocate<ResolveDelegate<TContext>>(OnResolveUnregisteredChanged);

            // Resolve Registered Type
            Allocate<ResolveDelegate<TContext>>(typeof(ContainerRegistration), OnResolveRegisteredChanged);

            // Resolve Array
            Allocate<ResolveDelegate<TContext>>(typeof(Array), OnResolveArrayChanged);
        }

        #endregion
    }
}
