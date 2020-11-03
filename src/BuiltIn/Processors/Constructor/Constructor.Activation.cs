using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Activation

        public override void PreBuild(ref PipelineContext context)
        {
            // Do nothing if building up
            if (null != context.Target) return;

            Type type = context.Type;
            var members = type.GetConstructors(BindingFlags);

            ///////////////////////////////////////////////////////////////////
            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {type}");
                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Inject the constructor, if available
            if (context.Registration?.Constructor is InjectionConstructor injected)
            {
                int index;

                using var action = context.Start(injected);
                if (-1 == (index = injected.SelectFrom(members)))
                {
                    action.Error($"Injected constructor '{injected}' doesn't match any accessible constructors on type {type}");
                    return;
                }

                using var subaction = context.Start(members[index]);

                if (null == injected.Data) 
                    Build(ref context);
                else                       
                    Build(ref context, injected.Data);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == members.Length)
            {
                using var action = context.Start(members[0]);
                
                Build(ref context);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in members)
            {
                if (!ctor.IsDefined(typeof(ImportingConstructorAttribute), true)) continue;

                using var action = context.Start(ctor);

                Build(ref context);

                return;
            }


            //ConstructorInfo? info;

            //var selection = Select(ref builder);

            throw new NotImplementedException("Constructor Selection");
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

        private void Build(ref PipelineContext context, object?[] data)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : Build(ref context, parameters, data);

            if (context.IsFaulted) return;

            // TODO: Preemptive optimization
            if (context.Registration is PerResolveLifetimeManager)
                context.PerResolve = info.Invoke(arguments);
            else
                context.Target = info.Invoke(arguments);
        }


        private void Build(ref PipelineContext context)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : base.Build(ref context, parameters);

            if (context.IsFaulted) return;

            // TODO: Preemptive optimization
            if (context.Registration is PerResolveLifetimeManager) 
                context.PerResolve = info.Invoke(arguments);
            else
                context.Target = info.Invoke(arguments);
        }

        #endregion
    }
}
