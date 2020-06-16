using System;
using System.Collections.Generic;
using System.Linq;
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
                if (Matches(data![i], parameters[i].ParameterType))
                    continue;

                return false;
            }

            return true;
        }

        #endregion


        #region Data Matching

        public static bool Matches(this object data, Type match)
        {
            return data switch
            {
                null => (null == match) || !match.IsValueType() || (null != Nullable.GetUnderlyingType(match)),
                Type _ when typeof(Type).Equals(match) => true,
                Type type                              => MatchesType(type, match),
                IMatch<Type> equatable                 => equatable.Match(match),
                _                                      => MatchesObject(data, match),
            };
        }

        public static bool MatchesType(this Type type, Type match)
        {
            if (null == type) return true;

            if (match.IsAssignableFrom(type)) 
                return true;

            if (typeof(Array) == type && match.IsArray)
                return true;

            if (type.IsGenericType() && type.IsGenericTypeDefinition() && match.IsGenericType() &&
                type.GetGenericTypeDefinition() == match.GetGenericTypeDefinition())
                return true;

            return false;
        }

        public static bool MatchesObject(this object data, Type match)
        {
            var type = data is Type ? typeof(Type) : data?.GetType();

            if (null == type) return true;

            return match.IsAssignableFrom(type);
        }

        #endregion


        #region Error Reporting

        public static string Signature(this object[] data)
        {
            return string.Join(", ", data?.Select(GetSignature) ?? Enumerable.Empty<string>());

            string GetSignature(object param)
            {
                if (null == param) return "null";
                if (param is Type) return $"Type {param}";

                return $"{param.GetType().Name} {param}";
            }
        }


        public static string Signature(this MethodBase selection)
        {
            var sb = new List<string>();
            foreach (var parameter in selection.GetParameters())
                sb.Add($"{parameter.ParameterType} {parameter.Name}");

            return string.Join(", ", sb);
        }


        #endregion
    }
}
