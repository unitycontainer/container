using System.Diagnostics;

namespace Unity
{
    [DebuggerDisplay("{ DebuggerDisplay,nq }", Name = "UnityContainer")]
    public partial class UnityContainer
    {
        internal string DebuggerDisplay => null == Name ? "container[]" : $"{Name}[count]" ;
    }
}
