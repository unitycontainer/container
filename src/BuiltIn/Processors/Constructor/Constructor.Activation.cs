using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

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
            var ctors = type.GetConstructors(BindingFlags);

            ///////////////////////////////////////////////////////////////////
            // Error if no constructors
            if (0 == ctors.Length)
            {
                context.Error($"No accessible constructors on type {type}");
                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Inject Constructor if available
            if (context.Registration?.Constructor is InjectionConstructor iCtor)
            {
                int position;

                using var injected = context.Start(iCtor);
                if (-1 == (position = iCtor.SelectFrom(ctors)))
                {
                    injected.Error($"Injected constructor '{iCtor}' doesn't match any accessible constructors on type {type}");
                    return;
                }

                using var action = context.Start(ctors[position]);
                if (null != iCtor.Data && 0 != iCtor.Data.Length)
                    Activate(ref context, iCtor.Data);
                else
                    Activate(ref context);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == ctors.Length)
            {

                using var action = context.Start(ctors[0]);
                Activate(ref context);
                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in ctors)
            {
                if (!ctor.IsDefined(typeof(ImportingConstructorAttribute), true)) continue;

                using var action = context.Start(ctor);

                Activate(ref context);

                return;
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

        private void Activate(ref PipelineContext context, object?[] data)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = GetDependencyInfo(parameters[i], data[i]);
                arguments[i] = Activate(ref parameter);

                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(arguments);
        }


        private void Activate(ref PipelineContext context)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = GetDependencyInfo(parameters[i]);
                arguments[i] = Activate(ref parameter);
                
                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(arguments);
        }

        #endregion
    }
}
