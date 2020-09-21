using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region PipelineBuilder

        public  Pipeline? Resolution_Visitor(ref PipelineBuilder<Pipeline?> builder)
        {
            throw new NotImplementedException();
            //var pipeline = builder.Build();

            //ConstructorInfo? info;
            //Type type = builder.Type;
            //var ctors = type.GetConstructors(BindingFlags);
            //var perResolve = builder.LifetimeManager is PerResolveLifetimeManager;

            //if (0 == ctors.Length) return NoCtorPipeline(pipeline, "No accessible constructors");

            //// Injected constructor
            //if (null != builder.Constructor)
            //{
            //    if (null == (info = builder.Constructor.MemberInfo(ctors)))
            //        return NoCtorPipeline(pipeline, "Invalid injected constructor");
                
            //    return GetResolverDelegate(info, builder.Constructor.Data, pipeline, perResolve);
            //}

            //// Only one constructor
            //if (1 == ctors.Length)
            //{
            //    info = ctors[0];
            //    return GetResolverDelegate(info, ParameterResolvers(info), pipeline, perResolve);
            //}


            throw new NotImplementedException();

//            object[]? resolvers = null;
            // Select ConstructorInfo
            //var selection = Select(ref builder);

            // Select constructor for the Type

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
            //        return (ref ResolutionContext c) =>
            //        {
            //            if (null == c.Existing)
            //                throw exception;

            //            return null == pipeline ? c.Existing : pipeline.Invoke(ref c);
            //        };

            //    default:
            //        return (ref ResolutionContext c) =>
            //        {
            //            if (null == c.Existing)
            //                throw new InvalidRegistrationException($"No public constructor is available for type {c.Type}.");

            //            return null == pipeline ? c.Existing : pipeline.Invoke(ref c);
            //        };
            //}

        }

        #endregion


        #region Implementation

        /// <summary>
        /// Pipeline without valid constructor
        /// </summary>
        /// <param name="pipeline">Downstream pipeline</param>
        /// <param name="error">Error message to display if seed value is not provided</param>
        /// <returns>Value returned from pipeline or <see cref="RegistrationManager.NoValue"/> if no seed provided</returns>
        protected virtual Pipeline NoCtorPipeline(Pipeline? pipeline, string error)
        {
            throw new NotImplementedException();
            //// Activate type
            //return (ref ResolutionContext context) =>
            //{
            //    // Valid for BuildUp calls only
            //    if (null == context.Existing)
            //    {
            //        context.FromError(error);
            //        return; 
            //    }

            //    // Invoke other initializers
            //    pipeline?.Invoke(ref context);
            //};
        }

        protected virtual Pipeline GetResolverDelegate(ConstructorInfo info, object? data, Pipeline? pipeline, bool perResolve)
        {
            Debug.Assert(null != data && data is Pipeline[]);
            var resolvers = (Pipeline[])data!;

            try
            {
                // Select Lifetime type ( Per Resolve )
                if (perResolve)
                {
                    // Activate type
                    return (ref ResolutionContext context) =>
                    {
                        if (null == context.Target)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                            { 
                                //dependencies[i] = resolvers[i](ref context);
                            }

                            var value = info.Invoke(dependencies);

                            context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));
                            context.FromValue(value);
                        }

                        // Invoke other initializers
                        pipeline?.Invoke(ref context);
                    };
                }
                else
                {
                    // Activate type
                    return (ref ResolutionContext context) =>
                    {
                        if (null == context.Target)
                        {
                            var dependencies = new object?[resolvers.Length];
                            for (var i = 0; i < dependencies.Length; i++)
                            { 
                                //dependencies[i] = resolvers[i](ref context);
                            }

                            var value = info.Invoke(dependencies);

                            context.FromValue(value);
                        }

                        // Invoke other initializers
                        pipeline?.Invoke(ref context);
                    };
                }
            }
            catch (InvalidRegistrationException ex)
            {
                // Throw if try to create
                return (ref ResolutionContext context) =>
                {
                    if (null == context.Target)
                    {
                        // TODO: Requires revisiting
                        context.FromException(ex);
                        return;
                    }
                        
                    pipeline?.Invoke(ref context);
                };
            }
            catch (Exception ex)
            {
                // Throw if try to create
                return (ref ResolutionContext context) =>
                {
                    if (null == context.Existing)
                    {
                        // TODO: Requires revisiting
                        context.FromException(new InvalidRegistrationException("Error", ex));
                        return;
                    }

                    pipeline?.Invoke(ref context);
                };
            }
        }

        #endregion   
    }
}
