using System;
using System.Linq;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor 
    {
        /// <summary>
        /// Selects appropriate constructor and calls <see cref="BuildUp"/> method 
        /// to activate an instance
        /// </summary>
        /// <param name="context">A <see cref="ResolveContext"/> structure holding 
        /// resolution context</param>
        public override void PreBuildUp(ref ResolveContext context)
        {
            // If instance already exists skip activation
            if (null != context.Existing) return;

            // Indicate that we are selecting constructor
            context.Activity = typeof(ConstructorInfo);

            // Type to build
            Type type = context.Manager?.Type ?? context.Type;
            var ctors = type.GetConstructors(SupportedBindingFlags)
                            .Where(SupportedMembersPredicate)
                            .ToArray();

            // Invoke injected constructor if available
            if (null != context.Manager?.Constructor)
            {
                var info = context.Manager.Constructor.InjectionInfo(ctors);

                // BuildUp if found
                if (null != info.MemberInfo)
                {
                    if (null == info.Data)
                        BuildUp(ref context, info.MemberInfo);
                    else
                        BuildUp(ref context, info.MemberInfo, info.Data);

                    return;
                }

                // Throw if error
                if (null != info.Exception) throw info.Exception;
            }

            // Search for and invoke constructor annotated with attribute
            foreach (var ctor in ctors)
            {
                if (!IsAnnotated(ctor)) continue;

                BuildUp(ref context, ctor);

                return;
            }

            // Use algorithm to select constructor
            var selection = SelectConstructor(ctors, ref context);

            // Activate or throw if not found
            if (null == selection) throw new InvalidOperationException("No constructor");
            
            BuildUp(ref context, selection);
        }

        protected override void BuildUp(ref ResolveContext context, ConstructorInfo info)
        {
            // Indicate that we are building constructor
            context.Activity = info;

            var parameters = info.GetParameters();
            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
                values[i] = context.Resolve(parameters[i]);

            // Activate instance
            context.Existing = info.Invoke(values);
        }

        protected override void BuildUp(ref ResolveContext context, ConstructorInfo info, object[] data)
        {
            // Indicate that we are building constructor
            context.Activity = info;

            var parameters = info.GetParameters();
            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
                values[i] = context.Resolve(parameters[i], data[i]);

            // Activate instance
            context.Existing = info.Invoke(values);
        }

    }
}
