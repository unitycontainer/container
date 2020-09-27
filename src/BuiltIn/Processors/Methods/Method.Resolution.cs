using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor
    {
        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info)
        {
            throw new NotImplementedException();
            //var resolvers = ParameterResolvers(info);

            //return (ref PipelineContext c) =>
            //{
            //    if (null == c.Data) return c.Data;

            //    var dependencies = new object?[resolvers.Length];
            //    for (var i = 0; i < dependencies.Length; i++)
            //        dependencies[i] = resolvers[i](ref c);

            //    info.Invoke(c.Data, dependencies);

            //    return c.Data;
            //};
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info, object? data)
        {
            throw new NotImplementedException();
            //object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;
            
            //if (null == injectors) return GetResolverDelegate(info);

            //var resolvers = ParameterResolvers(info, injectors);

            //return (ref PipelineContext c) =>
            //{
            //    if (null == c.Data) return c.Data;

            //    var dependencies = new object?[resolvers.Length];
            //    for (var i = 0; i < dependencies.Length; i++) dependencies[i] = resolvers[i](ref c);

            //    info.Invoke(c.Data, dependencies);

            //    return c.Data;
            //};
        }
    }
}
