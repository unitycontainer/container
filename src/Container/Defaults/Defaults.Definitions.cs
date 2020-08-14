

namespace Unity.Container
{
    public partial class Defaults
    {
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
    }
}
