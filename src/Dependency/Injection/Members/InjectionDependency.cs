using System;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    public class InjectionDependency : InjectionMember, 
                                       IEquatable<ParameterInfo>,
                                       IResolve

    {
        #region Fields

        private readonly Type   _type;
        private readonly string _name;
        private readonly object _value;

        #endregion


        #region Constructors

        public InjectionDependency(object value)
        {
            _value = value ?? 
                throw new ArgumentNullException(
                    $"value 'null' only allowed when Type is specified");
        }

        public InjectionDependency(Type type, object value)
        {
            _type = type;
            _value = value;
        }

        public InjectionDependency(Type type, string name, object value)
        {
            _type = type;
            _name = name;
            _value = value;
        }

        #endregion


        #region IEquatable

        public bool Equals((Type, string) other)
        {
            return (null == _type || other.Item1 == _type) &&
                   (null == _name || other.Item2 == _name);
        }

        public bool Equals(ParameterInfo other)
        {
            return (null == _type || other.ParameterType == _type) &&
                   (null == _name || other.Name == _name);
        }

        #endregion


        #region IResolve

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            return _value;
        }
        
        #endregion
    }


    /// <summary>
    /// A convenience version of <see cref="InjectionDependency"/> that lets you
    /// specify the dependency type using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency to override.</typeparam>
    public class InjectionDependency<T> : InjectionDependency
    {
        /// <summary>
        /// Construct a new <see cref="DependencyOverride{T}"/> object that will
        /// override the given dependency, and pass the given value.
        /// </summary>
        public InjectionDependency(object value)
            : base(typeof(T), value)
        {
        }
    }
}
