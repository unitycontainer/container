namespace Unity.Container
{
    public partial class Defaults
    {
        #region Constants

        /// <summary>
        /// Default name of root container
        /// </summary>
        public const string DEFAULT_ROOT_NAME = "root";

        /// <summary>
        /// Default capacity of root container
        /// </summary>
        public const int DEFAULT_ROOT_CAPACITY = 59;

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
