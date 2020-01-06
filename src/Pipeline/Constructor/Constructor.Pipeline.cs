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
        {
            SelectMethod = container.ExecutionMode.IsLegacy()
                         ? (CtorSelectorDelegate)LegacySelector
                         : SmartSelector;
        }

        #endregion


        #region Public Properties

        public CtorSelectorDelegate SelectMethod { get; set; }

        #endregion


        #region Overrides

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => type.SupportedConstructors();

        #endregion
    }
}
