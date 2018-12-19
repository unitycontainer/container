using System;
using Unity.Builder;

namespace Unity.Policy.Mapping
{
    /// <summary>
    /// Represents a builder policy for mapping build keys.
    /// </summary>
    public class BuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        #region Fields

        private readonly Type _type;

        #endregion


        #region Constructors


        public BuildKeyMappingPolicy(Type type, bool build)
        {
            RequireBuild = build;
            _type = type;
        }

        #endregion


        #region IBuildKeyMappingPolicy


        public Type Map(ref BuilderContext context)
        {
            return _type;
        }

        /// <summary>
        /// Instructs engine to resolve type rather than build it
        /// </summary>
        public bool RequireBuild { get; }

        #endregion
    }
}
