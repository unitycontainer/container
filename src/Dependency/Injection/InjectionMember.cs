using System;
using System.Reflection;

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

    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember
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

        public virtual int SelectFrom(TMemberInfo[] members)
        {
            for (var index = 0; index < members.Length; index++)
            {
                if (members[index].Name == Name) return index;
            }

            return -1;
        }

        #endregion


        #region Overrides

        public override bool BuildRequired => true;

        #endregion
    }
}
