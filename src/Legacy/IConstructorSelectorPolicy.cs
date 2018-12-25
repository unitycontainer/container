using System;
using System.Reflection;

namespace Unity.Policy
{
    [Obsolete("IConstructorSelectorPolicy has been deprecated, please use ISelect<ConstructorInfo> instead", true)]
    public interface IConstructorSelectorPolicy : ISelect<ConstructorInfo>
    {
    }
}
