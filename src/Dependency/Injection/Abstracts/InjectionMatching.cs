using System;
using System.Reflection;

namespace Unity.Injection
{
    public static class InjectionMatching
    {
        #region Signature matching

        public static bool MatchMemberInfo(this object[] data, MethodBase info)
        {
            var parameters = info.GetParameters();

            if ((data?.Length ?? 0) != parameters.Length) return false;

            for (var i = 0; i < (data?.Length ?? 0); i++)
            {
                if (Matches(data?[i], parameters[i].ParameterType))
                    continue;

                return false;
            }

            return true;
        }

        #endregion


        #region Data Matching

        public static bool Matches(this object data, Type match)
        {
            switch (data)
            {
                case IEquatable<Type> equatable:
                    return equatable.Equals(match);

                case Type type:
                    return MatchesType(type, match);

                default:
                    return MatchesObject(data, match);
            }
        }

        public static bool MatchesType(this Type type, Type match)
        {
            if (null == type) return true;

            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            if (matchInfo.IsAssignableFrom(typeInfo)) return true;
            if ((typeInfo.IsArray || typeof(Array) == type) &&
               (matchInfo.IsArray || match == typeof(Array)))
                return true;

            if (typeInfo.IsGenericType && typeInfo.IsGenericTypeDefinition && matchInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == matchInfo.GetGenericTypeDefinition())
                return true;

            return false;
        }

        public static bool MatchesObject(this object parameter, Type match)
        {
            var type = parameter is Type ? typeof(Type) : parameter?.GetType();

            if (null == type) return true;

            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            if (matchInfo.IsAssignableFrom(typeInfo)) return true;
            if ((typeInfo.IsArray || typeof(Array) == type) &&
                (matchInfo.IsArray || match == typeof(Array)))
                return true;

            if (typeInfo.IsGenericType && typeInfo.IsGenericTypeDefinition && matchInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == matchInfo.GetGenericTypeDefinition())
                return true;

            return false;
        }

        #endregion

    }
}
