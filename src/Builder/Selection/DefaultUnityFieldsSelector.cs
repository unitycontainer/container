using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Builder
{
    public class DefaultUnityFieldsSelector : MemberSelectorBase<FieldInfo, object>, 
                                          IFieldSelectorPolicy
    {
        public IEnumerable<object> SelectFields(ref BuilderContext context) 
        {
            return Enumerable.Empty<object>();
        }

        public override IEnumerable<object> Select(ref BuilderContext context)
        {
            return Enumerable.Empty<object>();
        }

        protected override FieldInfo[] DeclaredMembers(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
