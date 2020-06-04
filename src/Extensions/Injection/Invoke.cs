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

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Ctor() => new InjectionConstructor();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Ctor(params object[] parameters) => new InjectionConstructor(parameters);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Ctor(params Type[] parameters) => new InjectionConstructor(parameters);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Ctor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Constructor

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Constructor() => new InjectionConstructor();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Constructor(params object[] parameters) => new InjectionConstructor(parameters);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Constructor(params Type[] parameters) => new InjectionConstructor(parameters);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Constructor(ConstructorInfo info, params object[] parameters) => new InjectionConstructor(info, parameters);

        #endregion


        #region Method

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Method(string name, params object[] parameters) => new InjectionMethod(name, parameters);

        #endregion
    }
}
