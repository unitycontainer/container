using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// This extension allows more concise notation while overriding 
    /// dependencies during resolution
    /// </summary>
    public static partial class Override
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Property(string name, object value) => new PropertyOverride(name, value);


        #region Field

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Field(string name, object value) => new FieldOverride(name, value);

        #endregion


        #region Parameter

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter(object value)
            => new ParameterOverride(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter(string name, object value)
            => new ParameterOverride(name, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter(Type type, object value)
            => new ParameterOverride(type, value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter(string? name, Type type, object value)
            => new ParameterOverride(name, type, value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter<TType>(object value)
            => new ParameterOverride(typeof(TType), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Parameter<TType>(string name, object value)
            => Parameter(name, typeof(TType), value);

        #endregion


        #region Dependency

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency(object value)
            => Dependency(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), null, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency(string name, object value)
            => Dependency(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), name, value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency(Type type, object value)
        {
            return new DependencyOverride(type, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency(Type type, string? name, object value)
        {
            return new DependencyOverride(type, name, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency<TType>(object value)
            => new DependencyOverride(typeof(TType), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolverOverride Dependency<TType>(string name, object value) 
            => new DependencyOverride(typeof(TType), name, value);

        #endregion
    }
}
