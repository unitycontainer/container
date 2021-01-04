using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData> : BuilderStrategy
                                                                where TMemberInfo : MemberInfo
                                                                where TDependency : class
                                                                where TData       : class
    {
        #region Fields

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        protected DeclaredMembers<TMemberInfo> GetDeclaredMembers;

        protected ImportDescriptionProvider<TDependency, ImportInfo> DescribeImport { get; set; }

        #endregion


        #region Constructors

        protected MemberStrategy(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<TMemberInfo, DeclaredMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            DescribeImport     = policies.Get<ImportDescriptionProvider<TDependency, ImportInfo>>(OnImportProviderChanged)!;
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
                if (@override is IMatch<TDependency, MatchRank> candidate)
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
                if (@override is IMatch<TDependency, MatchRank> candidate)
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
            => GetDeclaredMembers = (DeclaredMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        private void OnImportProviderChanged(Type? target, Type type, object? policy)
        {
            DescribeImport = (ImportDescriptionProvider<TDependency, ImportInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));
        }

        #endregion
    }
}
