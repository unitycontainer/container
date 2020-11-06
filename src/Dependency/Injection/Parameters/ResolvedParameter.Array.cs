using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    [DebuggerDisplay("ResolvedArrayParameter: Type={ParameterType.Name}")]
    public class ResolvedArrayParameter : ParameterBase
    {
        #region Fields

        private readonly static TypeInfo TypeInfo = typeof(ResolvedArrayParameter).GetTypeInfo();
        private static MethodInfo? TranslateMethod;
        private static MethodInfo? ResolveMethod;

        private readonly object? _values;
        private readonly ResolveDelegate<PipelineContext>? _resolver;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type and collection of element values.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public ResolvedArrayParameter(Type elementType, params object[] elementValues)
            : this((elementType ?? throw new ArgumentNullException(nameof(elementType))).MakeArrayType(),
                    elementType, elementValues)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types and collection of element values.
        /// </summary>
        /// <param name="contractType">The contract type for the array</param>
        /// <param name="elementType">The type of elements to resolve</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type contractType, Type elementType, params object[] elementValues)
            : base(contractType, false)
        {
            // Exit if no data
            if (elementValues is null || 0 == elementValues.Length)
            {
                _values = Array.CreateInstance(contractType, 0);
                return;
            }

            var complex = false;

            var data = new ReflectionInfo<Type>[elementValues.Length];
            for (var i = 0; i < data.Length; i++)
            {
                ref var entry = ref data[i];

                entry = elementType.AsInjectionInfo(elementValues[i]);

                if (ImportType.Value != entry.Data.DataType) complex = true;
            }

            if (!complex)
            {
                // For 'all values' simply translate into array
                var translator = TranslateMethod ??= 
                                 TypeInfo.GetDeclaredMethod(nameof(DoTranslate))!
                                         .MakeGenericMethod(elementType);
                _values = translator.Invoke(null, new object[] { data });
                return;
            }

            // For complex elements create resolver

            // TODO: Requires optimization
            _resolver = (ResolveMethod ??= TypeInfo
                .GetDeclaredMethod(nameof(DoResolve))!)
                    .MakeGenericMethod(typeof(PipelineContext), elementType)
                    .CreatePipeline(data);
        }

        #endregion


        #region Reflection

        public override ReflectionInfo<Type> FillReflectionInfo(Type type)
            => null == _resolver
                ? new ReflectionInfo<Type>(type, ParameterType ?? type, AllowDefault, _values,   ImportType.Value)
                : new ReflectionInfo<Type>(type, ParameterType ?? type, AllowDefault, _resolver, ImportType.Pipeline);

        public override ReflectionInfo<ParameterInfo> FillReflectionInfo(ParameterInfo member)
            => null == _resolver
                ? new ReflectionInfo<ParameterInfo>(member, ParameterType ?? member.ParameterType,
                                                            AllowDefault || member.HasDefaultValue, _values,   ImportType.Value)
                : new ReflectionInfo<ParameterInfo>(member, ParameterType ?? member.ParameterType,
                                                            AllowDefault || member.HasDefaultValue, _resolver, ImportType.Pipeline);
        public override ReflectionInfo<FieldInfo> FillReflectionInfo(FieldInfo member)
            => null == _resolver
                ? new ReflectionInfo<FieldInfo>(member, ParameterType ?? member.FieldType, AllowDefault, _values,   ImportType.Value)
                : new ReflectionInfo<FieldInfo>(member, ParameterType ?? member.FieldType, AllowDefault, _resolver, ImportType.Pipeline);

        public override ReflectionInfo<PropertyInfo> FillReflectionInfo(PropertyInfo member)
            => null == _resolver
                ? new ReflectionInfo<PropertyInfo>(member, ParameterType ?? member.PropertyType, AllowDefault, _values,   ImportType.Value)
                : new ReflectionInfo<PropertyInfo>(member, ParameterType ?? member.PropertyType, AllowDefault, _resolver, ImportType.Pipeline);

        #endregion


        #region Implementation

        private static object DoTranslate<TElement>(ReflectionInfo<Type>[] data) where TElement : class
        {
            var result = new TElement[data.Length];

            for (var i = 0; i < data.Length; i++)
                result[i] = (TElement)data[i].Data.Value!;

            return result;
        }

        private static object DoResolve<TContext, TElement>(ReflectionInfo<Type>[] data, ref TContext context)
            where TContext : IResolveContext
            where TElement : class
        {
            var result = new TElement[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                ref var entry = ref data[i];
                result[i] = entry.Data.DataType switch
                {
                    ImportType.Value    => (TElement)entry.Data.Value!,
                    ImportType.Pipeline => (TElement)((ResolveDelegate<TContext>)entry.Data.Value!)(ref context)!,
                    _                   => (TElement)context.Resolve(entry.Import.ContractType, entry.Import.ContractName)!,
                };
            }

            return result;
        }

        public override string ToString() => $"ResolvedArrayParameter: Type={ParameterType!.Name}";

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="ResolvedArrayParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements for the array of the parameter.</typeparam>
    public class ResolvedArrayParameter<TElement> : ResolvedArrayParameter
    {
        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter{TElement}"/> that
        /// resolves to the given element generic type with the given element values.
        /// </summary>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public ResolvedArrayParameter(params object[] elementValues)
            : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }

    #endregion
}
