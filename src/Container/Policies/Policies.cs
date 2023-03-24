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

        #endregion


        #region Constructors

        internal Policies()
        {
            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Build Chains
            TypeChain     = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();
            FactoryChain  = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();
            InstanceChain = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();
            MappingChain  = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();

            // Setup build on change for the chains
            TypeChain.Invalidated     += OnTypeChainChanged;
            FactoryChain.Invalidated  += (s, e) => FactoryPipeline  = RebuildPipeline(s);
            InstanceChain.Invalidated += (s, e) => InstancePipeline = RebuildPipeline(s);
            MappingChain.Invalidated  += (s, e) => MappingPipeline  = RebuildPipeline(s);

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