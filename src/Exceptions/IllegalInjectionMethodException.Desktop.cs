using System;

namespace Unity.Exceptions
{
    /// <summary>
    /// The exception thrown when injection is attempted on a method
    /// that is an open generic or has out or ref params.
    /// </summary>
    [Serializable] 
    public partial class IllegalInjectionMethodException 
    {
    }
}
