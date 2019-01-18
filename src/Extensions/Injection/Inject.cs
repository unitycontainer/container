using System;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Inject
    {
        #region Parameter

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionParameter Parameter(object value) => new InjectionParameter(value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionParameter Parameter(Type type, object value) 
            => new InjectionParameter(type ?? throw new ArgumentNullException(nameof(type)), value);
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionParameter Parameter<TTarget>(object value) => new InjectionParameter(typeof(TTarget), value);

        #endregion


        #region Dependency

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(Type type, object value) => new InjectionDependency(type, value);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency<TTarget>(object value)   => new InjectionDependency(typeof(TTarget), value);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(string name, object value) => new InjectionDependency(name, value);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(Type type, string name, object value) => new InjectionDependency(type, name, value);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency<TTarget>(string name, object value)   => new InjectionDependency(typeof(TTarget), name, value);

        #endregion


        #region Field

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Field(string name, object value) 
            => new InjectionField(name ?? throw new ArgumentNullException(nameof(name)), value);
        #endregion


        #region Property

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property(string name, object value) 
            => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)), value);
        #endregion
    }
}
