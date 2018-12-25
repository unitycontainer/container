using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;

namespace Unity.Policy
{
    [Obsolete("IPropertySelectorPolicy has been deprecated, please use IPropertySelectorPolicy instead", true)]
    public interface IPropertySelectorPolicy : ISelect<PropertyInfo>
    {
        IEnumerable<object> SelectProperties(ref BuilderContext context);
    }
}
