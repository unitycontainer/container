using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Builder
{
    public class ImportedFieldsSelector : MemberSelectorBase<FieldInfo, object>, 
                                          IFieldSelectorPolicy
    {
        public IEnumerable<object> SelectFields<TBuilderContext>(ref TBuilderContext context) where TBuilderContext : IBuilderContext
        {
            return Enumerable.Empty<object>();
        }

        public override IEnumerable<object> Select<TContext>(ref TContext context)
        {
            return Enumerable.Empty<object>();
        }

        protected override FieldInfo[] DeclaredMembers(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
