using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract partial class MemberInfoProcessor<TMemberInfo, TData>
    {
        /// <summary>
        /// Combination of <see cref="BindingFlags"/> used to get declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }


        protected Func<TMemberInfo, DependencyInfo> GetDependencyInfo { get; private set; }

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        protected abstract TMemberInfo[] GetMembers(Type type);


        #region BuildUp

        protected virtual object? BuildUp(ref ResolutionContext context, TData? data = null) => throw new NotImplementedException();

        #endregion
    }
}
