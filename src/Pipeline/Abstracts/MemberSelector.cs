using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Registration;

namespace Unity
{
    public delegate IEnumerable<object> MemberSelector<TMember>(Type type, IRegistration set)
        where TMember : MemberInfo;
}
