using System;
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
        /// <param name="context">A <see cref="BuildContext"/> structure holding 
        /// resolution context</param>
        public override void PreBuildUp(ref BuildContext context)
        {
            // If instance already exists skip activation
            if (null != context.Existing) return;


            // Type to build
            Type type = (Type?)context.Data ?? context.Contract.Type;
            var ctors = type.GetConstructors(SupportedBindingFlags);
                            //.Where(SupportedMembersPredicate)
                            //.ToList();

            //context.Existing = new object(); return;


            // Invoke injected constructor if available
            if (null != context.Manager?.Constructor)
            {
                var ctor = context.Manager.Constructor;

                context.MemberInfo = ctor.MemberInfo(ctors);

                // BuildUp if found
                if (null != context.MemberInfo)
                {
                    // Resolve otherwise
                    if (null == ctor.Data || 0 == ctor.Data.Length)
                        BuildUp(ref context);
                    else
                    { 
                        BuildUp(ref context, ctor.Data);
                    }

                    return;
                }
            }

            // Check if selection is required
            switch (ctors.Length)
            {
                case 0:
                    throw new InvalidOperationException(NoConstructor);
                
                case 1:
                    context.MemberInfo = ctors[0];
                    BuildUp(ref context);
                    return;
            }

            // Search for and invoke constructor annotated with attribute
            foreach (var ctor in ctors)
            {
                if (IsAnnotated(ctor))
                {
                    context.MemberInfo = ctor;
                    BuildUp(ref context);
                    return;
                }
            }

            // Use algorithm to select constructor
            //context.MemberInfo = SelectConstructor(ctors, ref context);

            // Activate or throw if not found
            if (null == context.MemberInfo) throw new InvalidOperationException(NoConstructor);

            BuildUp(ref context);
        }

        protected override void BuildUp(ref BuildContext context)
        {
            var info = (ConstructorInfo)context.MemberInfo!;
            var parameters = info.GetParameters();
            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
            { 
                //values[i] = context.Resolve(parameters[i]);
            }

            // Activate instance
            context.Existing = info.Invoke(values);
        }

        protected override void BuildUp(ref BuildContext context, object[] data)
        {
            var info = (ConstructorInfo)context.MemberInfo!;
            var parameters = info.GetParameters();
            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
            { 
                //values[i] = context.Resolve(parameters[i], data[i]);
            }

            // Activate instance
            context.Existing = info.Invoke(values);
        }

    }
}
