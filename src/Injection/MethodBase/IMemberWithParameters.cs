using System;

namespace Unity
{
    public interface IMemberWithParameters<out TMemberInfo>
    {
        TMemberInfo MemberInfo(Type type);

        object[] GetParameters();
    }
}
