using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

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

        public static int CompareTo(object[]? data, MethodBase? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)Resolution.Matching.MatchTo(data![i], parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }

        protected override void Execute<TContext, TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            => descriptor.MemberInfo.Invoke(context.Existing, (object[]?)data.Value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChnaged(Type? target, Type type, object? policy)
            => ParameterProvider = (IImportProvider<ParameterInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
