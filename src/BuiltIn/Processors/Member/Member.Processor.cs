using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
    {
        #region Constants

        /// <summary>
        /// Binding flags used to obtain declared members by default
        /// </summary>
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        #endregion


        #region Fields

        /// <summary>
        /// Combination of <see cref="BindingFlags"/> to use when getting declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults)
        {
            BindingFlags = defaults
                .GetOrAdd(typeof(TMemberInfo), DefaultBindingFlags, 
                    (object flags) => BindingFlags = (BindingFlags)flags);
        }

        #endregion


        #region Implementation

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TMemberInfo[] GetMembers(Type type);


        public abstract object? GetValue(ref DependencyInfo dependency, object? data);


        #endregion
    }
}
