using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    internal class EnumerableResolver
    {
        #region Fields

        private delegate object ResolveEnumerableDelegate(ref BuilderContext context, Type type);

        private static readonly MethodInfo ResolveMethod = 
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveEnumerable));
        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
            var typeArgument = context.Type.GetTypeInfo().GenericTypeArguments.First();
#else
            var typeArgument = context.Type.GenericTypeArguments.First();
#endif
            return (ResolveDelegate<BuilderContext>)
                ResolveMethod.MakeGenericMethod(typeArgument)
                              .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
        };

        #endregion
    }
}
