using System.Collections.Generic;

namespace Unity.Builder.Selection
{
    /// <summary>
    /// Base class for return of selector policies that need
    /// to keep track of a set of parameter resolvers.
    /// </summary>
    public class SelectedMemberWithParameters
    {
        private readonly List<object> _parameterResolvers;

        public SelectedMemberWithParameters()
        {
            _parameterResolvers = new List<object>();
        }

        public SelectedMemberWithParameters(IEnumerable<object> resolvers)
        {
            _parameterResolvers = new List<object>(resolvers);
        }

        /// <summary>
        /// Adds the parameter resolver. Resolvers are assumed
        /// to be in the order of the parameters to the member.
        /// </summary>
        /// <param name="newResolver">The new resolver.</param>
        public void AddParameterResolver(object newResolver)
        {
            _parameterResolvers.Add(newResolver);
        }

        /// <summary>
        /// Gets the parameter resolvers.
        /// </summary>
        /// <returns>An array with the parameter resolvers.</returns>
        public object[] GetParameterResolvers()
        {
            return _parameterResolvers.ToArray();
        }
    }

    /// <summary>
    /// Base class for return values from selector policies that
    /// return a MemberInfo of some sort plus a list of parameter
    /// keys to look up the parameter resolvers.
    /// </summary>
    public class SelectedMemberWithParameters<TMemberInfoType> : SelectedMemberWithParameters
    {
        /// <summary>
        /// Construct a new <see cref="SelectedMemberWithParameters{TMemberInfoType}"/>, storing
        /// the given member info.
        /// </summary>
        /// <param name="memberInfo">Member info to store.</param>
        protected SelectedMemberWithParameters(TMemberInfoType memberInfo)
        {
            MemberInfo = memberInfo;
        }

        protected SelectedMemberWithParameters(TMemberInfoType memberInfo, IEnumerable<object> resolvers)
            : base(resolvers)
        {
            MemberInfo = memberInfo;
        }

        /// <summary>
        /// The member info stored.
        /// </summary>
        public TMemberInfoType MemberInfo { get; }
    }
}
