using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        public override void PreBuild(ref PipelineContext context)
        {
            // Do nothing if building up
            if (null != context.Target) return;

            // Type to build
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
                using var action = context.Start(iCtor);

                var selection = iCtor.SelectMember(ctors);

                if (null == selection.MemberInfo)
                {
                    action.Error($"Injected constructor '{iCtor}' doesn't match any accessible constructors on type {type}");
                    return; 
                }

                if (null == iCtor.Data || 0 == iCtor.Data.Length)
                    context.Target = Activate(ref context, selection.MemberInfo);
                else
                    context.Target = Activate(ref context, selection.MemberInfo, selection.Data);

                return; 
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == ctors.Length)
            {

                context.Target = Activate(ref context, ctors[0]);
                return; 
            }

            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in ctors)
            {
                if (!ctor.IsDefined(typeof(ImportingConstructorAttribute), true)) continue;

                context.Target = Activate(ref context, ctor);
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



        #region Implementation

        protected object? Activate(ref PipelineContext context, ConstructorInfo info, object?[] data)
        {
            using var action = context.Start(info);

            var parameters = info.GetParameters();
            if (0 == parameters.Length) return info.Invoke(EmptyParametersArray);

            DependencyInfo dependencyInfo = new DependencyInfo(ref context);
            var values = new object?[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                dependencyInfo.Info = parameters[i];
                values[i] = GetValue(ref dependencyInfo, data[i]);

                if (context.IsFaulted) return values[i];
            }
            
            return info.Invoke(values);
        }

        protected object? Activate(ref PipelineContext context, ConstructorInfo info)
        {
            using var action = context.Start(info);
             
            var parameters = info.GetParameters();
            if (0 == parameters.Length) return info.Invoke(EmptyParametersArray);

            DependencyInfo dependencyInfo = new DependencyInfo(ref context);
            var values = new object?[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                dependencyInfo.Info = parameters[i];
                values[i] = Resolve(ref dependencyInfo);
                
                if (context.IsFaulted) return values[i];
            }

            return info.Invoke(values);
        }

        #endregion
    }
}
