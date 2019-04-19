using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Factories
{
    public class ArrayResolver
    {
        #region Fields

        private static readonly MethodInfo ResolveMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.GetArray));

        #endregion


        #region ResolveDelegateFactory


        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            var typeArgument = context.RegistrationType.GetElementType();
            var targetType = ((UnityContainer)context.Container).GetTargetType(typeArgument);

            // Simple types
            var method = (ResolveArray)
                ResolveMethod.MakeGenericMethod(typeArgument)
                             .CreateDelegate(typeof(ResolveArray));

            return (ref BuilderContext c) => method(c.Resolve, c.Resolve);
        };

        #endregion


        #region Implementation

        
        #endregion


        #region Nested Types


        internal delegate object ResolveArray(Func<Type, string, object> resolve, Func<Type, string, InternalRegistration, object> resolveRegistration);

        #endregion
    }
}
