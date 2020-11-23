using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
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
        /// Collection of cached metadata
        /// </summary>
        Cache,

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
        /// This is implicit/internal registration
        /// </summary>
        Internal,

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


        #region Source

        public virtual ImportSource Source => ImportSource.Any;

        #endregion


        #region Registration Data

        public bool RequireBuild { get; private set; }

        public object? Data { get; internal set; }

        public RegistrationCategory Category { get; internal set; }

        public InjectionMethodBase<ConstructorInfo>? Constructor { get; private set; }

        public InjectionMemberInfo<FieldInfo>? Fields { get; private set; }

        public InjectionMemberInfo<PropertyInfo>? Properties { get; private set; }

        public InjectionMethodBase<MethodInfo>? Methods { get; private set; }

        public InjectionMember? Other { get; private set; }

        public InjectionMember? GetInjected<TMember>()
        {
            return (typeof(TMember)) switch
            {
                Type t when t == typeof(ConstructorInfo) => Constructor,
                Type t when t == typeof(FieldInfo)       => Fields,
                Type t when t == typeof(PropertyInfo)    => Properties,
                Type t when t == typeof(MethodInfo)      => Methods,
                _ => Other,
            };
        }


        #endregion


        #region Resolver

        public virtual ResolveDelegate<PipelineContext>? Pipeline { get; internal set; }
        
        #endregion


        #region Try/Get/Set Value

        /// <summary>
        /// Attempts to retrieve a value from the backing lifetime manager
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on lifetime 
        /// synchronization objects primitives.
        /// </remarks>
        /// <param name="scope">The lifetime container this manager is associated with</param>
        /// <returns>The object stored with the manager or <see cref="NoValue"/></returns>
        public virtual object? TryGetValue(ICollection<IDisposable> scope) => NoValue;

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="scope">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? GetValue(ICollection<IDisposable> scope) => NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="scope">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ICollection<IDisposable> scope) { }

        #endregion


        #region Registration Types

        public Type? Type =>
            RegistrationCategory.Type == Category
                ? (Type?)Data
                : null;

        public object? Instance =>
            RegistrationCategory.Instance == Category
                ? Data
                : NoValue;

        public Func<IUnityContainer, Type, string?, ResolverOverride[], object?>? Factory =>
            RegistrationCategory.Factory == Category
                ? (Func<IUnityContainer, Type, string?, ResolverOverride[], object?>?)Data
                : null;

        #endregion


        #region Initializers Support

        public void Add(InjectionMember member)
        {
            switch (member)
            {
                case InjectionMethodBase<ConstructorInfo> ctor:
                    ctor.Next = Constructor;
                    Constructor = ctor;
                    break;

                case InjectionMemberInfo<FieldInfo> field:
                    field.Next = Fields;
                    Fields = field;
                    break;

                case InjectionMemberInfo<PropertyInfo> property:
                    property.Next = Properties;
                    Properties = property;
                    break;

                case InjectionMethodBase<MethodInfo> method:
                    method.Next = Methods;
                    Methods = method;
                    break;

                default:
                    member.Next = Other;
                    Other = member;
                    break;
            }

            RequireBuild |= member.BuildRequired;
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

            Other        = manager.Other;
            Fields       = manager.Fields;
            Methods      = manager.Methods;
            Properties   = manager.Properties;
            Constructor  = manager.Constructor;
            RequireBuild = manager.RequireBuild;

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


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object? obj) 
                => ReferenceEquals(this, obj);

            public override int GetHashCode() 
                => base.GetHashCode();
        }

        #endregion
    }


    public static class RegistrationManagerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNoValue(this object? other) 
            => ReferenceEquals(other, RegistrationManager.NoValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValue(this object? other)
        => !ReferenceEquals(other, RegistrationManager.NoValue);
    }
}
