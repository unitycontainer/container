using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Factories
{
    public class FuncResolver
    {
        #region Fields

        private static readonly MethodInfo ImplementationMethod
            = typeof(FuncResolver).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(ResolverImplementation))!;
        #endregion


        #region TypeResolverFactory

        public static TypeFactoryDelegate Factory = (Type type, UnityContainer container) =>
        {
            var typeToBuild = type.GetTypeInfo().GenericTypeArguments[0];
            var factoryMethod = ImplementationMethod.MakeGenericMethod(typeToBuild);

            return (ResolveDelegate<PipelineContext>)factoryMethod.CreateDelegate(typeof(ResolveDelegate<PipelineContext>));
        };

        #endregion


        #region Implementation

        // TODO: Add PerResolve handler
        private static Func<T> ResolverImplementation<T>(ref PipelineContext context)
        {
            var nameToBuild = context.Name;
            var container = context.Container;

            return () => 
            {
                var result = container.Resolve(typeof(T), nameToBuild);
                return null == result ? default : (T)result; 
            };
        }
        
        #endregion
    }
}
