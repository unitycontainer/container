using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Matching
    {
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
                case PipelineFactory<BuilderContext>:
                case IInjectionProvider:
                case IResolverFactory<Type>:
                case ResolveDelegate<BuilderContext>:
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
                case PipelineFactory<BuilderContext>:
                case IInjectionProvider:
                case IResolverFactory<Type>:
                case ResolveDelegate<BuilderContext>:
                    return MatchRank.HigherProspect;
            }

            var objectType = value.GetType();

            if (objectType == parameter.ParameterType)
                return MatchRank.ExactMatch;

            return parameter.ParameterType.IsAssignableFrom(objectType)
                ? MatchRank.Compatible : MatchRank.NoMatch;
        }
    }
}
