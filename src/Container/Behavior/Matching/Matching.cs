using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    internal static class Matching
    {
        public static void Initialize(ExtensionContext context)
        {

        }

        public static int MatchTo(object[]? data, MethodBase? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)data![i].MatchTo(parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }


        public static MatchRank MatchTo(this object? value, Type target)
        {
            switch (value)
            {
                case null:
                    return !target.IsValueType || null != Nullable.GetUnderlyingType(target)
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return array.MatchTo(target);

                case IMatch<Type, MatchRank> iMatchType:
                    return iMatchType.Match(target);

                case Type type:
                    return type.MatchTo(target);

                case IResolve:
                case PipelineFactory<PipelineContext>:
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
                    return !parameter.ParameterType.IsValueType || null != Nullable.GetUnderlyingType(parameter.ParameterType)
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return array.MatchTo(parameter.ParameterType);

                case IMatch<ParameterInfo, MatchRank> iMatchParam:
                    return iMatchParam.Match(parameter);

                case Type type:
                    return type.MatchTo(parameter.ParameterType);

                case IResolve:
                case PipelineFactory<PipelineContext>:
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

            if (target == type) return MatchRank.ExactMatch;
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
