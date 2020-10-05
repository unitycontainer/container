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
            var members = type.GetConstructors(BindingFlags);
            var downstream = builder.Build();

            ///////////////////////////////////////////////////////////////////
            // Check if any constructors are available
            if (0 == members.Length)
            {
                // Pipeline for BuildUp only, it throws if no object provided
                return (ref PipelineContext c) => (c.Target is null)
                    ? c.Error($"No accessible constructors on type {type}")
                    : downstream?.Invoke(ref c);
            }

            /////////////////////////////////////////////////////////////////
            // Build from Injected Constructor, if present
            if (builder.Context.Registration?.Constructor is InjectionConstructor injected)
            {
                int index;

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    // Pipeline for BuildUp only, it throws if no object provided
                    return (ref PipelineContext c) => (c.Target is null)
                        ? c.Error($"Injected constructor '{injected}' doesn't match any accessible constructors on type {type}")
                        : downstream?.Invoke(ref c);
                }

                var info = members[index];
                var args = null != injected.Data
                    ? ToDependencyArray(info.GetParameters(), injected.Data)
                    : ToDependencyArray(info.GetParameters());

                return CreatePipeline(info, args, downstream);
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == members.Length)
            {

                var info = members[0];
                var args = ToDependencyArray(info.GetParameters());

                return CreatePipeline(info, args, downstream);
            }


            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var info in members)
            {
                if (!info.IsDefined(typeof(ImportingConstructorAttribute))) continue;

                var args = ToDependencyArray(info.GetParameters());

                return CreatePipeline(info, args, downstream);
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

        private ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, DependencyInfo<ParameterInfo>[]? parameters, ResolveDelegate<PipelineContext>? pipeline)
        {
            if (null == parameters) return ParameterlessPipeline(info, pipeline);

            if (null == pipeline)
            {
                return (ref PipelineContext context) =>
                {
                    if (null == context.Target)
                    {
                        using var action = context.Start(info);
                        var argumetns = GetDependencies(ref context, parameters);

                        if (context.IsFaulted) return context.Target;

                        context.Target = info.Invoke(argumetns);
                    }

                    return context.Target;
                };
            }
            else
            {
                return (ref PipelineContext context) =>
                {
                    if (null == context.Target)
                    {
                        using var action = context.Start(info);
                        var argumetns = GetDependencies(ref context, parameters);

                        if (context.IsFaulted) return context.Target;

                        context.Target = info.Invoke(argumetns);
                    }

                    return pipeline.Invoke(ref context);
                };
            }
        }

        protected ResolveDelegate<PipelineContext> ParameterlessPipeline(ConstructorInfo info, ResolveDelegate<PipelineContext>? pipeline)
        {
            if (null == pipeline)
            {
                return (ref PipelineContext context) =>
                {
                    if (null == context.Target)
                    {
                        using var action = context.Start(info);
                        context.Target = info.Invoke(EmptyParametersArray);
                    }

                    return context.Target;
                };
            }

            return (ref PipelineContext context) =>
            {
                if (null == context.Target)
                {
                    using var action = context.Start(info);
                    context.Target = info.Invoke(EmptyParametersArray);
                }

                return pipeline(ref context);
            };
        }

        #endregion
    }

}
