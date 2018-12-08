using System;

namespace Unity.Dependency
{
    public class DependencyValue : IDependencyValue
    {
        #region Constructors

        public DependencyValue(Type target, Type type, string name, object value)
        {
            Target = target;
            Type = type;
            Name = name;
            Value = value;
        }

        #endregion


        #region IDependencyValue

        public Type Target { get; }

        public Type Type { get; }

        public string Name { get; }

        public virtual object Value { get; }

        #endregion

    }
}
