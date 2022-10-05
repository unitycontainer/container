﻿using System;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        #region Fields

        private Delegate? _pipeline;

        #endregion


        #region Pipeline

        public virtual ResolveDelegate<TContext>? GetPipeline<TContext>(Scope scope) 
            where TContext : IBuilderContext
            => (ResolveDelegate<TContext>?)_pipeline;

        public virtual ResolveDelegate<TContext>? SetPipeline<TContext>(Scope scope, ResolveDelegate<TContext>? pipeline)
            where TContext : IBuilderContext
        {
            _pipeline = pipeline;

            return pipeline;
        }

        #endregion
    }
}