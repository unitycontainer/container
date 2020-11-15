using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        protected Func<Type, TMemberInfo[]> GetMembers;

        /// <summary>
        /// Function to load <see cref="ImportInfo{TMember}"/> with data from current <see cref="ParameterInfo"/>,
        /// <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> and all supported attributes.
        /// </summary>
        protected ImportProvider<ImportInfo, ImportType> LoadImportInfo { get; private set; }

        #endregion


        #region Constructors

        protected MemberProcessor(Defaults defaults, Func<Type, TMemberInfo[]> members,
                                        ImportProvider<ImportInfo, ImportType> loader)
        {

            GetMembers = defaults.GetOrAdd(typeof(TDependency), members,
                (object policy) => GetMembers = (Func<Type, TMemberInfo[]>)policy);

            LoadImportInfo = defaults.GetOrAdd(typeof(TDependency), loader,
                (object policy) => LoadImportInfo = (ImportProvider<ImportInfo, ImportType>)policy);
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TMember? GetInjectedMembers<TMember>(RegistrationManager? registration) where TMember : class => null;

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

        #endregion
    }
}
