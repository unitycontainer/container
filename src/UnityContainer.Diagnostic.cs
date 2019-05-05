using System;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region BuilderContext

        internal BuilderContext.ResolvePlanDelegate ContextResolvePlan { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        internal static object ContextValidatingResolvePlan(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
        {
            if (null == resolver) throw new ArgumentNullException(nameof(resolver));
#if NET40
            return resolver(ref thisContext);
#else
            unsafe
            {
                var parent = thisContext.Parent;
                while (IntPtr.Zero != parent)
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (thisContext.RegistrationType == parentRef.RegistrationType && thisContext.Name == parentRef.Name)
                        throw new InvalidOperationException($"Circular reference for Type: {thisContext.Type}, Name: {thisContext.Name}",
                            new CircularDependencyException());

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    ContainerContext = thisContext.ContainerContext,
                    Registration = thisContext.Registration,
                    RegistrationType = thisContext.Type,
                    Name = thisContext.Name,
                    Type = thisContext.Type,
                    Compose = thisContext.Compose,
                    ResolvePlan = thisContext.ResolvePlan,
                    List = thisContext.List,
                    Overrides = thisContext.Overrides,
                    DeclaringType = thisContext.Type,
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
                };

                return resolver(ref context);
            }
#endif
        }

        #endregion
    }
}
