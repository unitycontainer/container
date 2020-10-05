using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            // Do nothing if seed method exists
            // TODO: if (null != builder.Target) return builder.Build();

            Type type = builder.Context.Type;
            var ctors = type.GetConstructors(BindingFlags);
            var downstream = builder.Build();

            ///////////////////////////////////////////////////////////////////
            // Check if any constructors are available
            if (0 == ctors.Length)
            {
                // Pipeline for BuildUp only, it throws if no object provided
                return (ref PipelineContext c) => (c.Target is null)
                    ? c.Error($"No accessible constructors on type {type}")
                    : downstream?.Invoke(ref c);
            }

            /////////////////////////////////////////////////////////////////
            // Build from Injected Constructor if present
            if (builder.Context.Registration?.Constructor is InjectionConstructor iCtor)
            {
                int position;

                if (-1 == (position = iCtor.SelectFrom(ctors)))
                {
                    // Pipeline for BuildUp only, it throws if no object provided
                    return (ref PipelineContext c) => (c.Target is null)
                        ? c.Error($"Injected constructor '{iCtor}' doesn't match any accessible constructors on type {type}")
                        : downstream?.Invoke(ref c);
                }

                return CreatePipeline(ctors[position], iCtor.Data, downstream);
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == ctors.Length) return CreatePipeline(ctors[0], downstream);


            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in ctors)
            {
                if (!ctor.IsDefined(typeof(ImportingConstructorAttribute))) continue;

                return CreatePipeline(ctor, downstream);
            }


            //ConstructorInfo? info;

            //var selection = Select(ref builder);

            throw new NotImplementedException();

            #region
            //switch (selection)
            //{
            //    case ConstructorInfo memberInfo:
            //        info = memberInfo;
            //        resolvers = ParameterResolvers(info);
            //        break;

            //    case InjectionMethodBase<ConstructorInfo> injectionMember:
            //        info = injectionMember.MemberInfo(builder.Type);
            //        resolvers = null != injectionMember.Data && injectionMember.Data is object[] injectors && 0 != injectors.Length
            //                  ? ParameterResolvers(info, injectors)
            //                  : ParameterResolvers(info);
            //        break;

            //    case Exception exception:
            //        return (ref PipelineContext c) =>
            //        {
            //            if (null == c.Existing)
            //                throw exception;

            //            return null == pipeline ? c.Existing : pipeline.Invoke(ref c);
            //        };

            //    default:
            //        return (ref PipelineContext c) =>
            //        {
            //            if (null == c.Existing)
            //                throw new InvalidRegistrationException($"No public constructor is available for type {c.Type}.");

            //            return null == pipeline ? c.Existing : pipeline.Invoke(ref c);
            //        };
            //}

            //return GetResolverDelegate(info, resolvers, pipeline, builder.LifetimeManager is PerResolveLifetimeManager);
            #endregion
        }

        #endregion


        #region Implementation

        protected ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, ResolveDelegate<PipelineContext>? pipeline)
        {
            var parameters = info.GetParameters();

            if (0 == parameters.Length) return NoParametersPipeline(info, pipeline);

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < dependencies.Length; i++)
                dependencies[i] = GetDependencyInfo(parameters[i]);

            return CreatePipeline(info, CreateArgumentPileline(dependencies), pipeline);
        }

        protected ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, object[] data, ResolveDelegate<PipelineContext>? pipeline)
        {
            var parameters = info.GetParameters();

            if (0 == parameters.Length) return NoParametersPipeline(info, pipeline);

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < dependencies.Length; i++)
            {
                ref var dependency = ref dependencies[i];
                var parameter = parameters[i];

                dependency = GetDependencyInfo(parameter, data[i]);
            }

            return CreatePipeline(info, CreateArgumentPileline(dependencies), pipeline);
        }

        protected ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, ResolveDelegate<PipelineContext> dependencies, ResolveDelegate<PipelineContext>? pipeline)
        {
            if (null == pipeline)
            {
                return (ref PipelineContext c) =>
                {
                    if (null == c.Target)
                    {
                        using var action = c.Start(info);
                        c.Target = info.Invoke((object?[])dependencies(ref c)!);
                    }
                    return c.Target;
                };
            }

            return (ref PipelineContext c) =>
            {
                if (null == c.Target)
                { 
                    using var action = c.Start(info);
                    c.Target = info.Invoke((object?[])dependencies(ref c)!);
                }
                return pipeline(ref c);
            };
        }

        protected ResolveDelegate<PipelineContext> NoParametersPipeline(ConstructorInfo info, ResolveDelegate<PipelineContext>? pipeline)
        {
            if (null == pipeline)
            {
                return (ref PipelineContext c) =>
                {
                    if (null == c.Target)
                    {
                        using var action = c.Start(info);
                        c.Target = info.Invoke(EmptyParametersArray);
                    }
                    return c.Target;
                };
            }

            return (ref PipelineContext c) =>
            {
                if (null == c.Target)
                {
                    using var action = c.Start(info);
                    c.Target = info.Invoke(EmptyParametersArray);
                }
                return pipeline(ref c);
            };
        }

        #endregion
    }

}
