using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
                                                                 where TData       : class
    {
        #region Fields

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        protected SupportedMembers<TMemberInfo> GetSupportedMembers;

        /// <summary>
        /// Function to load <see cref="ImportInfo{TMember}"/> with data from current <see cref="ParameterInfo"/>,
        /// <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> and all supported attributes.
        /// </summary>
        protected ImportProvider<ImportInfo, ImportType> LoadImportInfo { get; private set; }

        #endregion


        #region Constructors

        protected MemberProcessor(Defaults defaults)
        {
            GetSupportedMembers = defaults.Subscribe<SupportedMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            LoadImportInfo = defaults.Subscribe<TDependency, ImportProvider<ImportInfo, ImportType>>(OnImportInfoLoaderChanged)!;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();

        protected ResolverOverride? GetOverride(in PipelineContext context, in ImportInfo import)
        {
            var length = context.Overrides.Length;
            if (0 == length--) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = length; index >= 0; --index)
            {
                var @override = context.Overrides[index];

                // Match member first
                if (@override is IMatch<TDependency> candidate)
                {
                    rank = candidate.Match(import.MemberInfo);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }

                    continue;
                }

                if (@override is IMatchImport dependency)
                {
                    rank = dependency.MatchImport(in import);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
                return candidateOverride;

            return null;
        }

        protected ResolverOverride? GetOverride<TContext>(ref TContext context, in ImportInfo import)
            where TContext : IBuilderContext
        {
            var length = context.Overrides.Length;
            if (0 == length--) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = length; index >= 0; --index)
            {
                var @override = context.Overrides[index];

                // Match member first
                if (@override is IMatch<TDependency> candidate)
                {
                    rank = candidate.Match(import.MemberInfo);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }

                    continue;
                }

                if (@override is IMatchImport dependency)
                {
                    rank = dependency.MatchImport(in import);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
                return candidateOverride;

            return null;
        }

        private void OnMembersSelectorChanged(Type? target, Type type, object? policy)
            => GetSupportedMembers = (SupportedMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));
        
        private void OnImportInfoLoaderChanged(Type? target, Type type, object? policy)
            => LoadImportInfo = (ImportProvider<ImportInfo, ImportType>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
