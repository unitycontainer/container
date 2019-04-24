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
        public static ParameterBase Dependency<TTarget>() => new ResolvedParameter(typeof(TTarget), null);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Dependency<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Parameter

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter() => new ResolvedParameter();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter(string name) => new ResolvedParameter(name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter(Type type) => new ResolvedParameter(type);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter<TTarget>() => new ResolvedParameter(typeof(TTarget));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter(Type type, string name) => new ResolvedParameter(type, name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Generic

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static GenericParameter Generic(string genericParameterName) => new GenericParameter(genericParameterName);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static GenericParameter Generic(string genericParameterName, string registrationName) => new GenericParameter(genericParameterName, registrationName);

        #endregion


        #region Optional

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional() => new OptionalParameter();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional(string name) => new OptionalParameter(name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional(Type type) => new OptionalParameter(type);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional<TTarget>() => new OptionalParameter(typeof(TTarget));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional(Type type, string name) => new OptionalParameter(type, name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Optional<TTarget>(string name) => new OptionalParameter(typeof(TTarget), name);

        #endregion


        #region Field

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Field(string name) => new InjectionField(name);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember OptionalField(string name) => new InjectionField(name, ResolutionOption.Optional);

        #endregion


        #region Property

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property(string name) => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember OptionalProperty(string name) => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)), ResolutionOption.Optional);

        #endregion
    }
}
