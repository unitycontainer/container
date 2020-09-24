using System;
using System.Reflection;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region PipelineBuilder

        public override object? Build(ref PipelineBuilder<object?> builder)
        {
            ref var context = ref builder.Context;

            // Do nothing if building up
            if (null != context.Data) return builder.Build();

            // Type to build
            Type type = (Type)context.Action;
            var ctors = type.GetConstructors(BindingFlags);

            ///////////////////////////////////////////////////////////////////
            // Error if no constructors
            if (0 == ctors.Length) return builder.FromError($"No accessible constructors on type {type}");

            ///////////////////////////////////////////////////////////////////
            // Inject Constructor if available
            if (context.Registration?.Constructor is InjectionConstructor iCtor)
            {
                var selection = iCtor.SelectMember(ctors);

                if (null == selection.MemberInfo) 
                    return builder.FromError($"Injected constructor '{iCtor}' doesn't match any accessible constructors on type {type}");

                return Build(new PipelineContext(ref context, selection.MemberInfo, selection.Data));
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == ctors.Length) return Build(new PipelineContext(ref context, ctors[0]));

            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in ctors)
            {
                var selection = FromAnnotation(ctor);

                if (null == selection.MemberInfo) continue;

                return Build(new PipelineContext(ref context, selection.MemberInfo, selection.Data));
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

        protected object? Build(PipelineContext context)
        {
            ConstructorInfo info = (ConstructorInfo)context.Action;
            var parameters = info.GetParameters();

            if (0 == parameters.Length) return info.Invoke(EmptyParametersArray);
            //if (0 == parameters.Length) return Method();


            throw new NotImplementedException();

            //object Method() => info.Invoke(EmptyParametersArray);
        }

        #endregion
    }
}
