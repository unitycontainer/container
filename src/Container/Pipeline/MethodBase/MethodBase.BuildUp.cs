using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Pipeline
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        protected virtual object[] ResolveParameters(ref BuildContext context, ParameterInfo[] parameters)
        {
            throw new NotImplementedException();
        }

        protected virtual object[] ResolveParameters(ref BuildContext context, ParameterInfo[] parameters, object[] injectors)
        {
            throw new NotImplementedException();
        }
    }
}
