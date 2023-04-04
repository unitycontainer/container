using System.Reflection;

namespace Unity.Injection;




public interface IInjectionInfo<TMemberInfo> : IInjectionInfo
{
    /// <summary>
    /// One of <see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or
    /// <see cref=" PropertyInfo"/>
    /// </summary>
    TMemberInfo MemberInfo { get; }

    /// <summary>
    /// True if annotated with <see cref="Unity.DependencyResolutionAttribute"/>
    /// </summary>
    bool IsImport { get; set; }
}
