using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Invoke
    {
        #region Factory

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Factory(Func<IUnityContainer, object> factoryFunc) => new InjectionFactory(factoryFunc);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Factory(Func<IUnityContainer, Type, string, object> factoryFunc) => new InjectionFactory(factoryFunc);

        #endregion


        #region Ctor

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Ctor() => new InjectionConstructor();
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Ctor(params object[] parameters) => new InjectionConstructor(parameters);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Ctor(params Type[] parameters) => new InjectionConstructor(parameters);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Ctor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Constructor

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Constructor() => new InjectionConstructor();
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Constructor(params object[] parameters) => new InjectionConstructor(parameters);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Constructor(params Type[] parameters) => new InjectionConstructor(parameters);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Constructor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Method

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Method(string name) => new InjectionMethod(name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Method(string name, params object[] parameters) => new InjectionMethod(name, parameters);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IInjectionMember Method(MethodInfo info, params object[] parameters) => throw new NotImplementedException();

        #endregion
    }
}
