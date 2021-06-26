using System;
using System.Reflection;
using Unity.Injection;
using Unity.Resolution;

namespace Regression
{
    public class ValidatingResolverFactory : IResolverFactory<Type>, 
                                             IResolverFactory<ParameterInfo>,
                                             IEquatable<Type>
    {
        private object _value;

        public ValidatingResolverFactory(object value)
        {
            _value = value;
        }

        public Type Type { get; private set; }

        public string Name { get; private set; }

        public ResolveDelegate<TContext> GetResolver<TContext>(Type info)
            where TContext : IResolveContext
        {
            return (ref TContext context) =>
            {
                Type = context.Type;
                Name = context.Name;

                return _value;
            };
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext => GetResolver<TContext>(info.ParameterType);

        public bool Equals(Type other) => _value.Matches(other);
    }
}


