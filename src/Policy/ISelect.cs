
using System;

namespace Unity
{
    public interface ISelect<TInfo, TData>
    {
        (TInfo, TData) Select(Type type);
    }
}
