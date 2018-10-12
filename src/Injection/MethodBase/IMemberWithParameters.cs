using System;

namespace Unity.Injection
{
    public interface IMemberWithParameters<out TMemberInfo>
    {
        TMemberInfo MemberInfo(Type type);

        object[] GetParameters();
    }
}
