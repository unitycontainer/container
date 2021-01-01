using System;

namespace Unity.BuiltIn
{
    public static partial class Selectors
    {
        /// <summary>
        /// Selects target Type during array resolution
        /// </summary>
        /// <param name="container">Container scope</param>
        /// <param name="element">Array element <see cref="Type"/></param>
        /// <returns><see cref="Type"/> of array's element</returns>
        public static Type ArrayTargetTypeSelector(UnityContainer container, Type element)
        {
            Type? next;
            Type? type = element;

            do
            {
                if (type.IsGenericType)
                {
                    if (container.Scope.Contains(type)) return type!;

                    var definition = type.GetGenericTypeDefinition();
                    if (container.Scope.Contains(definition)) return definition;

                    next = type.GenericTypeArguments[0]!;
                    if (container.Scope.Contains(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType()!;
                    if (container.Scope.Contains(next)) return next;
                }
                else
                {
                    return type!;
                }
            }
            while (null != (type = next));

            return element;
        }
    }
}
