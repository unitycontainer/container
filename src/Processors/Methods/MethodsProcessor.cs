using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Processors
{
    public class MethodsProcessor : MethodBaseInfoProcessor<MethodInfo>
    {
        #region Constructors

        public MethodsProcessor()
            : base(new (Type type, Converter<MethodInfo, object> factory)[]
                { (typeof(InjectionMethodAttribute), info => info) })
        {
        }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();


        protected override MethodInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetMethodsHierarchical()
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
#endif
        }

        #endregion
    }
}
