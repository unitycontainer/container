using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Policy
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}
