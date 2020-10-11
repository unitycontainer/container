using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region 

        public static InjectedData TranslateData<TInfo>(TInfo info, object? data)
        {
            return data switch
            {
                IResolve iResolve                         => new InjectedData((ResolveDelegate<PipelineContext>)iResolve.Resolve, InjectionType.Resolver),
                ResolveDelegate<PipelineContext> resolver => new InjectedData(data,                                               InjectionType.Resolver),
                IResolverFactory<TInfo> infoFactory       => new InjectedData(infoFactory.GetResolver<PipelineContext>(info),     InjectionType.Resolver),
                IResolverFactory<Type> typeFactory        => info switch 
                {
                    ParameterInfo parameter               => new InjectedData(typeFactory.GetResolver<PipelineContext>(parameter.ParameterType), InjectionType.Resolver),
                    PropertyInfo property                 => new InjectedData(typeFactory.GetResolver<PipelineContext>(property.PropertyType),   InjectionType.Resolver),
                    FieldInfo field                       => new InjectedData(typeFactory.GetResolver<PipelineContext>(field.FieldType),         InjectionType.Resolver),
                    _                                     => new InjectedData(data, InjectionType.Unknown) 
                },
                _                                         => new InjectedData(data, InjectionType.Value),
            };
        }

        #endregion
    }
}
