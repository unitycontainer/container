using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Pipeline
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberInfoProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Delegates

        /// <summary>
        /// A predicate that determines if the <see cref="MethodBase"/> member has been
        /// annotated with one of injection attributes
        /// </summary>
        /// <remarks>
        /// By default the container recognizes two attributes: <see cref="InjectionConstructorAttribute"/>
        /// and <see cref="InjectionMethodAttribute"/>
        /// </remarks>
        /// <param name="member"><see cref="MethodBase"/> derived member</param>
        /// <returns>True if member has been annotated with one of the recognized attributes</returns>
        public delegate bool AnnotatedForInjectionPredicate(TMemberInfo member);
        
        #endregion


        #region Fields

        protected AnnotatedForInjectionPredicate IsAnnotated;

        #endregion


        #region Constructors

        public MethodBaseProcessor(Defaults defaults)
            :base(defaults)
        {
            // Add annotation predicate to default policies and subscribe to notifications
            var predicate = (AnnotatedForInjectionPredicate?)defaults.Get(typeof(AnnotatedForInjectionPredicate));
            if (null == predicate)
            {
                IsAnnotated = DefaultAnnotationPredicate;
                defaults.Set(typeof(AnnotatedForInjectionPredicate), 
                                   (AnnotatedForInjectionPredicate)DefaultAnnotationPredicate,
                                   (policy) => IsAnnotated = (AnnotatedForInjectionPredicate)policy);
            }
            else
                IsAnnotated = predicate;
        }

        #endregion


        #region Implementation

        protected override bool DefaultSupportedPredicate(TMemberInfo member) => !member.IsFamily && !member.IsPrivate;

        protected abstract bool DefaultAnnotationPredicate(TMemberInfo member);

        #endregion
    }
}
