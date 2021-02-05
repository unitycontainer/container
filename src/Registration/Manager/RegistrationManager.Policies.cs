using System;
using System.Collections;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager// : IEnumerable
    {
        #region PolicySet


        /// <inheritdoc />
        public void Clear(Type _) => throw new NotImplementedException();

        // TODO: not necessary
        /// <inheritdoc />
        public object? Get(Type type)
        {
            // TODO: Sequence
            //for (var policy = Policies; policy is not null; policy = policy.Next as InjectionMethod)
            //{
            //    if (policy is PolicyWrapper wrapper)
            //    {
            //        if (type.IsAssignableFrom(wrapper.Item1))
            //            return wrapper.Item2;
            //    }
            //    else
            //    {
            //        if (type.IsAssignableFrom(policy.GetType()))
            //            return policy;
            //    }
            //}

            return null;
        }


        /// <inheritdoc />
        public void Set(Type type, object policy)
        {
            // TODO: Sequence
            //if (policy is InjectionMember member && type == policy.GetType())
            //{
            //    member.Next = Policies;
            //    Policies = member;
                
            //    return;
            //}
        }

        #endregion


        #region IEnumerable

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    // TODO: Sequence
        //    throw new NotImplementedException();
        //    //for (var member = Policies;
        //    //         member is not null;
        //    //         member = member.Next)
        //    //{
        //    //    yield return member;
        //    //}
        //}

        #endregion
    }
}
