using System.Runtime.CompilerServices;

namespace Unity
{
    public static class RegistrationManagerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNoValue(this object? other) 
            => ReferenceEquals(other, UnityContainer.NoValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValue(this object? other)
        => !ReferenceEquals(other, UnityContainer.NoValue);
    }
}
