using System;
using Unity.Strategies;
using Unity.Extension;
using Unity.Policy;
using Unity.Builder;
using Unity.Resolution;
using Unity.Registration;
using System.Globalization;

namespace Unity.Benchmarks
{
    /// <summary>
    /// An extension to install custom strategy that disables
    /// saving of created build plan
    /// </summary>
    public class PipelineSpyExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.Add(new PipelineSpyStrategy(), UnityBuildStage.PreCreation);
        }
    }

    /// <summary>
    /// Unity v5 uses <see cref="BuildPlanStrategy"/> to create and save
    /// a build plan for each created type
    /// </summary>
    public class PipelineSpyStrategy : BuildPlanStrategy
    {
        // Override default implementation
        public override void PreBuildUp(ref BuilderContext context)
        {
            var resolver = context.Registration.Get<ResolveDelegate<BuilderContext>>() ?? (ResolveDelegate<BuilderContext>)
                                   GetGeneric(ref context, typeof(ResolveDelegate<BuilderContext>));

            if (null == resolver)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (!(context.Registration is ContainerRegistration) &&  context.RegistrationType.GetTypeInfo().IsGenericTypeDefinition)
#else
                if (!(context.Registration is ContainerRegistration) && context.RegistrationType.IsGenericTypeDefinition)
#endif
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "The type {0} is an open generic type. An open generic type cannot be resolved.",
                        context.RegistrationType.FullName), new Exception());
                }
                else if (context.Type.IsArray && context.Type.GetArrayRank() > 1)
                {
                    var message = $"Invalid array {context.Type}. Only arrays of rank 1 are supported";
                    throw new ArgumentException(message, new Exception());
                }

                var factory = context.Registration.Get<ResolveDelegateFactory>() ?? (ResolveDelegateFactory)(
                              context.Get(context.Type, UnityContainer.All, typeof(ResolveDelegateFactory)) ??
                              GetGeneric(ref context, typeof(ResolveDelegateFactory)) ??
                              context.Get(null, null, typeof(ResolveDelegateFactory)));

                if (null != factory)
                {
                    resolver = factory(ref context);

                    // Disable saving, force creation every time
                    // context.Registration.Set(typeof(ResolveDelegate<BuilderContext>), resolver);
                    context.Existing = resolver(ref context);
                }
                else
                    throw new ResolutionFailedException(context.Type, context.Name, $"Failed to find Resolve Delegate Factory for Type {context.Type}");
            }
            else
            {
                context.Existing = resolver(ref context);
            }

            // Prevent further processing
            context.BuildComplete = true;
        }
    }
}
