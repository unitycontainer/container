using System;
using Unity.Policy;
using System.Reflection;

namespace Unity.Processors
{
    public class MethodBaseInfoProcessor<TMemberInfo> : MemberBuildProcessor<TMemberInfo, object[]>
                                    where TMemberInfo : MemberInfo
    {
        public MethodBaseInfoProcessor((Type type, Converter<TMemberInfo, object> factory)[] factories)
            : base(factories)
        {
        }
    }
}
