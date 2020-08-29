using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationCategory
    {
        /// <summary>
        /// Initial, uninitialized state
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// This is implicit/internal registration
        /// </summary>
        Internal,

        /// <summary>
        /// This registration is a clone
        /// </summary>
        /// <remarks>
        /// In most cases this category implies that
        /// the Data field holds reference to a parent
        /// manager
        /// </remarks>
        Clone,

        /// <summary>
        /// This is RegisterType registration
        /// </summary>
        Type,

        /// <summary>
        /// This is RegisterInstance registration
        /// </summary>
        Instance,

        /// <summary>
        /// This is RegisterFactory registration
        /// </summary>
        Factory
    }

    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract class RegistrationManager : IEnumerable,
                                                IPolicySet
    {
        #region Invalid Value object

        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        #endregion


        #region Constructors

        public RegistrationManager(params InjectionMember[] members)
            => Add(members);

        #endregion


        #region Registration Data

        public bool RequireBuild { get; private set; }

        public object? Data { get; internal set; }

        public RegistrationCategory Category { get; internal set; }

        public InjectionConstructor? Constructor { get; private set; }

        public InjectionField? Fields { get; private set; }

        public InjectionProperty? Properties { get; private set; }

        public InjectionMethod? Methods { get; private set; }

        public InjectionMember? Other { get; private set; }

        #endregion


        #region Resolver

        public Delegate? ResolveDelegate { get; internal set; }
        
        #endregion


        #region Try Get Value
        /// <summary>
        /// Attempts to retrieve a value from the backing lifetime manager
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on lifetime 
        /// synchronization objects primitives.
        /// </remarks>
        /// <param name="lifetime">The lifetime container this manager is associated with</param>
        /// <returns>The object stored with the manager or <see cref="NoValue"/></returns>
        public abstract object? TryGetValue(ICollection<IDisposable> lifetime);

        #endregion


        #region Registration Types

        public Type? Type =>
            RegistrationCategory.Type == Category
                ? (Type?)Data
                : null;

        public object? Instance =>
            RegistrationCategory.Instance == Category
                ? Data
                : null;

        public ResolveDelegate<IResolveContext>? Factory =>
            RegistrationCategory.Factory == Category
                ? (ResolveDelegate<IResolveContext>?)Data
                : null;

        #endregion


        #region Initializers Support

        public void Add(InjectionMember member)
        {
            switch (member)
            {
                case InjectionConstructor ctor:
                    ctor.Next = Constructor;
                    Constructor = ctor;
                    break;

                case InjectionField field:
                    field.Next = Fields;
                    Fields = field;
                    break;

                case InjectionProperty property:
                    property.Next = Properties;
                    Properties = property;
                    break;

                case InjectionMethod method:
                    method.Next = Methods;
                    Methods = method;
                    break;

                default:
                    member.Next = Other;
                    Other = member;
                    break;
            }
        }

        public void Add(IEnumerable<InjectionMember> members)
        {
            foreach (var member in members) Add(member);
        }

        #endregion


        #region IEnumerable

        public IEnumerator<InjectionMember> GetEnumerator()
        {
            // Start with constructor (Only one constructor)
            if (null != Constructor) yield return Constructor;

            // Fields
            for (InjectionMember? member = Fields; null != member; member = member.Next)
                yield return member;

            // Properties
            for (InjectionMember? member = Properties; null != member; member = member.Next)
                yield return member;

            // Methods
            for (InjectionMember? member = Methods; null != member; member = member.Next)
                yield return member;

            // Other
            for (InjectionMember? member = Other; null != member; member = member.Next)
                yield return member;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion


        #region Clone

        protected virtual void CloneData(RegistrationManager manager, InjectionMember[]? members = null)
        {
            Data        = manager;
            Category    = RegistrationCategory.Clone;

            Other       = manager.Other;
            Fields      = manager.Fields;
            Methods     = manager.Methods;
            Properties  = manager.Properties;
            Constructor = manager.Constructor;

            if (null != members && 0 != members.Length) Add(members);
        }

        #endregion


        #region IPolicySet 

        object? IPolicySet.Get(Type policyInterface)
        {
            //foreach (var member in InjectionMembers)
            //{
            //    if (member.GetType() == policyInterface)
            //        return member;
            //}

            return null;
        }

        void IPolicySet.Set(Type policyInterface, object policy) => throw new NotImplementedException();

        void IPolicySet.Clear(Type policyInterface) => throw new NotImplementedException();

        #endregion


        #region Nested Types

        public sealed class InvalidValue
        {
            internal InvalidValue()
            {
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion
    }
}
