using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, object>
    {
        #region Constructors

        public FieldProcessor(IPolicySet policySet)
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return GetFieldsHierarchical(type).Where(f => !f.IsInitOnly && !f.IsStatic);
#else
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                       .Where(f => !f.IsInitOnly && !f.IsStatic);
#endif
        }

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        #endregion


        #region Implementation
#if NETSTANDARD1_0

        public static IEnumerable<FieldInfo> GetFieldsHierarchical(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredFields;
            }

            return type.GetTypeInfo()
                .DeclaredFields
                .Concat(GetFieldsHierarchical(type.GetTypeInfo().BaseType));
        }
#endif
        #endregion
    }
}
