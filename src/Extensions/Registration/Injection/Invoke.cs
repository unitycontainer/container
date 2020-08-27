using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This extension allows more concise notation while registering
    /// types with Unity Container
    /// </summary>
    public static partial class Invoke
    {
        #region Ctor

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Ctor() => new InjectionConstructor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Ctor(params object[] parameters) => new InjectionConstructor(parameters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Ctor(params Type[] parameters) => new InjectionConstructor(parameters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Ctor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Constructor

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Constructor() => new InjectionConstructor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Constructor(params object[] parameters) => new InjectionConstructor(parameters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Constructor(params Type[] parameters) => new InjectionConstructor(parameters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Constructor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Method

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Method(string name, params object[] parameters) => new InjectionMethod(name, parameters);

        #endregion
    }
}
