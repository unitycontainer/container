using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    internal class FuncResolver
    {
        #region Fields

        private static readonly MethodInfo ImplementationMethod
            = typeof(FuncResolver).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(ResolverImplementation));
        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            var typeToBuild = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var factoryMethod = ImplementationMethod.MakeGenericMethod(typeToBuild);

            return (ResolveDelegate<BuilderContext>)factoryMethod.CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
        };

        #endregion


        #region Implementation

        private static Func<T> ResolverImplementation<T>(ref BuilderContext context)
        {
            var nameToBuild = context.Name;
            var container = context.Container;

            return () => (T)container.Resolve(typeof(T), nameToBuild);
        }
        
        #endregion
    }
}
