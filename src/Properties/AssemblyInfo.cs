using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: IgnoresAccessChecksTo("System.Private.CoreLib")]

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string _)
        {
        }
    }
}