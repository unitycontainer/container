using System;

namespace Unity.BuiltIn
{
    public static class ArrayTypeSelector

    {
        #region Selection

        public static Type Selector(UnityContainer container, Type argType)
        {
            Type? next;
            Type? type = argType;

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

            return argType;
        }

        #endregion
    }
}
