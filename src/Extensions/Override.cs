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

        public static ResolverOverride Field(string name, object value) => new FieldOverride(name, value);

        #endregion


        #region Parameter

        public static ResolverOverride Parameter<TType>(object value)
            => Parameter(typeof(TType), value);
        public static ResolverOverride Parameter<TType>(string name, object value)
            => Parameter(typeof(TType), name, value);

        public static ResolverOverride Parameter(Type type, object value) 
            => new ParameterOverride(type, value);

        public static ResolverOverride Parameter(Type type, string name, object value) 
            => new ParameterOverride(type, name, value);

        public static ResolverOverride Parameter(string name, object value) 
            => new ParameterOverride(name, value);

        #endregion



        #region Dependency

        public static ResolverOverride Dependency(string name, object value) 
            => Dependency(value?.GetType() ?? throw new ArgumentNullException(nameof(value)), name, value);

        public static ResolverOverride Dependency<TType>(string name, object value) 
            => Dependency(typeof(TType), name, value);

        public static ResolverOverride Dependency(Type type, string name, object value)
        {
            return new DependencyOverride(type, name, value);
        }

        #endregion
    }
}
