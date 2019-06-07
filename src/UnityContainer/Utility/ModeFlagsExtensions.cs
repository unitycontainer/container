namespace Unity.Utility
{
    internal static class ModeFlagsExtensions
    {
        public static bool IsValid(this ModeFlags mode) => ((int)ModeFlags.Activated | (int)ModeFlags.Compiled) !=
                                              ((int)mode & ((int)ModeFlags.Activated | (int)ModeFlags.Compiled));
        public static bool IsOptimized(this ModeFlags mode)  => 0 == ((int)mode & (int)ModeFlags.Diagnostic);
        public static bool IsDiagnostic(this ModeFlags mode) => 0 != ((int)mode & (int)ModeFlags.Diagnostic);
        public static bool IsActivated(this ModeFlags mode)  => 0 != ((int)mode & (int)ModeFlags.Activated);
        public static bool IsCompiled(this ModeFlags mode)   => 0 != ((int)mode & (int)ModeFlags.Compiled);
        public static bool IsLegacy(this ModeFlags mode)     => 0 != ((int)mode & (int)ModeFlags.Legacy);
    }
}
