using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Dependency
{
    public interface IDependencyValue : IDependency
    {
        object Value { get; }
    }
}
