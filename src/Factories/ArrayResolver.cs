using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    public class ArrayResolver
    {
        #region Fields

        private static readonly MethodInfo ResolveMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveArray));

        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            var typeArgument = context.RegistrationType.GetElementType();

            return (ResolveDelegate<BuilderContext>)
                ResolveMethod.MakeGenericMethod(typeArgument)
                             .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
        };

        #endregion


        #region Resolver

        public static ResolveDelegate<BuilderContext> Resolver = (ref BuilderContext context) =>
        {
            return null;
        };

        #endregion
    }
}
