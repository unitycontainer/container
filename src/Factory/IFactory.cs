using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Factory
{
    public interface IFactory<TSource, TResult>
    {
        TResult Create(TResult source);
    }
}
