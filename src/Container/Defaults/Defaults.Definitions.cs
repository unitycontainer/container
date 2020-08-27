using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Constants

        /// <summary>
        /// Size of <see cref="PipelineContext"/> and <see cref="ResolveContext"/> 
        /// structures. This size is for x64 process, when running in 32 bit mode
        /// extra bytes are not used.
        /// </summary>
        public const int CONTEXT_STRUCT_SIZE = 56;

        /// <summary>
        /// Default name of root container
        /// </summary>
        public const string DEFAULT_ROOT_NAME = "root";

        /// <summary>
        /// Default capacity of root container
        /// </summary>
        public const int DEFAULT_ROOT_CAPACITY = 37;

        #endregion


        #region Delegates

        public delegate ResolveDelegate<ResolveContext> BalancedFactoryDelegate(in Contract contract, RegistrationManager? manager = null);

        public delegate ResolveDelegate<ResolveContext> OptimizedFactoryDelegate(in Contract contract, RegistrationManager? manager = null);

        public delegate ResolveDelegate<ResolveContext> UnregisteredFactoryDelegate(ref ResolveContext context);

        public delegate bool ValidMemberInfoPredicate<T>(T member) where T : MemberInfo;

        public delegate void PolicyChangeNotificationHandler(object policy);

        #endregion


        #region Marker Types

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Type"/> policies
        /// </summary>
        public class TypeCategory      {}

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Instance"/> policies
        /// </summary>
        public class InstanceCategory  {}

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Factory"/> policies
        /// </summary>
        public class FactoryCategory   {}

        #endregion
    }
}
