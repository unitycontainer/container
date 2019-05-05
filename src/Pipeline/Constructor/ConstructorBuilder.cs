using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Pipeline
{
    public partial class ConstructorBuilder : ParametersBuilder<ConstructorInfo>
    {
        #region Constructors

        public ConstructorBuilder(UnityContainer container)
            : base(typeof(InjectionConstructorAttribute), container)
        {
            SelectMethod = SmartSelector;
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

        public override ISelect<ConstructorInfo> GetOrDefault(IPolicySet registration) => 
            registration.Get<ISelect<ConstructorInfo>>() ?? 
                Defaults.SelectConstructor;

        #endregion
    }
}
