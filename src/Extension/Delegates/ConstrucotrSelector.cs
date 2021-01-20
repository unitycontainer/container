using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// The selector that depends on the current container's configuration
    /// </summary>
    /// <param name="scope">Container scope</param>
    /// <param name="members">Array of <see cref="ConstructorInfo"/> objects to select from</param>
    /// <returns>Selected value</returns>
    public delegate ConstructorInfo? ConstructorSelector(UnityContainer scope, ConstructorInfo[] members);
}

 