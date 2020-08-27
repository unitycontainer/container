using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        private static ConstructorInfo DefaultConstructorSelector(ConstructorInfo[] members, ref ResolveContext context)
        {
            if (1 == members.Length) return members[0];

            throw new NotImplementedException();
        }
    }
}
