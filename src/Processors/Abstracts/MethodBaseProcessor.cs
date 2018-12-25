using System;
using Unity.Policy;
using System.Reflection;

namespace Unity.Processors
{
    public class MethodBaseInfoProcessor<TMemberInfo> : MemberInfoProcessor<TMemberInfo, object[]>
                                    where TMemberInfo : MemberInfo
    {
        public MethodBaseInfoProcessor()
        {

        }

        public MethodBaseInfoProcessor((Type type, Converter<TMemberInfo, object> factory)[] factories)
            : base(factories)
        {
            
        }
    }
}
