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


        #region Fields

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        protected Func<Type, TMemberInfo[]> GetMembers;

        /// <summary>
        /// Function to load <see cref="ImportInfo{TMember}"/> with data from current <see cref="ParameterInfo"/>,
        /// <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> and all supported attributes.
        /// </summary>
        protected ImportProvider<ImportInfo<TDependency>> GetImportInfo { get; private set; }

        protected ImportDataProvider<TDependency> ParseImportData { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults, Func<Type, TMemberInfo[]> members, 
                                                  ImportProvider<ImportInfo<TDependency>> loader,
                                                  ImportDataProvider<TDependency> parser)
        {
            GetMembers = defaults.GetOrAdd(typeof(TDependency), members,
                (object policy) => GetMembers = (Func<Type, TMemberInfo[]>)policy);

            GetImportInfo = defaults.GetOrAdd(typeof(TDependency), loader,
                (object policy) => GetImportInfo = (ImportProvider<ImportInfo<TDependency>>)policy);

            ParseImportData = defaults.GetOrAdd(typeof(TDependency), parser,
                (object policy) => ParseImportData = (ImportDataProvider<TDependency>)policy);
        }

        #endregion
    }
}
