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
        protected ResolvedArrayParameter(Type contractType, Type elementType, object[] elementValues)
            : base(contractType, false) 
            => (_values, _resolver) = GetResolver(contractType, elementType, elementValues);

        #endregion


        #region Reflection

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            import.ContractType = ParameterType!;
            import.AllowDefault = AllowDefault;

            if (null == _resolver)
                import.Value = _values;
            else
                import.Pipeline = _resolver;
        }

        #endregion


        #region Implementation

        internal static (object?, ResolveDelegate<PipelineContext>?) GetResolver(Type contractType, Type elementType, object[] elementValues)
        {
            if (0 == elementValues.Length) return (Array.CreateInstance(elementType, 0), null);

            var complex = false;

            var data = new ReflectionInfo[elementValues.Length];
            for (var i = 0; i < data.Length; i++)
            {
                ref var entry = ref data[i];

                entry.ContractType  = elementType;
                entry.DeclaringType = contractType;

                PipelineProcessor.ProcessImport(ref entry, elementValues[i]);

                if (ImportType.Value != entry.Data.ImportType) complex = true;
            }

            if (!complex)
            {
                // For 'all values' simply translate into array
                var translator = (TranslateMethod ??= TypeInfo.GetDeclaredMethod(nameof(DoTranslate))!)
                                                              .MakeGenericMethod(elementType);
                return (translator.Invoke(null, new object[] { data }), null);
            }

            // For complex elements create resolver
            return (null, (ResolveMethod ??= TypeInfo
                .GetDeclaredMethod(nameof(DoResolve))!).MakeGenericMethod(typeof(PipelineContext), elementType)
                                                       .CreatePipeline(data));
        }

        private static object DoTranslate<TElement>(ReflectionInfo[] data)
        {
            var result = new TElement[data.Length];

            for (var i = 0; i < data.Length; i++)
                result[i] = (TElement)data[i].Data.Value!;

            return result;
        }

        private static object DoResolve<TContext, TElement>(ReflectionInfo[] data, ref TContext context)
            where TContext : IResolveContext
        {
            var result = new TElement[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                ref var entry = ref data[i];
                result[i] = entry.Data.ImportType switch
                {
                    ImportType.Value => (TElement)entry.Data.Value!,
                    ImportType.Pipeline => (TElement)((ResolveDelegate<TContext>)entry.Data.Value!)(ref context)!,
                    _ => (TElement)context.Resolve(entry.ContractType, entry.ContractName)!,
                };
            }

            return result;
        }

        public override string ToString() 
            => $"ResolvedArrayParameter: Type={ParameterType!.Name}";

        #endregion


        private struct ReflectionInfo : IInjectionInfo
        {
            public ImportData Data;

            public bool AllowDefault { get; set; }
            public Type ContractType { get; set; }
            public string? ContractName { get; set; }

            
            public object? Value { set => Data = new ImportData(value, ImportType.Value); }
            public object? External { set => Data = new ImportData(value, ImportType.Unknown); }
            public ResolveDelegate<PipelineContext> Pipeline { set => Data = new ImportData(value, ImportType.Pipeline); }


            public Type MemberType => ContractType;
            public Type DeclaringType { get; set; }


            public ImportType ImportType => Data.ImportType;
            public object? ImportValue => Data.Value;

            public Attribute[]? Attributes { get; }

            Type IImportInfo.ContractType => ContractType;
            string? IImportInfo.ContractName => ContractName;
        }
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
