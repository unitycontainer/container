using System;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Factories
{
    public class FuncResolver
    {
        #region Fields

        private static readonly MethodInfo ImplementationMethod
            = typeof(FuncResolver).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(ResolverImplementation));
        #endregion


        #region TypeResolverFactory

        public static TypeFactoryDelegate Factory = (Type type, UnityContainer container) =>
        {
            var typeToBuild = type.GetTypeInfo().GenericTypeArguments[0];
            var factoryMethod = ImplementationMethod.MakeGenericMethod(typeToBuild);

            return (ResolveDelegate<BuilderContext>)factoryMethod.CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
        };

        #endregion


        #region Implementation

        // TODO: Add PerResolve handler
        private static Func<T> ResolverImplementation<T>(ref BuilderContext context)
        {
            var nameToBuild = context.Name;
            var container = context.Container;

            return () => (T)container.Resolve(typeof(T), nameToBuild);
        }
        
        #endregion
    }
}
