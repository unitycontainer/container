using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
                                                                 where TData       : class
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

        /// <summary>
        /// Returns attribute the info is annotated with
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> member</param>
        /// <returns>Attribute or null if nothing found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual ImportAttribute? GetImportAttribute(TMemberInfo info) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual InjectionMember<TMemberInfo, TData>? GetInjected(RegistrationManager? registration) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();


        #endregion


        protected abstract DependencyInfo<TDependency> GetDependencyInfo(TDependency member);

        protected abstract DependencyInfo<TDependency> GetDependencyInfo(TDependency member, object? data);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

    }
}
