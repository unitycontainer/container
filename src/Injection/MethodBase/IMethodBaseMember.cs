using System;

namespace Unity
{
    public interface IMethodBaseMember<out TMemberInfo>
    {
        TMemberInfo GetInfo(Type type);

        object[] GetParameters();
    }
}
