using System;
using System.Reflection;

namespace Unity.Injection
{
    public static class InjectionComparison
    {
        public static int CompareTo(this object value, ParameterInfo parameter)
        {
            switch (value)
            {
                case null:
                    return !parameter.ParameterType.IsValueType() || (null != Nullable.GetUnderlyingType(parameter.ParameterType))
                         ? 0 : -1;

                case IComparable<ParameterInfo> toParam:
                    return toParam.CompareTo(parameter);

                case IComparable<Type> toType:
                    return toType.CompareTo(parameter.ParameterType);

                case Type valueType:
                    return CompareTo(valueType, parameter.ParameterType);
            }

            var type = value.GetType();

            if (type == parameter.ParameterType)
                return 0;

            return parameter.ParameterType.IsAssignableFrom(type) 
                ? 1 : -1;
        }


        public static int CompareTo(this Type type, Type match)
        {
            if (typeof(Type).Equals(match))
                return 0;

            if (type == match)
                return 2;

            if (match.IsAssignableFrom(type))
                return 1;

            if (typeof(Array) == type && match.IsArray)
                return 1;

            if (type.IsGenericType() && type.IsGenericTypeDefinition() && match.IsGenericType() &&
                type.GetGenericTypeDefinition() == match.GetGenericTypeDefinition())
                return 1;

            return -1;
        }
    }
}
