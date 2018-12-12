using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Inject
    {
        #region Parameter

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter(object value) => new InjectionParameter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter(Type type, object value) => new InjectionParameter(type, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter<TTarget>(object value) => new InjectionParameter(typeof(TTarget), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter(string name, object value) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter(Type type, string name, object value) => throw new NotImplementedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionParameter Parameter<TTarget>(string name, object value) => throw new NotImplementedException();

        #endregion


        #region Dependency

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(Type type, object value) => new InjectionDependency(type, value);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency<TTarget>(object value)   => new InjectionDependency(typeof(TTarget), value);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(string name, object value) => new InjectionDependency(name, value);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency(Type type, string name, object value) => new InjectionDependency(type, name, value);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Dependency<TTarget>(string name, object value)   => new InjectionDependency(typeof(TTarget), name, value);

        #endregion


        #region Property

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Property(string name, object value) => new InjectionProperty(name, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Property(Type type, object value) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Property<TTarget>(object value) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static InjectionMember Property(PropertyInfo info, object value) => throw new NotImplementedException();

        #endregion
    }
}
