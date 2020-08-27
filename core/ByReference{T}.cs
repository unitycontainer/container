namespace System
{
    internal readonly ref struct ByReference<T>
    {
#pragma warning disable IDE0051, 169 // Unused local field
        private readonly IntPtr value;
#pragma warning restore IDE0051, 169

        public ByReference(ref T value)
        {
            throw new PlatformNotSupportedException();
        }

        public ref T Value
        {
            get => throw new PlatformNotSupportedException();
        }
    }
}
