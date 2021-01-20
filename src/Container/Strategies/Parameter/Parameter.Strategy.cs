﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo> : MemberStrategy<TMemberInfo, ParameterInfo, object[]> 
                                                                   
                                               where TMemberInfo : MethodBase
    {
        #region Fields

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];
        
        protected IImportProvider<ParameterInfo> ParameterProvider { get; private set; }

        #endregion


        #region Constructors

        /// <inheritdoc/>
        public ParameterStrategy(IPolicies policies)
            : base(policies)
        {
            ParameterProvider = policies.CompareExchange<IImportProvider<ParameterInfo>>(this, null, OnProviderChnaged) ?? this;
        }

        #endregion



        #region Implementation


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChnaged(Type? target, Type type, object? policy)
            => ParameterProvider = (IImportProvider<ParameterInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}