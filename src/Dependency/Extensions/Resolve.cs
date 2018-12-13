using System;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Resolve
    {
        #region Dependency

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Dependency(Type type) => new ResolvedParameter(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Dependency<TTarget>() => new ResolvedParameter(typeof(TTarget));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Dependency(Type type, string name) => new ResolvedParameter(type, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Dependency<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion



        #region Optional Dependency

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue OptionalDependency(Type type) => new OptionalParameter(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue OptionalDependency<TTarget>() => new OptionalParameter(typeof(TTarget));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue OptionalDependency(Type type, string name) => new OptionalParameter(type, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue OptionalDependency<TTarget>(string name) => new OptionalParameter(typeof(TTarget), name);

        #endregion



        #region Parameter

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Parameter(Type type) => new ResolvedParameter(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Parameter<TTarget>() => new ResolvedParameter(typeof(TTarget));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Parameter(Type type, string name) => new ResolvedParameter(type, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TypedInjectionValue Parameter<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Property

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Property(string name) => new InjectionProperty(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Property(Type type) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Property<TTarget>() => throw new NotImplementedException();

        #endregion
    }
}
