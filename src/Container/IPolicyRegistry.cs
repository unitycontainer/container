using System;
using Unity.Policy;

namespace Unity.Container
{
    public interface IPolicyRegistry
    {
        IBuilderPolicy this[Type type, string name, Type policy] { get; set; }
    }
}
