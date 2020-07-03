using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationType
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
    public abstract class RegistrationManager : IEnumerable<InjectionMember>, 
                                                IPolicySet
    {
        #region Constructors

        public RegistrationManager(params InjectionMember[] members) 
            => InjectionMembers = members;

        #endregion


        #region Registration Data

        public object? Data { get; internal set; }

        public RegistrationType RegistrationType { get; internal set; }

        public ICollection<InjectionMember> InjectionMembers { get; protected set; }

        #endregion


        #region Initializers Support

        public IEnumerator<InjectionMember> GetEnumerator() 
            => InjectionMembers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable)InjectionMembers).GetEnumerator();

        public void Add(InjectionMember member)
        {
            if (!(InjectionMembers is List<InjectionMember> list))
            {
                list = new List<InjectionMember>(InjectionMembers);
                InjectionMembers = list;
            }

            list.Add(member);
        }

        public void Add(ICollection<InjectionMember> members)
        {
            if (0 == InjectionMembers.Count)
            {
                InjectionMembers = members;
            }
            else if (InjectionMembers is List<InjectionMember> list)
            {
                foreach (var member in members) list.Add(member);
            }
            else
            {
                InjectionMembers = InjectionMembers.Concat(members)
                                                   .ToList();
            }
        }

        #endregion


        #region IPolicySet 

        object? IPolicySet.Get(Type policyInterface)
        {
            foreach (var member in InjectionMembers)
            {
                if (member.GetType() == policyInterface)
                    return member;
            }

            return null;
        }

        void IPolicySet.Set(Type policyInterface, object policy) => throw new NotImplementedException();

        void IPolicySet.Clear(Type policyInterface) => throw new NotImplementedException();

        #endregion
    }
}
