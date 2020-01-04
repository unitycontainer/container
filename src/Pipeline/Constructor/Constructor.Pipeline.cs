using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Utility;

namespace Unity
{
    public partial class ConstructorPipeline : MethodBasePipeline<ConstructorInfo>
    {
        #region Constructors

        public ConstructorPipeline(UnityContainer container)
            : base(typeof(InjectionConstructorAttribute), container)
        {
            SelectMethod = container.ExecutionMode.IsLegacy()
                ? (Func<Type, ConstructorInfo[], object?>)LegacySelector
                : SmartSelector;
        }

        #endregion


        #region Public Properties

        public Func<Type, ConstructorInfo[], object?> SelectMethod { get; set; }

        #endregion


        #region Overrides

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => UnityDefaults.SupportedConstructors(type);

        #endregion
    }
}
