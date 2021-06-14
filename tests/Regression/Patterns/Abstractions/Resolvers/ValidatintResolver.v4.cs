using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Regression
{
    public class ValidatingResolver : InjectionParameterValue, IDependencyResolverPolicy
    {
        private object _value;

        public ValidatingResolver(object value) 
            => _value = value;

        public object Resolve(IBuilderContext context)
        {
            Type = context.OriginalBuildKey.Type;
            Name = context.OriginalBuildKey.Name;
            return _value;
        }

        public override bool MatchesType(Type t)
        {
            if (_value is null) return false;

            return t.IsAssignableFrom(_value.GetType());
        }

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild) 
            => this;

        public override string ParameterTypeName => _value?.GetType().Name;

        public Type Type { get; private set; }

        public string Name { get; private set; }
    }
}


