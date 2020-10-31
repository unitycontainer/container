using System.Reflection;
using System.Threading.Tasks;
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
        public const int DEFAULT_ROOT_CAPACITY = 59;

        #endregion


        #region Resolution Delegates
        

        public delegate object? ResolveUnregisteredDelegate(ref PipelineContext context);

        public delegate object? ResolveArrayDelegate(ref PipelineContext context);

        public delegate object? ResolveMappedDelegate(ref PipelineContext context);

        #endregion


        #region Producer Delegates

        public delegate ValueTask<object?> RegistrationProducerDelegate(ref PipelineContext context);

        public delegate ValueTask<object?> UnregisteredProducerDelegate(ref PipelineContext context);

        public delegate ValueTask<object?> ArrayProducerDelegate(ref PipelineContext context);

        public delegate ValueTask<object?> MappedTypeProducerDelegate(ref PipelineContext context);

        #endregion


        #region Factory Delegates

        public delegate ResolveDelegate<PipelineContext> BalancedPipelineFactory(ref PipelineContext context);

        public delegate ResolveDelegate<PipelineContext> OptimizedPipelineFactory(ref PipelineContext context);

        public delegate ResolveDelegate<PipelineContext> UnregisteredPipelineFactory(ref PipelineContext context);

        #endregion


        #region

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
