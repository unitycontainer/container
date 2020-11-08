using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
                                                                 where TData       : class
    {
        #region Constants

        /// <summary>
        /// Binding flags used to obtain declared members by default
        /// </summary>
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        #endregion


        #region Fields

        /// <summary>
        /// Combination of <see cref="BindingFlags"/> to use when getting declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }

        protected ReflectionProvider<ImportInfo<TDependency>, bool> GetImportInfo { get; private set; }

        protected ReflectionData<ImportInfo<TDependency>, ImportData> ParseData { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults, ReflectionProvider<ImportInfo<TDependency>, bool> importProvider,
                                                  ReflectionData<ImportInfo<TDependency>, ImportData> parser)
        {
            BindingFlags = defaults.GetOrAdd(typeof(TMemberInfo), DefaultBindingFlags, 
                (object flags) => BindingFlags = (BindingFlags)flags);

            GetImportInfo = defaults.GetOrAdd(typeof(TDependency), importProvider,
                (object policy) => GetImportInfo = (ReflectionProvider<ImportInfo<TDependency>, bool>)policy);

            ParseData = defaults.GetOrAdd(typeof(TDependency), parser,
                (object policy) => ParseData = (ReflectionData<ImportInfo<TDependency>, ImportData>)policy);
        }

        #endregion
    }
}
