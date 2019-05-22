using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Utility;

namespace Unity
{
    public partial class ConstructorPipeline : ParametersPipeline<ConstructorInfo>
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

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(ctor => !ctor.IsFamily && !ctor.IsPrivate && !ctor.IsStatic);
        }

        public override MemberSelector<ConstructorInfo> GetOrDefault(IPolicySet? registration) => 
            registration?.Get<MemberSelector<ConstructorInfo>>() ?? 
                Defaults.SelectConstructor;

        #endregion
    }
}
