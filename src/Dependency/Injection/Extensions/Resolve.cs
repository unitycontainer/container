using System;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This extension allows more concise notation while registering
    /// parameters with Unity Container
    /// </summary>
    public static partial class Resolve
    {
        #region Dependency

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Dependency<TTarget>() => new ResolvedParameter(typeof(TTarget), null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Dependency<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Parameter

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter() => new ResolvedParameter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter(string name) => new ResolvedParameter(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter(Type type) => new ResolvedParameter(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter<TTarget>() => new ResolvedParameter(typeof(TTarget));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter(Type type, string name) => new ResolvedParameter(type, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Parameter<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Generic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GenericParameter Generic(string genericParameterName) => new GenericParameter(genericParameterName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GenericParameter Generic(string genericParameterName, string registrationName) => new GenericParameter(genericParameterName, registrationName);

        #endregion


        #region Optional

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional() => new OptionalParameter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional(string name) => new OptionalParameter(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional(Type type) => new OptionalParameter(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional<TTarget>() => new OptionalParameter(typeof(TTarget));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional(Type type, string name) => new OptionalParameter(type, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterBase Optional<TTarget>(string name) => new OptionalParameter(typeof(TTarget), name);

        #endregion


        #region Field

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Field(string field) => new InjectionField(field);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember OptionalField(string field) => new OptionalField(field);

        #endregion


        #region Property

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember Property(string property) => new InjectionProperty(property);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InjectionMember OptionalProperty(string property) => new OptionalProperty(property);

        #endregion
    }
}
