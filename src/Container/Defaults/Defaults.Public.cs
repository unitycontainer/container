using System;
using System.Diagnostics;

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



        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);


        public bool Contains(Type? target, Type type)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = Meta[hash % Meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return true;
                }

                position = Meta[position].Location;
            }

            return false;
        }



        #region Marker Types

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Type"/> policies
        /// </summary>
        public class CategoryType 
        {}

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Instance"/> policies
        /// </summary>
        public class CategoryInstance  
        {}

        /// <summary>
        /// Type identifying <see cref="RegistrationCategory.Factory"/> policies
        /// </summary>
        public class CategoryFactory   
        {}

        #endregion
    }
}
