using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        public override object? Analyse<TContext>(ref TContext context)
        {
            return base.Analyse(ref context);
        }
    }
}
