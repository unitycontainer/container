using System.Reflection;

namespace Unity
{
    public sealed partial class UnityContainer
    {
        #region Fields

        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();

        #endregion

        public bool CanResolve(Type type, string? name)
        {
            // TODO: Verify benchmarks
            if (IsRegistered(type, name)) return true;

            if (type.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return IsRegistered(type, name) || CanResolve(type.GetElementType()!, name);
                }

                // Can resolve if a factory is registered
                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();
                    if (Policies.Contains(genericType, typeof(FactoryPipeline)))
                        return true;
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(type) ||
                    typeof(string) == type ||
                    type.IsEnum ||
                    type.IsPrimitive ||
                    type.IsAbstract ||
                    !type.GetTypeInfo().DeclaredConstructors
                         .Any(c => !c.IsFamily && !c.IsPrivate))
                { 
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
