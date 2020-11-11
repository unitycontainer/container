using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for objects that can be used to configure what
    /// class members get injected by the container.
    /// </summary>
    public abstract class InjectionMember
    {
        /// <summary>
        /// Marker constant for Catch All name
        /// </summary>
        public const string AnyContractName = "Any Contract Name";

        /// <summary>
        /// This property triggers mandatory build if true
        /// </summary>
        public abstract bool BuildRequired { get; }
        
        /// <summary>
        /// Reference to the next member
        /// </summary>
        public InjectionMember? Next { get; internal set; }
    }

    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember, IMatch<TMemberInfo>,
                                                                IInjectionProvider
                                            where TMemberInfo : MemberInfo
                                            where TData       : class
    {
        #region Constructors

        protected InjectionMember(string name, TData data)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Data = data;
        }

        #endregion


        #region Public API

        /// <summary>
        /// Name of the injected member
        /// </summary>
        /// <remarks>
        /// <para> When injected member is either <see cref="InjectionMethod"/>, <see cref="InjectionField"/>,
        /// or <see cref="InjectionProperty"/> this is the name of respective method, field, or property.</para>
        /// <para>In case of <see cref="InjectionConstructor"/>, Name is always ".ctor"</para>
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Data associated with injection member.
        /// </summary>
        public virtual TData? Data { get; }

        /// <summary>
        /// Injecting any members requires pipeline build
        /// </summary>
        public override bool BuildRequired => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public virtual MatchRank Match(TMemberInfo other)
            => other.Name == Name ? MatchRank.ExactMatch : MatchRank.NoMatch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public virtual void GetImportInfo<TImport>(ref TImport import)
            where TImport : IInjectionInfo
        { }

        #endregion
    }
}
