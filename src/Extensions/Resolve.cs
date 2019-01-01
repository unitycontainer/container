using System;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Resolve
    {
        #region Dependency

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Dependency(Type type) => new ResolvedParameter(type);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Dependency<TTarget>() => new ResolvedParameter(typeof(TTarget));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Dependency(Type type, string name) => new ResolvedParameter(type, name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Dependency<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Optional Dependency

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue OptionalDependency(Type type) => new OptionalParameter(type);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue OptionalDependency<TTarget>() => new OptionalParameter(typeof(TTarget));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue OptionalDependency(Type type, string name) => new OptionalParameter(type, name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue OptionalDependency<TTarget>(string name) => new OptionalParameter(typeof(TTarget), name);

        #endregion


        #region Parameter

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Parameter(Type type) => new ResolvedParameter(type);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Parameter<TTarget>() => new ResolvedParameter(typeof(TTarget));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Parameter(Type type, string name) => new ResolvedParameter(type, name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypedInjectionValue Parameter<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Property

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property(string name) => new InjectionProperty(name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property(Type type) => throw new NotImplementedException();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property<TTarget>() => throw new NotImplementedException();

        #endregion
    }
}
