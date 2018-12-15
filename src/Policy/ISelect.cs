using System;

namespace Unity.Policy
{
    public interface ISelect<TInfo, TData>
    {
        (TInfo, TData) Select(Type type);
    }
}
