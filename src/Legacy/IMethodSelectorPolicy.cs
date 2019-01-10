using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;

namespace Unity.Policy
{
    [Obsolete("IMethodSelectorPolicy has been deprecated, please use ISelectMembers<MethodInfo> instead", true)]
    public interface IMethodSelectorPolicy
    {
        IEnumerable<object> SelectMethods(ref BuilderContext context);
    }
}
