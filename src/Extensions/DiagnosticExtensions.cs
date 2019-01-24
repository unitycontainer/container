using System;
using System.Diagnostics;

namespace Unity
{
    public static class DiagnosticExtensions
    {
        [Conditional("DEBUG")]
        public static void EnableDiagnostic(this UnityContainer container)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
               
            container.AddExtension(new Diagnostic());
        }
    }
}
