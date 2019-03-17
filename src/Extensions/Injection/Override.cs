using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    public static partial class Override
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Property(string name, object value) => new PropertyOverride(name, value);


        #region Field

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Field(string name, object value) => new FieldOverride(name, value);

        #endregion


        #region Parameter

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter(object value)
            => new ParameterOverride(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), value);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter(string name, object value)
            => new ParameterOverride(name, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter(Type type, object value)
            => new ParameterOverride(type, value);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter(Type type, string name, object value)
            => new ParameterOverride(type, name, value);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter<TType>(object value)
            => new ParameterOverride(typeof(TType), value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter<TType>(string name, object value)
            => Parameter(typeof(TType), name, value);

        #endregion


        #region Dependency

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency(object value)
            => Dependency(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), null, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency(string name, object value)
            => Dependency(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), name, value);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency(Type type, object value)
        {
            return new DependencyOverride(type, value);
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency(Type type, string name, object value)
        {
            return new DependencyOverride(type, name, value);
        }


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency<TType>(object value)
            => new DependencyOverride(typeof(TType), value);

        #if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency<TType>(string name, object value) 
            => new DependencyOverride(typeof(TType), name, value);

        #endregion
    }
}
