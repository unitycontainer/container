using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity
{
    public delegate IEnumerable<object> MemberSelector<TMember, TParam>(Type type, params TParam[]? members)
        where TMember : MemberInfo;
}
