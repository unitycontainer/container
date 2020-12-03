using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    public static class Matching
    {
        public static MatchRank MatchTo(this object? value, Type target)
        {
            switch (value)
            {
                case null:
                    return !target.IsValueType || (null != Nullable.GetUnderlyingType(target))
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return MatchTo(array, target);

                case IMatch<Type> iMatchType:
                    return iMatchType.Match(target);

                case Type type:
                    return MatchTo(type, target);

                case IResolve:
                case PipelineFactory:
                case IInjectionProvider:
                case IResolverFactory<Type>:
                case ResolveDelegate<PipelineContext>:
                    return MatchRank.HigherProspect;
            }

            var objectType = value.GetType();

            if (objectType == target)
                return MatchRank.ExactMatch;

            return target.IsAssignableFrom(objectType) 
                ? MatchRank.Compatible : MatchRank.NoMatch;
        }

        public static MatchRank MatchTo(this object value, ParameterInfo parameter)
        {
            switch (value)
            {
                case null:
                    return !parameter.ParameterType.IsValueType || (null != Nullable.GetUnderlyingType(parameter.ParameterType))
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return MatchTo(array, parameter.ParameterType);

                case IMatch<ParameterInfo> iMatchParam:
                    return iMatchParam.Match(parameter);

                case Type type:
                    return MatchTo(type, parameter.ParameterType);

                case IResolve:
                case PipelineFactory:
                case IInjectionProvider:
                case IResolverFactory<Type>:
                case ResolveDelegate<PipelineContext>:
                    return MatchRank.HigherProspect;
            }

            var objectType = value.GetType();

            if (objectType == parameter.ParameterType)
                return MatchRank.ExactMatch;

            return parameter.ParameterType.IsAssignableFrom(objectType)
                ? MatchRank.Compatible : MatchRank.NoMatch;
        }

        public static MatchRank MatchTo(this Array array, Type target)
        {
            var type = array.GetType();

            if (target == type)          return MatchRank.ExactMatch;
            if (target == typeof(Array)) return MatchRank.HigherProspect;

            return target.IsAssignableFrom(type)
                ? MatchRank.Compatible
                : MatchRank.NoMatch;
        }

        public static MatchRank MatchTo(this Type type, Type match)
        {
            if (typeof(Type).Equals(match))
                return MatchRank.ExactMatch;

            if (type == match || Nullable.GetUnderlyingType(type) == match)
                return MatchRank.HigherProspect;

            if (match.IsAssignableFrom(type))
                return MatchRank.Compatible;

            if (typeof(Array) == type && match.IsArray)
                return MatchRank.HigherProspect;

            if (type.IsGenericType && type.IsGenericTypeDefinition && match.IsGenericType &&
                type.GetGenericTypeDefinition() == match.GetGenericTypeDefinition())
                return MatchRank.ExactMatch;

            return MatchRank.NoMatch;
        }
    }
}
