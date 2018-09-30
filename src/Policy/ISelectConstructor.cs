using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Policy
{
    public interface ISelectConstructor
    {
        object Select(Type type);
    }
}
