using System;
using System.Reflection;

namespace Unity.Container
{
    public abstract partial class MemberInfoProcessor<TMemberInfo, TData>
    {
        #region Get Dependency Info

        protected virtual DependencyInfo OnGetDependencyInfo(TMemberInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            return new DependencyInfo(attribute.Name, attribute);
        }

        #endregion


        #region DependencyInfo

        protected readonly struct DependencyInfo
        {
            public readonly Type?   Type;
            public readonly string? Name;
            public readonly object? Data;
            // TODO: Lifetime

            public DependencyInfo(Type type, string? name, object? data)
            {
                Type = type;
                Name = name;
                Data = data;
            }

            public DependencyInfo(string? name, object? data)
            {
                Type = default;
                Name = name;
                Data = data;
            }

            public DependencyInfo(object? data)
            {
                Type = default;
                Name = default;
                Data = data;
            }

            public bool IsValid => null != Data || null != Name || null != Type;
        }

        #endregion
    }
}
