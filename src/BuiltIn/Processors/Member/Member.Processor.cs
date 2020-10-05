using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
                                                                 where TData       : class
    {
        #region Constants

        /// <summary>
        /// Binding flags used to obtain declared members by default
        /// </summary>
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        #endregion


        #region Fields

        /// <summary>
        /// Combination of <see cref="BindingFlags"/> to use when getting declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults)
        {
            BindingFlags = defaults.GetOrAdd(typeof(TMemberInfo), DefaultBindingFlags, 
                (object flags) => BindingFlags = (BindingFlags)flags);
        }

        #endregion
    }
}
