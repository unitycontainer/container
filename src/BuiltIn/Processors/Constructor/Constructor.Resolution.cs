using System;
using System.Reflection;
using Unity.Container;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            // Do nothing if seed method exists
            if (null != builder.Target) return builder.Build();

            // Type to build
            Type type = (Type)builder.Context.Action;
            var ctors = type.GetConstructors(BindingFlags);

            ///////////////////////////////////////////////////////////////////
            // Check if any constructors are available
            if (0 == ctors.Length)
            {
                // Create pipeline

                var pipeline = builder.Build();
                return (ref PipelineContext c) =>
                {
                    if (null != c.Data) return pipeline?.Invoke(ref c);
                    throw new InvalidRegistrationException($"No accessible constructors on type {c.Type}");
                };
            }


            ///////////////////////////////////////////////////////////////////
            // Build from Injected Constructor if present
            if (builder.Context.Registration?.Constructor is InjectionConstructor iCtor)
            {
                var selection = iCtor.SelectMember(ctors);
                if (null == selection.MemberInfo)
                {
                    // Create BuildUp only pipeline

                    var id = iCtor.ToString();
                    var pipeline = builder.Build();

                    return (ref PipelineContext c) =>
                    {
                        if (null != c.Data) return pipeline?.Invoke(ref c);
                        throw new InvalidRegistrationException($"Injected constructor '{id}' doesn't match any accessible constructors on type {c.Type}");
                    };
                }

                return CreatePipeline(ref selection, ref builder);
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == ctors.Length) return CreatePipeline(ctors[0], ref builder);


            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in ctors)
            {
                //var selection = FromAnnotation(ctor);
                
                //if (null == selection.MemberInfo) continue;

                //return CreatePipeline(ref selection, ref builder);
            }

            
            //ConstructorInfo? info;

            //var selection = Select(ref builder);

            throw new NotImplementedException();

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
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext> CreatePipeline(ref SelectionInfo<ConstructorInfo, object[]> selection,
                                                                          ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            var parameters = ParameterPipelines(ref selection);

            throw new NotImplementedException();
        }

        protected virtual ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            ResolveDelegate<PipelineContext>? pipeline = builder.Build();

            return builder.Context.Registration switch
            {
                // Default constructor with no pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            context.Data = info.Invoke(EmptyParametersArray);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        return context.Data;
                    }
                ,

                // Default constructor with pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            context.Data = info.Invoke(EmptyParametersArray);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,

                // Default constructor with no pipeline
                _ when null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data) context.Data = info.Invoke(EmptyParametersArray);
                        return context.Data;
                    }
                ,

                // Default constructor with pipeline
                _ when null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data) context.Data = info.Invoke(EmptyParametersArray);

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,

                // Unknown case
                _ => throw new NotImplementedException()
            };
        }

        protected virtual ResolveDelegate<PipelineContext> CreatePipeline(ConstructorInfo info, ResolveDelegate<PipelineContext>[]? resolvers,
                                                                               ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            ResolveDelegate<PipelineContext>? pipeline = builder.Build();

            return builder.Context.Registration switch
            {
                // Default constructor with no pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null == resolvers && null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            context.Data = info.Invoke(EmptyParametersArray);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        return context.Data;
                    }
                ,

                // Default constructor with pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null == resolvers && null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            context.Data = info.Invoke(EmptyParametersArray);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,

                // Constructor with no pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null != resolvers && null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                                dependencies[i] = resolvers[i](ref context);

                            context.Data = info.Invoke(dependencies);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        return context.Data;
                    }
                ,

                // Constructor with pipeline and Per Resolve lifetime
                PerResolveLifetimeManager _ when null != resolvers && null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                                dependencies[i] = resolvers[i](ref context);

                            context.Data = info.Invoke(dependencies);
                            // TODO: context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Existing));
                        }

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,


                // Default constructor with no pipeline
                _ when null == resolvers && null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data) context.Data = info.Invoke(EmptyParametersArray);
                        return context.Data;
                    }
                ,

                // Default constructor with pipeline
                _ when null == resolvers && null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data) context.Data = info.Invoke(EmptyParametersArray);

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,

                // Constructor with no pipeline
                _ when null != resolvers && null == pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                                dependencies[i] = resolvers[i](ref context);

                            context.Data = info.Invoke(dependencies);
                        }

                        return context.Data;
                    }
                ,

                // Constructor with pipeline
                _ when null != resolvers && null != pipeline =>
                    (ref PipelineContext context) =>
                    {
                        if (null == context.Data)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                                dependencies[i] = resolvers[i](ref context);

                            context.Data = info.Invoke(dependencies);
                        }

                        // Invoke other initializers
                        return pipeline.Invoke(ref context);
                    }
                ,


                // Unknown case
                _ => throw new NotImplementedException()
            };
        }

        #endregion
    }
}
