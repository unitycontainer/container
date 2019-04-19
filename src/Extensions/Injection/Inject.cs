using System;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    public static partial class Inject
    {
        #region Array

        public static ParameterBase Array(Type elementType, params object[] elementValues) 
            => new ResolvedArrayParameter(elementType, elementValues);

        public static ParameterBase Array<TElement>(params object[] elementValues)
            => new ResolvedArrayParameter(typeof(TElement), elementValues);

        #endregion


        #region Parameter

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter(object value) => new InjectionParameter(value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter(Type type, object value) 
            => new InjectionParameter(type ?? throw new ArgumentNullException(nameof(type)), value);
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterBase Parameter<TTarget>(object value) => new InjectionParameter(typeof(TTarget), value);

        #endregion


        #region Field

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Field(string name, object value) 
            => new InjectionField(name ?? throw new ArgumentNullException(nameof(name)), value);
        
        #endregion


        #region Property

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InjectionMember Property(string name, object value) 
            => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)), value);
        
        #endregion
    }
}
