using System;

namespace Unity.Policy
{

    public interface ISelect<TInfo, TData>
    {
        Converter<Type, (TInfo, TData)> Select { get; }
    }
}
