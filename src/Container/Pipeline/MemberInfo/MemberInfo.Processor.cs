using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Injection;

namespace Unity.Pipeline
{
    public abstract partial class MemberInfoProcessor<TMemberInfo, TData> : PipelineProcessor
                                                        where TMemberInfo : MemberInfo
                                                        where TData       : class
    {
        #region Fields

        protected readonly BindingFlags SupportedBindingFlags;
        protected Func<TMemberInfo, bool> SupportedMembersPredicate;

        #endregion


        #region Constructors

        public MemberInfoProcessor(Defaults defaults) 
            : this(defaults, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        { }

        public MemberInfoProcessor(Defaults defaults, BindingFlags flags)
        {
            SupportedBindingFlags     = flags;
            SupportedMembersPredicate = DefaultSupportedPredicate;
        }

        #endregion


        #region Declared Members API

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

        /// <summary>
        /// Predicate that determines if current <see cref="MemberInfo"/> object
        /// is supported by Unity Container.
        /// </summary>
        /// <remarks>
        /// The predicate examines <see cref="MemberInfo"/> information and filters
        /// out members with unsupported features
        /// </remarks>
        /// <param name="member"><see cref="MemberInfo"/> to examine</param>
        /// <returns>True if member is supported, otherwise False</returns>
        protected virtual bool DefaultSupportedPredicate(TMemberInfo member) => true;

        /// <summary>
        /// Creates an <see cref="Enumerator"/> that enumerates over supported members
        /// </summary>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>An <see cref="Enumerator{TMemberInfo}"/> containing supported members</returns>
        protected virtual IEnumerable<TMemberInfo> DeclaredMembers(Type type) 
            => GetMembers(type).Where(SupportedMembersPredicate);

        // TODO: might be optimized

        #endregion


        #region Activation

        protected virtual void BuildUp(ref ResolveContext context, TMemberInfo info) => throw new NotImplementedException();
        protected virtual void BuildUp(ref ResolveContext context, TMemberInfo info, TData data) => throw new NotImplementedException();

        #endregion
    }
}
