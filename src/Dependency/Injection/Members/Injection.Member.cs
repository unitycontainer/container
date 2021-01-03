using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for objects that can be used to configure what
    /// class members get injected by the container.
    /// </summary>
    public abstract class InjectionMember
    {
        /// <summary>
        /// This property triggers mandatory build if true
        /// </summary>
        public abstract bool BuildRequired { get; }
        
        /// <summary>
        /// Reference to the next member
        /// </summary>
        public InjectionMember? Next { get; internal set; }
    }

    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember, 
                                                                IMatch<TMemberInfo, MatchRank>,
                                                                IInjectionProvider
                                            where TMemberInfo : MemberInfo
                                            where TData       : class
    {
        #region Constructors

        protected InjectionMember(string name, TData data)
        {
            Name = name;
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


        /// <inheritdoc/>
        public abstract MatchRank Match(TMemberInfo other);

        /// <inheritdoc/>
        public abstract void GetImportInfo<TImport>(ref TImport import)
            where TImport : IInjectionInfo;

        #endregion
    }
}
