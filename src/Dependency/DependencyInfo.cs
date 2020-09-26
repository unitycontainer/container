using System;

namespace Unity
{
    public readonly ref struct DependencyInfo
    {
        public readonly Type    Type;
        public readonly string? Name;
        public readonly object? Data;

        public DependencyInfo(Type type, string? name, object? data)
        {
            Type = type;
            Name = name;
            Data = data;
        }
    }
}
