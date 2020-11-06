using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, ParameterInfo, object[]>
                                                where TMemberInfo : MethodBase
    {
        #region Fields

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];

        protected Func<ParameterInfo, object?, ReflectionInfo<ParameterInfo>> InjectionInfoFromData;
        protected Func<ParameterInfo, ReflectionInfo<ParameterInfo>>          InjectionInfoFromParameter;

        #endregion


        #region Constructors

        /// <inheritdoc/>
        public ParameterProcessor(Defaults defaults)
            : base(defaults)
        {
            InjectionInfoFromData = defaults.GetOrAdd<Func<ParameterInfo, object?, ReflectionInfo<ParameterInfo>>>(ToInjectionInfoFromData, 
                (policy) => InjectionInfoFromData = (Func<ParameterInfo, object?, ReflectionInfo<ParameterInfo>>)policy);

            InjectionInfoFromParameter = defaults.GetOrAdd<Func<ParameterInfo, ReflectionInfo<ParameterInfo>>>(ToInjectionInfo,
                (policy) => InjectionInfoFromParameter = (Func<ParameterInfo, ReflectionInfo<ParameterInfo>>)policy);
        }

        #endregion


        #region Import Info

        protected override bool FillImportInfo(ParameterInfo member, ref ImportInfo<ParameterInfo> info)
        {
            var import = member.GetCustomAttribute<ImportAttribute>(true);
            if (null != import)
            {
                info = new ImportInfo<ParameterInfo>(member, member.ParameterType, import, member.HasDefaultValue);
                return true;
            }

            var many = member.GetCustomAttribute<ImportManyAttribute>(true);
            if (null != many)
            {
                info = new ImportInfo<ParameterInfo>(member, member.ParameterType, many, member.HasDefaultValue);
                return true;
            }

            info = new ImportInfo<ParameterInfo>(member, member.ParameterType, member.HasDefaultValue);
            return false;
        }

        #endregion
    }
}
