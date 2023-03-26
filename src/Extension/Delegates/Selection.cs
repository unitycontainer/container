using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// The selector used in selection process of injection constructor
    /// </summary>
    /// <param name="scope">Container scope</param>
    /// <param name="members">Array of <see cref="ConstructorInfo"/> objects to select from</param>
    /// <returns>Selected value</returns>
    public delegate ConstructorInfo? ConstructorSelector(UnityContainer scope, ConstructorInfo[] members);

    /// <summary>
    /// The selector used in selection process of injection fields
    /// </summary>
    /// <param name="scope">Container scope</param>
    /// <param name="members">Array of <see cref="FieldInfo"/> objects to select from</param>
    /// <returns>Selected value</returns>
    public delegate IEnumerable<FieldInfo>? FieldsSelector(UnityContainer scope, ConstructorInfo[] members);

    /// <summary>
    /// The selector used in selection process of injection properties
    /// </summary>
    /// <param name="scope">Container scope</param>
    /// <param name="members">Array of <see cref="FieldInfo"/> objects to select from</param>
    /// <returns>Selected value</returns>
    public delegate IEnumerable<PropertyInfo>? PropertiesSelector(UnityContainer scope, ConstructorInfo[] members);

    /// <summary>
    /// The selector used in selection process of injection methods
    /// </summary>
    /// <param name="scope">Container scope</param>
    /// <param name="members">Array of <see cref="FieldInfo"/> objects to select from</param>
    /// <returns>Selected value</returns>
    public delegate IEnumerable<MethodInfo>? MethodsSelector(UnityContainer scope, ConstructorInfo[] members);
}

